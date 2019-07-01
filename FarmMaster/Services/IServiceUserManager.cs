using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using BCrypt.Net;
using FarmMaster.Misc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace FarmMaster.Services
{
    public interface IServiceUserManager
    {
        void CreateUser(string username, string password, string firstName, string middleNames, string lastName, string email,
                        bool tosConsent, bool privacyConsent);
        bool UserExists(string username);
        bool UserPasswordMatches(string username, string password);
    }

    public class ServiceUserManager : IServiceUserManager
    {
        readonly FarmMasterContext _context;
        readonly IServiceSmtpClient _smtp;
        readonly IOptions<IServiceSmtpDomainConfig> _domains;

        public ServiceUserManager(FarmMasterContext context, IServiceSmtpClient smtp, IOptions<IServiceSmtpDomainConfig> domains)
        {
            this._context = context;
            this._smtp = smtp;
            this._domains = domains;
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
            this._context.SaveChanges(); // We do a save hear despite the function below also doing so,
                                         // so we can catch any errors *before* sending out the email.

            this.SendEmailVerifyEmail(user);
        }

        public bool UserExists(string username)
        {
            return this._context.UserLoginInfo.Any(i => i.Username == username);
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

        private void SendEmailVerifyEmail(User user)
        {
            user.UserPrivacy.EmailVerificationToken = Guid.NewGuid().ToString();
            this._context.SaveChanges();

            this._smtp.SendToWithTemplateAsync(
                user,
                EnumEmailTemplateNames.EmailVerify, 
                "Please verify your email.",
                this._domains.Value.VerifyEmail
            ).Wait();
        }
    }
}
