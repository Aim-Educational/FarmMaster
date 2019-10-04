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
        public string AjaxListUrl { get; set; }
        public string AjaxAddUrl { get; set; }
        public string AjaxDeleteUrl { get; set; }
        public int EntityId { get; set; }
    }

    public class AjaxCharacteristicsResponseValue
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool IsInherited { get; set; }
        public int Id { get; set; }
    }

    public class AjaxCharacteristicsAddRequest : AjaxRequestModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DynamicField.Type Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
