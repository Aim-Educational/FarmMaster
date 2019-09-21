﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    #region Common (Base)
    public class AjaxRequestModel
    {
        [Required]
        public string SessionToken { get; set; }
    }

    public class AjaxResponseWithMessageModel : ViewModelWithMessage
    {
    }

    public class AjaxResponseWithMessageAndValueModel<T> : AjaxResponseWithMessageModel
    where T : class
    {
        public T Value { get; set; }
    }

    public class AjaxStructReturnValue<T> where T : struct
    {
        public T Value { get; set; }

        public AjaxStructReturnValue(T value)
        {
            this.Value = value;
        }
    }
    #endregion
    
    #region Common (Genericly useable)
    public class AjaxByIdRequest : AjaxRequestModel
    {
        [Required]
        public int? Id { get; set; }
    }

    public class AjaxByIdForIdRequest : AjaxRequestModel
    {
        /// <summary>
        /// The 'parent' id
        /// </summary>
        [Required]
        public int? ById { get; set; }

        /// <summary>
        /// The 'child' id. The Id for the element to take the primary action against.
        /// </summary>
        [Required]
        public int ForId { get; set; }
    }

    public class AjaxByIdForIdWithReasonRequest : AjaxByIdForIdRequest
    {
        [Required]
        [StringLength(75)]
        public string Reason { get; set; }
    }

    public class AjaxByIdWithNameValueRequest : AjaxRequestModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(75)]
        public string Value { get; set; }
    }

    public class AjaxByIdWithNameValueReasonRequest : AjaxByIdWithNameValueRequest
    {
        [Required]
        [StringLength(75)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// Used as a return value.
    /// </summary>
    public class AjaxNameValue
    {
        public string Name;
        public string Value;

        public AjaxNameValue(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    /// <summary>
    /// Used as a return value.
    /// </summary>
    public class AjaxNameValueId : AjaxNameValue
    {
        public int Id;

        public AjaxNameValueId(string name, string value, int id) : base(name, value)
        {
            this.Id = id;
        }
    }
    #endregion
}