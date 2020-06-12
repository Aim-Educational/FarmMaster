using DataAccess;
using DataAccessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccessLogicTests
{
    public class MutuallyExclusiveForeignKeysTests
    {
        [Fact()]
        public void TestNoValuesSetIsFalse()
        {
            var keys = LocationManager._MutualKeys;
            var emptyLocation = new Location();

            foreach(var value in this.GetLocationTypes())
                Assert.False(keys.IsUniquelySet(value, emptyLocation));
        }

        [Fact()]
        public void TestMultipleValuesSetIsFalse()
        {
            // I... I can't actually test this yet until we add another location type.
        }

        [Fact()]
        public void TestSingleInstanceIsTrue()
        {
            this.TestAllValues(new Location{ Holding = new LocationHolding() }, LocationType.Holding);
        }

        private void TestAllValues(Location location, LocationType happyType)
        {
            var keys = LocationManager._MutualKeys;
            foreach (var value in this.GetLocationTypes())
            {
                if (value == happyType)
                    Assert.True(keys.IsUniquelySet(value, location));
                else
                    Assert.False(keys.IsUniquelySet(value, location));
            }
        }

        private IEnumerable<LocationType> GetLocationTypes()
        {
            return Enum.GetValues(typeof(LocationType)).Cast<LocationType>();
        }
    }
}
