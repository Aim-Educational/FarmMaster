using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Module.Core.Models
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

    public class CrudLayoutViewModel
    {
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string EntityIdExpression { get; set; }
    }
}
