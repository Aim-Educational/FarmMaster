using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using BCrypt.Net;

namespace FarmMaster.Services
{
    public interface IServiceUserManager
    {
        void CreateUser(string username, string password, string firstName, string middleNames, string lastName, string email);
        bool UserExists(string username);
        bool UserPasswordMatches(string username, string password);
    }

    public class ServiceUserManager : IServiceUserManager
    {
        readonly FarmMasterContext _context;

        public ServiceUserManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public void CreateUser(string username, string password, string firstName, string middleNames, string lastName, string email)
        {
            if(this.UserExists(username))
                throw new InvalidOperationException($"The user '{username}' already exists.");
            
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

            var user = new User
            {
                Contact = contact,
                UserLoginInfo = loginInfo
            };

            this._context.Add(contact);
            this._context.Add(loginInfo);
            this._context.Add(user);
            this._context.SaveChanges();
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
    }
}
