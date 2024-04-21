// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickEPUB.Test
{
    [TestClass]
    public class EpubSectionTests
    {
        [TestMethod]
        public void EpubSection_NewTest()
        {
            var section = new EpubSection("Test Title", "<p>Test Contents</p>");
            Assert.AreEqual("Test Title", section.Title);
            Assert.AreEqual("<p>Test Contents</p>", section.BodyHtml);
            Assert.IsFalse(section.HasCss);
            Assert.IsTrue(string.IsNullOrWhiteSpace(section.CssPath));
        }

        [TestMethod]
        public void EpubSection_NewWithCSSTest()
        {
            var section = new EpubSection("Test Title", "<p>Test Contents</p>", "test.css");
            Assert.AreEqual("Test Title", section.Title);
            Assert.AreEqual("<p>Test Contents</p>", section.BodyHtml);
            Assert.IsTrue(section.HasCss);
            Assert.AreEqual("test.css", section.CssPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubSection_NewNullTitleTest()
        {
            new EpubSection(null, "<p>Test Contents</p>");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubSection_NewEmptyTitleTest()
        {
            new EpubSection("", "<p>Test Contents</p>");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubSection_NewWhitespaceTitleTest()
        {
            new EpubSection(" ", "<p>Test Contents</p>");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubSection_NewNullBodyHtmlTest()
        {
            new EpubSection("Test Title", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubSection_NewEmptyBodyHtmlTest()
        {
            new EpubSection("Test Title", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubSection_NewWhitespaceBodyHtmlTest()
        {
            new EpubSection("Test Title", " ");
        }
    }
}
