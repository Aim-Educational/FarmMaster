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
    }

    public class ServiceUserManager : IServiceUserManager
    {
        readonly FarmMasterContext _context;
        readonly IServiceSmtpClient _smtp;
        readonly IServiceRoleManager _roles;
        readonly IServiceContactManager _contacts;
        readonly IOptions<IServiceSmtpDomainConfig> _domains;
        readonly IOptions<IServiceUserManagerConfig> _config;
        User _user; // This service is scoped, so we cache the user each time we get a request to reduce server load.

        public ServiceUserManager(FarmMasterContext context, 
                                  IServiceSmtpClient smtp, 
                                  IServiceRoleManager roles,
                                  IServiceContactManager contacts,
                                  IOptions<IServiceSmtpDomainConfig> domains,
                                  IOptions<IServiceUserManagerConfig> config)
        {
            this._context = context;
            this._smtp = smtp;
            this._domains = domains;
            this._config = config;
            this._roles = roles;
            this._contacts = contacts;
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
                PrivacyPolicyVersionAgreedTo = GlobalConstants.PrivacyPolicyVersion,
                TermsOfServiceVersionAgreedTo = GlobalConstants.TermsOfServiceVersion
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
                GlobalConstants.AuthCookieName,
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
                (http.Request.Cookies.ContainsKey(GlobalConstants.AuthCookieName))
                ? http.Request.Cookies[GlobalConstants.AuthCookieName]
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
                EnumEmailTemplateNames.EmailVerify, 
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
    }
}
