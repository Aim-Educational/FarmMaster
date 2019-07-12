using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class EmailContactInfoAuditViewModel
    {
        public class Data
        {
            public string Who;
            public string What;
            public string When;
            public string Why;
            public string AdditionalInfo;
        }

        public IEnumerable<Data> ActionsTaken;
    }
}
