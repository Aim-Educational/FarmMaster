using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmailSender;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EmailSender.Tests
{
    [TestClass()]
    public class EmailTemplateTests
    {
        const string EXAMPLE_TEMPLATE = @"Hello {{ username }}";

        [TestMethod()]
        public void EmailTemplateTest()
        {
            var template = new EmailTemplate(EXAMPLE_TEMPLATE);
            Assert.AreEqual(1, template.ValueKeys.Count());
            Assert.AreEqual("username", template.ValueKeys.First());
        }

        [TestMethod()]
        public void AreAllValuesDefinedTest()
        {
            var template = new EmailTemplate(EXAMPLE_TEMPLATE);

            Assert.IsTrue(template.AreAllValuesDefined(new EmailTemplateValues{ { "username", "SealabJaster" } }));
            Assert.IsFalse(template.AreAllValuesDefined(new EmailTemplateValues()));
        }

        [TestMethod()]
        public void ResolveTest()
        {
            var template = new EmailTemplate(EXAMPLE_TEMPLATE);
            Assert.AreEqual(
                "Hello SealabJaster",
                template.Resolve(new EmailTemplateValues{ { "username", "SealabJaster" } })
            );
        }
    }
}