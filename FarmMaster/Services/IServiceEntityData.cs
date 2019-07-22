using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services
{
    public interface IServiceEntityData<T> where T : class
    {
        IQueryable<T> Query();
        IQueryable<T> QueryAllIncluded();
        int GetIdFor(T entity);
    }

    public static class IServiceEntityDataExtentions
    {
        public static T FromId<T>(this IServiceEntityData<T> data, int id) where T : class
        {
            return data.Query().FirstOrDefault(d => data.GetIdFor(d) == id);
        }

        public static T FromIdAllIncluded<T>(this IServiceEntityData<T> data, int id) where T : class
        {
            return data.QueryAllIncluded().FirstOrDefault(d => data.GetIdFor(d) == id);
        }
    }
}