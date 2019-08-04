using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class ComponentCharacteristicsViewModel
    {
        public string IdBoxError { get; set; }
        public string AjaxListUrl { get; set; }
        public string AjaxAddUrl { get; set; }
        public string AjaxDeleteUrl { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
    }

    public class AjaxCharacteristicsRequest : AjaxRequestModel
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

    public class AjaxCharacteristicsAddRequest : AjaxRequestModel
    {
        [Required]
        public int EntityId { get; set; }

        [Required]
        public string EntityType { get; set; }

        [Required]
        public string CharaName { get; set; }

        [Required]
        public AnimalCharacteristic.Type CharaType { get; set; }

        [Required]
        public string CharaValue { get; set; }
    }

    public class AjaxCharacteristicsDeleteByNameRequest : AjaxRequestModel
    {
        [Required]
        public int EntityId { get; set; }
        
        [Required]
        public string EntityType { get; set; }

        [Required]
        public string CharaName { get; set; }
    }
}
