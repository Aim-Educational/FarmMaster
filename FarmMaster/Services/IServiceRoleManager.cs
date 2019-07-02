using Business.Model;
using FarmMaster.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceRoleManager
    {
        Role CreateRole(string name, params string[] permInternalNames);
        void AddPermission(Role role, string permInternalName, SaveChanges saveChanges = SaveChanges.Yes);
        void RemovePermission(Role role, string permInternalName, SaveChanges saveChanges = SaveChanges.Yes);
        bool HasPermission(Role role, string permInternalName);
    }

    public class ServiceRoleManager : IServiceRoleManager
    {
        readonly FarmMasterContext _context;

        public ServiceRoleManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public void AddPermission(Role role, string permInternalName, SaveChanges saveChanges = SaveChanges.Yes)
        {
            this.LoadPermissions(role);
            var perm = this._context.EnumRolePermissions.Single(p => p.InternalName == permInternalName);

            if (!role.Permissions?.Any(p => p.EnumRolePermission == perm) ?? true)
            {
                var map = new MapRolePermissionToRole
                {
                    EnumRolePermission = perm,
                    Role = role
                };

                this._context.Add(map);

                if(saveChanges == SaveChanges.Yes)
                    this._context.SaveChanges();
            }
        }

        public Role CreateRole(string name, params string[] permInternalNames)
        {
            if(this._context.Roles.Any(r => r.Name == name))
                throw new ArgumentException($"A role called {name} already exists.", "name");

            var role = new Role
            {
                Name = name
            };

            this._context.Add(role);

            foreach(var permName in permInternalNames)
                this.AddPermission(role, permName, SaveChanges.No);

            this._context.SaveChanges();
            return role;
        }

        public bool HasPermission(Role role, string permInternalName)
        {
            this.LoadPermissions(role);
            return role.Permissions?.Any(p => p.EnumRolePermission.InternalName == permInternalName)
                ?? false;
        }

        public void RemovePermission(Role role, string permInternalName, SaveChanges saveChanges = SaveChanges.Yes)
        {
            this.LoadPermissions(role);
            var map = role.Permissions?.SingleOrDefault(p => p.EnumRolePermission.InternalName == permInternalName);
            if (map != null)
            {
                this._context.Remove(map);

                if(saveChanges == SaveChanges.Yes)
                    this._context.SaveChanges();
            }
        }

        private void LoadPermissions(Role role)
        {
            this._context.Entry(role).Collection(r => r.Permissions).Load();
            if(role.Permissions != null)
            {
                foreach(var map in role.Permissions)
                    this._context.Entry(map).Reference(m => m.EnumRolePermission).Load();
            }
        }
    }
}
