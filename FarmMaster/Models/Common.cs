using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public abstract class ViewModelWithMessage
    {
        public enum Type
        {
            None,
            Information,
            Warning,
            Error
        }

        public Type MessageType;
        public string Message;

        public bool HasMessage => Message != null;

        public void ParseMessageQueryString(string message)
        {
            if(message == null || message.Length == 0)
                return;

            Type type = Type.None;
                 if(message[0] == 'i') type = Type.Information;
            else if(message[0] == 'w') type = Type.Warning;
            else if(message[0] == 'e') type = Type.Error;

            this.MessageType = type;

            if(message.Length > 1)
                this.Message = message.Substring(1);
        }

        public static string CreateMessageQueryString(Type type, string message)
        {
            return Enum.GetName(typeof(Type), type).ToLower()[0] + message;
        }
    }
}
