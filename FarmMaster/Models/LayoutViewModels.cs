﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    class BreadcrumbPairs : Dictionary<string, string>
    {

    }

    public class Breadcrumb : IEnumerable<KeyValuePair<string, string>>
    {
        const string KEY_ALWAYS_USE = "__ALWAYS_USE";
        readonly IDictionary<string, BreadcrumbPairs> _breadcrumbs;
        string _currentKey = KEY_ALWAYS_USE; // The key to use when adding a new crumb.
        string _keyToUse = KEY_ALWAYS_USE;   // The key to use when getting the final crumb results.

        public Breadcrumb()
        {
            this._breadcrumbs = new Dictionary<string, BreadcrumbPairs>();
        }

        public Breadcrumb Add(string name, string href)
        {
            var exists = this._breadcrumbs.TryGetValue(this._currentKey, out BreadcrumbPairs crumbs);
            if(!exists)
            {
                crumbs = new BreadcrumbPairs();
                this._breadcrumbs.Add(this._currentKey, crumbs);
            }

            crumbs.Add(name, href);
            return this;
        }


        public Breadcrumb For(string name)
        {
            this._currentKey = name;
            return this;
        }

        public Breadcrumb Use(string name, bool condition = true)
        {
            if(condition)
                this._keyToUse = name;

            return this;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            var exists = this._breadcrumbs.TryGetValue(KEY_ALWAYS_USE, out BreadcrumbPairs crumbsToAlwaysShow);
            if(!exists)
                crumbsToAlwaysShow = new BreadcrumbPairs();

            var otherCrumbs = new BreadcrumbPairs();
            if(this._keyToUse != KEY_ALWAYS_USE)
                this._breadcrumbs.TryGetValue(this._keyToUse, out otherCrumbs);

            return crumbsToAlwaysShow.Concat(otherCrumbs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
