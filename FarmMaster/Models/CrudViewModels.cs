using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class CrudIndexViewModel<EntityT>
    where EntityT : class
    {
        public IEnumerable<EntityT> Entities { get; set; }
    }

    public class CrudCreateEditViewModel<EntityT>
    where EntityT : class
    {
        public bool IsCreate { get; set; }
        public EntityT Entity { get; set; }
    }
}
