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

    public interface IServiceUserManager
    {
        void CreateUser(string username, string password, string firstName, string middleNames, string lastName, string email,
                        bool tosConsent, bool privacyConsent);
        bool UserExists(string username);
        bool UserPasswordMatches(string username, string password);
        void RenewSession(User user, HttpContext http);
        User UserFromCookieSession(HttpContext http);
        User UserFromLoginInfo(string username, string password);
        void SendEmailVerifyEmail(User user);
    }

    public class ServiceUserManager : IServiceUserManager
    {
        readonly FarmMasterContext _context;
        readonly IServiceSmtpClient _smtp;
        readonly IOptions<IServiceSmtpDomainConfig> _domains;
        readonly IOptions<IServiceUserManagerConfig> _config;
        User _user; // This service is scoped, so we cache the user each time we get a request to reduce server load.

        public ServiceUserManager(FarmMasterContext context, 
                                  IServiceSmtpClient smtp, 
                                  IOptions<IServiceSmtpDomainConfig> domains,
                                  IOptions<IServiceUserManagerConfig> config)
        {
            this._context = context;
            this._smtp = smtp;
            this._domains = domains;
            this._config = config;
        }

        public void CreateUser(string username, string password, string firstName, string middleNames, string lastName, string email, bool tosConsent, bool privacyConsent)
        {
            if(this.UserExists(username))
                throw new InvalidOperationException($"The user '{username}' already exists.");

            if(!tosConsent)
                throw new InvalidOperationException($"The user must give consent to the Terms of Service.");

            if(!privacyConsent)
                throw new InvalidOperationException($"The user must give consent to the Privacy Policy.");
            
            var contact = new Contact
            {
                FirstName = firstName,
                MiddleNames = middleNames,
                LastName = lastName,
                Email = email
            };

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

            this._context.Add(contact);
            this._context.Add(loginInfo);
            this._context.Add(privacy);
            this._context.Add(user);
            this._context.SaveChanges(); // We do a save here despite the function below also doing so,
                                         // so we can catch any errors *before* sending out the email.

            this.SendEmailVerifyEmail(user);
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

        public bool UserExists(string username)
        {
            return this._context.UserLoginInfo.Any(i => i.Username == username);
        }

        public User UserFromCookieSession(HttpContext http)
        {
            if(this._user != null)
                return this._user;

            if(!http.Request.Cookies.ContainsKey(GlobalConstants.AuthCookieName))
                return null;
            
            var user = this._context.Users
                                    .Include(u => u.Contact)
                                    .Include(u => u.Role)
                                    .Include(u => u.UserLoginInfo)
                                    .Include(u => u.UserPrivacy)
                                    .SingleOrDefault(u => u.UserLoginInfo.SessionToken == http.Request.Cookies[GlobalConstants.AuthCookieName]);
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

            this._user = this._context.Users
                                      .Include(u => u.Contact)
                                      .Include(u => u.Role)
                                      .Include(u => u.UserLoginInfo)
                                      .Include(u => u.UserPrivacy)
                                      .Single(u => u.UserLoginInfo.Username == username);

            return this._user;
        }
    }
}
