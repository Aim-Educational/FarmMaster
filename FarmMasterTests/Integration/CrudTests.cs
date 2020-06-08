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

        // Breed is reliant on species, and since each test is given their own clean database, we can't transfer data between them.
        // So we're doing both at once.
        [Fact()]
        public async void SpeciesAndBreedTest()
        {
            await this.Client.LoginAsync();
            this.SpeciesTest();
            this.BreedTest();
        }

        private async void SpeciesTest()
        {
            // Create
            await this.Client.CrudCreateAsync("/Species/Create", new Species
            {
                GestrationPeriod = TimeSpan.FromDays(666),
                Name             = "Lalafell" // So we don't conflict with any pre-seeded species.
            });

            // Second one, for later when testing breeds.
            await this.Client.CrudCreateAsync("/Species/Create", new Species
            { 
                GestrationPeriod = TimeSpan.FromDays(69),
                Name             = "Miqo'te"
            });

            // We can't test Index, since it uses JS to dynamically populate the table :p

            // Read(Edit)
            var response = await this.Client.GetEnsureStatusAsync("/Species/Edit?id=1", HttpStatusCode.OK);
            var content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Lalafell", content);
            Assert.Contains("666",      content);

            // Edit & Read
            await this.Client.CrudEditAsync("/Species/Edit", new Species
            {
                SpeciesId        = 1,
                GestrationPeriod = TimeSpan.FromDays(6),
                Name             = "Devilspawn"
            });

            response = await this.Client.GetEnsureStatusAsync("/Species/Edit?id=1", HttpStatusCode.OK);
            content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Devilspawn", content);
            Assert.Contains("6", content);
        }

        private async void BreedTest()
        {
            // Species #1: Devilspawn
            // Species #2: Miqo'te
            // Informally: Same species >:c

            // Create
            await this.Client.CrudCreateAsync("/Breed/Create", new Breed
            {
                SpeciesId = 1,
                Name = "Pocket Rocket"
            });

            // Read(Edit)
            var response = await this.Client.GetEnsureStatusAsync("/Breed/Edit?id=1", HttpStatusCode.OK);
            var content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Pocket Rocket", content);
            Assert.Contains("Devilspawn",    content);

            // Edit & Read
            await this.Client.CrudEditAsync("/Breed/Edit?id=1", new Breed
            {
                SpeciesId = 2,
                Name = "Hyperspace fish bowl mode" // "Don't use inside jokes in your code", says the sensible developer.
            });

            response = await this.Client.GetEnsureStatusAsync("/Breed/Edit?id=1", HttpStatusCode.OK);
            content  = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hyperspace", content);
            Assert.Contains("Miqo'te",    content);
        }
    }
}
