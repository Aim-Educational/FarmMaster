using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class ComponentCharacteristicsViewModel
    {
        public string IdBoxError { get; set; }
        public string AjaxUrl { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
    }

    public class AjaxCharacteristicsRequest : AjaxModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public int Id { get; set; }
    }

    public class AjaxCharacteristicsResponseValue
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
    }
}
