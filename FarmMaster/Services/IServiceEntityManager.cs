using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceEntityManager<T> where T : class
    {
        IQueryable<T> Query();
        IQueryable<T> QueryAllIncluded();
        int GetIdFor(T entity);
        void Update(T entity);
    }

    public static class IServiceEntityDataExtentions
    {
        public static T FromId<T>(this IServiceEntityManager<T> data, int id) where T : class
        {
            return data.Query().FirstOrDefault(d => data.GetIdFor(d) == id);
        }

        public static T FromIdAllIncluded<T>(this IServiceEntityManager<T> data, int id) where T : class
        {
            return data.QueryAllIncluded().FirstOrDefault(d => data.GetIdFor(d) == id);
        }

        // NOTE: Intelisense seems a bit bugged with this function, and can't figure out it exists when giving the function list.
        // For managers that manage multiple entity types, there's a specific issue of not being able to choose,
        // which entity to use the functions for easily, so this identity function is just an easy way to access,
        // the functions for a specific entity type.
        public static IServiceEntityManager<T> For<T>(this IServiceEntityManager<T> data) where T: class
        {
            return data;
        }
    }
}