using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class HoldingIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<Holding> Holdings;
    }

    public class HoldingCreateEditViewModel : ViewModelWithMessage
    {
        public bool IsCreate { get; set; } // Only the view cares about this.
        public Holding Holding { get; set; }
        public IDictionary<string, bool> SelectedRegistrations { get; set; }
        public IDictionary<string, string> SelectedRegistrationHerdNumbers { get; set; }
        public IEnumerable<EnumHoldingRegistration> AllRegistrations { get; set; }
    }
}
