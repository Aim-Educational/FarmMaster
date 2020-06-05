using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Module.Core.Models
{
    class BreadcrumbPairs : Dictionary<string, string>
    {

    }

    /// <summary>
    /// A ViewModel used to create the breadcrumbs shown on page.
    /// </summary>
    /// <remarks>
    /// Breadcrumbs are shown in the same order that they are added.
    /// 
    /// Breadcrumbs can be grouped together using <see cref="Breadcrumb.For(string)"/>.
    /// 
    /// A group can be activated by using <see cref="Breadcrumb.Use(string, bool)"/>.
    /// 
    /// Only the active group, and any breadcrumbs added before the use of <see cref="Breadcrumb.For(string)"/>
    /// are shown on screen.
    /// </remarks>
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
