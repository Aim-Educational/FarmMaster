using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using BCrypt.Net;
using FarmMaster.Misc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Contracts;

namespace FarmMaster.Services
{
    public class IServiceUserManagerConfig
    {
        public TimeSpan SessionTokenLifespan { get; set; }
    }

    public interface IServiceUserManager : IServiceEntityManager<User>
    {
        User Create(string username, string password, string fullName, string email,
                    bool tosConsent, bool privacyConsent);
        bool UserExists(string username);
        bool UserPasswordMatches(string username, string password);
        void RenewSession(User user, HttpContext http);
        void EndSession(User user, HttpContext http);
        User UserFromCookieSession(HttpContext http);
        User UserFromCookieSession(string sessionToken);
        User UserFromLoginInfo(string username, string password);
        void SendEmailVerifyEmail(User user);
        void FinishEmailVerify(string token);

        JObject UserGdprData(User user);
    }

    public class ServiceUserManager : IServiceUserManager
    {
        readonly FarmMasterContext _context;
        readonly IServiceSmtpClient _smtp;
        readonly IServiceRoleManager _roles;
        readonly IServiceContactManager _contacts;
        readonly IServiceHoldingManager _holdings;
        readonly IServiceSpeciesBreedManager _speciesBreeds;
        readonly IOptions<IServiceSmtpDomainConfig> _domains;
        readonly IOptions<IServiceUserManagerConfig> _config;
        User _user; // This service is scoped, so we cache the user each time we get a request to reduce server load.

        public ServiceUserManager(FarmMasterContext context, 
                                  IServiceSmtpClient smtp, 
                                  IServiceRoleManager roles,
                                  IServiceContactManager contacts,
                                  IOptions<IServiceSmtpDomainConfig> domains,
                                  IOptions<IServiceUserManagerConfig> config,
                                  IServiceHoldingManager holdings,
                                  IServiceSpeciesBreedManager speciesBreeds)
        {
            this._context = context;
            this._smtp = smtp;
            this._domains = domains;
            this._config = config;
            this._roles = roles;
            this._contacts = contacts;
            this._holdings = holdings;
            this._speciesBreeds = speciesBreeds;
        }

        public User Create(string username, string password, string fullName, string email, bool tosConsent, bool privacyConsent)
        {
            if(this.UserExists(username))
                throw new InvalidOperationException($"The user '{username}' already exists.");

            if(!tosConsent)
                throw new InvalidOperationException($"The user must give consent to the Terms of Service.");

            if(!privacyConsent)
                throw new InvalidOperationException($"The user must give consent to the Privacy Policy.");
            
            var contact = this._contacts.Create(Contact.Type.User, fullName, SaveChanges.No);
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var loginInfo = new UserLoginInfo
            {
                Username = username,
                Salt = salt,
                PassHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password + salt)
            };
            
            var privacy = new UserPrivacy
            {
                HasVerifiedEmail = false,
                PrivacyPolicyVersionAgreedTo = FarmConstants.Versions.PrivacyPolicy,
                TermsOfServiceVersionAgreedTo = FarmConstants.Versions.TermsOfService
            };

            var user = new User
            {
                Contact = contact,
                UserLoginInfo = loginInfo,
                UserPrivacy = privacy
            };

            var emailDb = new Email
            {
                Contact = contact,
                Name = "Default",
                Address = email
            };
            
            this._context.Add(loginInfo);
            this._context.Add(privacy);
            this._context.Add(user);
            this._context.Add(emailDb);
            this._context.SaveChanges(); // We do a save here despite the function below also doing so,
                                         // so we can catch any errors *before* sending out the email.

            this.SendEmailVerifyEmail(user);

            // If this is the first user to be registered, give them a role that can modify Roles,
            // as they're likely to be the account for setting up the system.
            if(this._context.Users.Count() == 1)
            {
                var role = this._roles.Create(
                    "Admin", 
                    "An administrator", 
                    BusinessConstants.Roles.VIEW_ROLES, 
                    BusinessConstants.Roles.EDIT_ROLES
                );

                user.Role = role;
                this._context.SaveChanges();
            }

            return user;
        }

        public void RenewSession(User user, HttpContext http)
        {
            user.UserLoginInfo.SessionToken = Guid.NewGuid().ToString();
            user.UserLoginInfo.SessionTokenExpiry = DateTimeOffset.UtcNow + this._config.Value.SessionTokenLifespan;

            http.Response.Cookies.Append(
                FarmConstants.CookieNames.AuthCookie,
                user.UserLoginInfo.SessionToken,
                new CookieOptions
                {
                    Expires     = user.UserLoginInfo.SessionTokenExpiry,
                    IsEssential = true, // Auth cookies are exempt from cookie consent laws.
                    Secure      = true,
                    SameSite    = SameSiteMode.Strict
                }
            );

            this._context.SaveChanges();
        }

        public void EndSession(User user, HttpContext http)
        {
            user.UserLoginInfo.SessionTokenExpiry = DateTimeOffset.UtcNow;
            this._context.SaveChanges();
        }

        public bool UserExists(string username)
        {
            return this._context.UserLoginInfo.Any(i => i.Username == username);
        }

        public User UserFromCookieSession(HttpContext http)
        {
            return this.UserFromCookieSession(
                (http.Request.Cookies.ContainsKey(FarmConstants.CookieNames.AuthCookie))
                ? http.Request.Cookies[FarmConstants.CookieNames.AuthCookie]
                : null
            );
        }

        public User UserFromCookieSession(string sessionToken)
        {
            if(this._user != null)
                return this._user;

            if(sessionToken == null)
                return null;
            
            var user = this.QueryAllIncluded().SingleOrDefault(u => u.UserLoginInfo.SessionToken == sessionToken);
            if(user == null
            || user.UserLoginInfo.SessionTokenExpiry <= DateTimeOffset.UtcNow)
                return null;

            this._user = user;
            return user;
        }

        public bool UserPasswordMatches(string username, string password)
        {
            if(!this.UserExists(username))
                throw new InvalidOperationException($"The user '{username}' does not exist.");

            var info = this._context.UserLoginInfo.First(i => i.Username == username);
            if(info == null)
                throw new NullReferenceException("Unknown error. Somehow 'info' is null.");

            return BCrypt.Net.BCrypt.EnhancedVerify(password + info.Salt, info.PassHash);
        }

        public void SendEmailVerifyEmail(User user)
        {
            user.UserPrivacy.EmailVerificationToken = Guid.NewGuid().ToString();
            this._context.SaveChanges();

            this._smtp.SendToWithTemplateAsync(
                user,
                FarmConstants.EmailTemplateNames.EmailVerify, 
                "Please verify your email.",
                this._domains.Value.VerifyEmail + user.UserPrivacy.EmailVerificationToken
            ).Wait();
        }

        public User UserFromLoginInfo(string username, string password)
        {
            if(!this.UserExists(username))
                throw new Exception($"The user '{username}' does not exist.");

            if(!this.UserPasswordMatches(username, password))
                throw new Exception($"The password is incorrect.");

            return this.QueryAllIncluded().Single(u => u.UserLoginInfo.Username == username);
        }

        public void FinishEmailVerify(string token)
        {
            var info = this._context.UserPrivacy.SingleOrDefault(i => i.EmailVerificationToken == token);
            if(info == null)
                throw new Exception($"An email with that token doesn't exist.");

            info.EmailVerificationToken = null;
            info.HasVerifiedEmail = true;
            this._context.SaveChanges();
        }

        public IQueryable<User> QueryAllIncluded()
        {
            return this._context.Users
                                .Include(u => u.Contact)
                                 .ThenInclude(c => c.EmailAddresses)
                                .Include(u => u.Contact)
                                 .ThenInclude(c => c.PhoneNumbers)
                                .Include(u => u.Role)
                                 .ThenInclude(r => r.Permissions)
                                 .ThenInclude(p => p.EnumRolePermission)
                                .Include(u => u.UserLoginInfo)
                                .Include(u => u.UserPrivacy);
        }

        public IQueryable<User> Query()
        {
            return this._context.Users;
        }

        public int GetIdFor(User entity)
        {
            return entity.UserId;
        }

        public void Update(User entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "No")]
        public JObject UserGdprData(User user)
        {
            Contract.Assert(user != null);

            var json = new JObject();
            json["user"] = JObject.FromObject(new 
            {
                user.UserId,
                RoleName = user.Role.Name,

                LoginInfo = new 
                {
                    user.UserLoginInfo.Username,
                    user.UserLoginInfo.PassHash
                },

                Privacy = new 
                {
                    user.UserPrivacy.HasVerifiedEmail,
                    user.UserPrivacy.PrivacyPolicyVersionAgreedTo,
                    user.UserPrivacy.TermsOfServiceVersionAgreedTo
                },

                Contact = new 
                {
                    ContactType = Convert.ToString(user.Contact.ContactType),
                    user.Contact.FullName,
                    user.Contact.IsAnonymous,
                    Emails = user.Contact.EmailAddresses.Select(e => new { e.Name, e.Address }),
                    Phones = user.Contact.PhoneNumbers.Select(p => new { p.Name, p.Number }),
                    Relationships = user.Contact.GetRelationships(this._context).Select(r => new 
                    {
                        r.Description,
                        ContactOneAbbreviatedName = r.ContactOne.FirstNameWithAbbreviatedLastName,
                        ContactTwoAbrreviatedName = r.ContactTwo.FirstNameWithAbbreviatedLastName,
                        Note = "Both contacts have their full names stored, but to protect the contact that isn't you, the names are abbreviated"
                    })
                },

                Holdings = this._holdings
                               .QueryAllIncluded()
                               .Where(h => h.OwnerContact == user.Contact)
                               .Select(h => new 
                {
                    h.Address,
                    h.GridReference,
                    h.HoldingNumber,
                    h.Name,
                    h.Postcode,
                    Registrations = h.Registrations.Select(r => new
                    {
                        r.HerdNumber,
                        r.HoldingRegistration.Description,
                        r.RareBreedNumber
                    })
                }),

                ActionsAgainstContacts = this._context
                                             .ActionsAgainstContactInfo
                                             .Where(a => a.UserResponsible == user || a.ContactAffected == user.Contact)
                                             .Include(a => a.ContactAffected)
                                             .Include(a => a.UserResponsible)
                                              .ThenInclude(u => u.Contact)
                                             .Select(a => new 
                {
                    ActionType = Convert.ToString(a.ActionType),
                    a.AdditionalInfo,
                    AffectedAbbreviatedName = a.ContactAffected.FirstNameWithAbbreviatedLastName,
                    a.DateTimeUtc,
                    a.HasContactBeenInformed,
                    a.Reason,
                    ResponsibleAbbreviatedName = a.UserResponsible.Contact.FirstNameWithAbbreviatedLastName
                }),

                BreedsAssociatedWith = this._speciesBreeds
                                           .For<Breed>()
                                           .QueryAllIncluded()
                                           .Where(b => b.BreedSociety == user.Contact)
                                           .Select(b => b.Name)
            });

            return json;
        }
    }
}
