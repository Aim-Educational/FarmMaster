using DataAccess;
using FarmMasterTests.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
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
        public async void ContactTest()
        {
            // Create
            await this.Client.LoginAsync();
            await this.Client.CrudCreateAsync("/Contact/Create", new Contact
            { 
                Name  = "Bradley Chatha",
                Email = "bradley.chatha@gmail.com",
                Phone = "69420",
                Type  = ContactType.Individual
            });

            // Read(Index)
            var response = await this.Client.GetEnsureStatusAsync("/Contact", HttpStatusCode.OK);
            var content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Bradley Chatha", content);

            // Read(Edit page)
            response = await this.Client.GetEnsureStatusAsync("/Contact/Edit?id=1", HttpStatusCode.OK);
            content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Bradley Chatha",               content);
            Assert.Contains("bradley.chatha@gmail.com",     content);
            Assert.Contains("69420",                        content);
            Assert.Contains(nameof(ContactType.Individual), content);

            // Edit & Read to ensure all changes take affect. (code dupe, but meh, I've had situtations before where singular fields won't update, so I want to test them all)
            await this.Client.CrudEditAsync("/Contact/Edit", new Contact
            {
                ContactId = 1,
                Email     = "sealabjaster@gmail.com",
                Name      = "Brandy Chaser",
                Phone     = "666",
                Type      = ContactType.BusinessEntity
            });

            response = await this.Client.GetEnsureStatusAsync("/Contact/Edit?id=1", HttpStatusCode.OK);
            content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Brandy Chaser",                    content);
            Assert.Contains("sealabjaster@gmail.com",           content);
            Assert.Contains("666",                              content);
            Assert.Contains(nameof(ContactType.BusinessEntity), content);
        }
    }
}
