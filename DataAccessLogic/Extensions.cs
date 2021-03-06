﻿using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAccessLogicLayer(this IServiceCollection collection)
        {
            return collection.AddScoped<IFarmMasterSettingsAccessor, FarmMasterSettingsAccessor>()
                             .AddScoped<IUnitOfWork, DbContextUnitOfWork<FarmMasterContext>>()
                             .AddScoped<IContactManager, ContactManager>()
                             .AddScoped<INoteManager, NoteManager>()
                             .AddScoped<ISpeciesManager, SpeciesManager>()
                             .AddScoped<IBreedManager, BreedManager>()
                             .AddScoped<ILocationManager, LocationManager>();
        }
    }
}
