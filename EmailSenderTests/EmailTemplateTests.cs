using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmailSender;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
                template.Resolve(
                    new EmailTemplateValues{ { "username", "SealabJaster" } },
                    null,
                    null
                )
            );

            // NOTE: When no link generator is passed, the URL is relative, not absolute.
            template = new EmailTemplate("{{ @Account#ConfirmEmail?token=tok }}");
            Assert.AreEqual(
                "<a href='/Account/ConfirmEmail?token=andy_loves_goats'>here</a>",
                template.Resolve(
                    new EmailTemplateValues { { "tok", "andy_loves_goats" } },
                    null,
                    null
                )
            );
        }
    }
}