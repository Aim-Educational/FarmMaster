using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FarmMaster.Models
{
    public abstract class ViewModelWithMessage
    {
        const int PREFIX_LENGTH = 2;

        public enum Type
        {
            None,
            Information,
            Warning,
            Error
        }

        public enum Format
        {
            Default,
            UnorderedList
        }

        public Type MessageType;
        public Format MessageFormat;
        public string Message;

        public bool HasMessage => Message != null;

        public void ParseMessageQueryString(string message)
        {
            if(message == null || message.Length < PREFIX_LENGTH)
                return;

            Type type = Type.None;
                 if(message[0] == 'i') type = Type.Information;
            else if(message[0] == 'w') type = Type.Warning;
            else if(message[0] == 'e') type = Type.Error;

            Format format = Format.Default;
                 if(message[1] == 'd') format = Format.Default;
            else if(message[1] == 'u') format = Format.UnorderedList;

            this.MessageType = type;
            this.MessageFormat = format;

            if(message.Length > PREFIX_LENGTH)
                this.Message = message.Substring(PREFIX_LENGTH);
        }

        public void ParseInvalidModelState(ModelStateDictionary modelState)
        {
            this.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(modelState));
        }

        public static string CreateMessageQueryString(Type type, string message, Format format = Format.Default)
        {
            return 
                $"{Enum.GetName(typeof(Type), type).ToLower()[0]}"
              + $"{Enum.GetName(typeof(Format), format).ToLower()[0]}"
              + message;
        }

        public static string CreateQueryString(ModelStateDictionary modelState)
        {
            return ViewModelWithMessage.CreateMessageQueryString(
                Type.Error,
                modelState
                .Select(s => s.Value)
                .SelectMany(e => e.Errors)
                .Select(e => e.ErrorMessage)
                .Aggregate((s1, s2) => $"{s1}\n{s2}"),
                Format.UnorderedList
            );
        }

        public static string CreateErrorQueryString(string message, Format format = Format.Default)
        {
            return ViewModelWithMessage.CreateMessageQueryString(Type.Error, message, format);
        }

        public static string CreateWarningQueryString(string message, Format format = Format.Default)
        {
            return ViewModelWithMessage.CreateMessageQueryString(Type.Warning, message, format);
        }

        public static string CreateInfoQueryString(string message, Format format = Format.Default)
        {
            return ViewModelWithMessage.CreateMessageQueryString(Type.Information, message, format);
        }
    }

    public class EmptyViewModelWithMessage : ViewModelWithMessage { }
}
