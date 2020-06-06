using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataAccessLogic
{
    public class ResultObject
    {
        private static ResultObject _okInstance;

        public bool Succeeded { get; set; }
        public IList<string> Errors { get; set; }
        public IList<Exception> Exceptions { get; set; }

        public ResultObject()
        {
            this.Errors = new List<string>();
            this.Exceptions = new List<Exception>();
        }

        public static ResultObject Ok
        {
            get
            {
                if (_okInstance == null)
                {
                    _okInstance = new ResultObject()
                    {
                        Succeeded = true
                    };
                }

                return _okInstance;
            }
        }

        public IEnumerable<string> GatherErrorMessages()
        {
#if DEBUG
            // To help with debugging.
            if (this.Exceptions.Any())
                Debugger.Break();
#endif

            return this.Exceptions
                       .Select(e => e.Message)
                       .Concat(this.Errors);
        }
    }

    public class ValueResultObject<ValueT> : ResultObject
    {
        public ValueT Value { get; set; }
    }
}
