using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class EmailContactInfoAuditViewModel
    {
        #pragma warning disable CA1724
        public class Data
        {
            public string Who;
            public string What;
            public string When;
            public string Why;
            public string AdditionalInfo;
        }
        #pragma warning restore CA1724

        public IEnumerable<Data> ActionsTaken;
    }
}
