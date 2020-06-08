using DataAccessLogic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessGraphQL.Util
{
    public static class CrudHelper
    {
        public static Task<List<EntityT>> GetPageAsync<EntityT>(this ICrudAsync<EntityT> crud, int after, int take)
        where EntityT : class
        {
            return crud.Query()
                       .Skip(after)
                       .Take(take)
                       .ToListAsync();
        }
    }
}
