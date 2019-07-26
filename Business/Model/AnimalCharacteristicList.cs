using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class AnimalCharacteristicList
    {
        [Key]
        public int AnimalCharacteristicListId { get; set; }

        public IEnumerable<AnimalCharacteristic> Characteristics { get; set; }
    }
}
