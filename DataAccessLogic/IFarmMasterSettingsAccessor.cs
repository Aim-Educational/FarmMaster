using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLogic
{
    public interface IFarmMasterSettingsAccessor
    {
        Settings Settings { get; set; }
    }

    public class FarmMasterSettingsAccessor : IFarmMasterSettingsAccessor
    {
        readonly FarmMasterContext _context;

        public FarmMasterSettingsAccessor(FarmMasterContext context)
        {
            this._context = context;
        }

        public Settings Settings
        {
            get
            {
                var settings = this._context.Settings.FirstOrDefault();
                if(settings == null)
                {
                    settings = this._context.Settings.Add(new Settings()).Entity;
                    this._context.SaveChanges();
                }

                return settings;
            }

            set
            {
                var settings = this.Settings;
                value.SettingsKey = settings.SettingsKey;
                this._context.Settings.Update(value);
                this._context.SaveChanges();
            }
        }
    }
}
