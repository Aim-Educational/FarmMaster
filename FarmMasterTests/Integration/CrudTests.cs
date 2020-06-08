using FarmMasterTests.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FarmMasterTests.Integration
{
    public class CrudTests : IntegrationTest
    {
        public CrudTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact()]
        public async void ContactCreateTest()
        {
            await this.Client.LoginAsync();

        }
    }
}
