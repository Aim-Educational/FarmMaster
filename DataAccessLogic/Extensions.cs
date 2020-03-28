using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAccessLogicLayer(this IServiceCollection collection)
        {
            return collection.AddScoped<IFarmMasterSettingsAccessor, FarmMasterSettingsAccessor>()
                             .AddScoped<IUnitOfWork, DbContextUnitOfWork<FarmMasterContext>>()
                             .AddScoped<IContactManager, ContactManager>();
        }
    }
}
