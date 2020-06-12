using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLogic.Util
{
    class ForeignKeyStore<EntityT>
    {
        public int?                  IdValue;
        public object                InstanceValue;
        public Func<EntityT, int?>   IdSelector;
        public Func<EntityT, object> InstanceSelector;
    }

    public class MutuallyExclusiveForeignKeys<EnumT, EntityT>
    where EnumT   : Enum
    where EntityT : class
    {
        private readonly ForeignKeyStore<EntityT>[] _keyStore;

        public MutuallyExclusiveForeignKeys()
        {
            var count      = Enum.GetNames(typeof(EnumT)).Length;
            this._keyStore = new ForeignKeyStore<EntityT>[count]; 
        }

        public MutuallyExclusiveForeignKeys<EnumT, EntityT> Define(
            EnumT type, 
            Func<EntityT, int?> idSelector, 
            Func<EntityT, object> instanceSelector
        )
        {
            var index = (int)(object)type;
            if(this._keyStore[index] != null)
                throw new InvalidOperationException($"Multiple definitions for key type: {type}");

            this._keyStore[index] = new ForeignKeyStore<EntityT> 
            {
                IdSelector       = idSelector,
                InstanceSelector = instanceSelector
            };

            return this;
        }

        public bool IsUniquelySet(EnumT type, EntityT entity)
        {
            this.PopulateValues(entity);

            var index  = (int)(object)type;
            var amISet = false;
            for (int i = 0; i < this._keyStore.Length; i++)
            {
                var store = this._keyStore[i];
                var isSet = store.IdValue != null || store.InstanceValue != null;

                if (i == index && isSet)
                    amISet = true;
                else if (isSet)
                    return false;
            }

            return amISet;
        }

        private void PopulateValues(EntityT entity)
        {
            for(int i = 0; i < this._keyStore.Length; i++)
            {
                var store           = this._keyStore[i];
                store.IdValue       = store.IdSelector(entity);
                store.InstanceValue = store.InstanceSelector(entity);
            }
        }
    }
}
