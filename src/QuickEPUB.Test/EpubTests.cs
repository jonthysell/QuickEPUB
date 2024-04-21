// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickEPUB.Test
{
    [TestClass]
    public class EpubTests
    {
        [TestMethod]
        public void Epub_NewTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            Assert.AreEqual("Test Title", doc.Title);
            Assert.AreEqual("Test Author", doc.Author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Epub_NewNullTitleTest()
        {
            new Epub(null, "Test Author");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Epub_NewEmptyTitleTest()
        {
            new Epub("", "Test Author");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Epub_NewWhitespaceTitleTest()
        {
            new Epub(" ", "Test Author");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Epub_NewNullAuthorTest()
        {
            new Epub("Test Title", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Epub_NewEmptyAuthorTest()
        {
            new Epub("Test Title", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Epub_NewWhitespaceAuthorTest()
        {
            new Epub("Test Title", " ");
        }

        [TestMethod]
        public void Epub_AddSectionTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            Assert.IsNotNull(doc.Sections);
            Assert.AreEqual(0, doc.Sections.Count());

            doc.AddSection("Section 1", "<p>This is section 1.</p>");
            Assert.AreEqual(1, doc.Sections.Count());

            EpubSection section = doc.Sections.First();
            Assert.AreEqual("Section 1", section.Title);
            Assert.AreEqual("<p>This is section 1.</p>", section.BodyHtml);
            Assert.IsFalse(section.HasCss);
            Assert.IsTrue(string.IsNullOrWhiteSpace(section.CssPath));
        }

        [TestMethod]
        public void Epub_AddCSSResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            Assert.IsNotNull(doc.Resources);
            Assert.AreEqual(0, doc.Resources.Count());

            using var fs = new FileStream("TestAssets\\test.css", FileMode.Open);

            doc.AddResource("test.css", EpubResourceType.CSS, fs);
            Assert.AreEqual(1, doc.Resources.Count());

            var resource = doc.Resources.First();
            Assert.AreEqual("test.css", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.CSS, resource.ResourceType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        public void Epub_AddImageResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            Assert.IsNotNull(doc.Resources);
            Assert.AreEqual(0, doc.Resources.Count());

            using var fs = new FileStream("TestAssets\\test.png", FileMode.Open);

            doc.AddResource("test.png", EpubResourceType.PNG, fs);
            Assert.AreEqual(1, doc.Resources.Count());

            var resource = doc.Resources.First();
            Assert.AreEqual("test.png", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.PNG, resource.ResourceType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        public void Epub_AddFontResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            Assert.IsNotNull(doc.Resources);
            Assert.AreEqual(0, doc.Resources.Count());

            using var fs = new FileStream("TestAssets\\test.ttf", FileMode.Open);

            doc.AddResource("test.ttf", EpubResourceType.TTF, fs);
            Assert.AreEqual(1, doc.Resources.Count());

            var resource = doc.Resources.First();
            Assert.AreEqual("test.ttf", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.TTF, resource.ResourceType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        public void Epub_ExportEpubTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p>This is section 1.</p>");

            using var fs = new FileStream("Epub_ExportEpubTest.epub", FileMode.Create);
            doc.Export(fs);
        }

        [TestMethod]
        public void Epub_ExportEpubWithCSSResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p>This is section 1.</p>", "test.css");

            using var cssStream = new FileStream("TestAssets\\test.css", FileMode.Open);
            doc.AddResource("test.css", EpubResourceType.CSS, cssStream);

            using var fs = new FileStream("Epub_ExportEpubWithCSSResourceTest.epub", FileMode.Create);
            doc.Export(fs);
        }


        [TestMethod]
        public void Epub_ExportEpubWithImageResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p><img src=\"test.png\" alt=\"test\"/></p>");

            using var pngStream = new FileStream("TestAssets\\test.png", FileMode.Open);
            doc.AddResource("test.png", EpubResourceType.PNG, pngStream);

            using var fs = new FileStream("Epub_ExportEpubWithImageResourceTest.epub", FileMode.Create);
            doc.Export(fs);
        }

        [TestMethod]
        public void Epub_ExportEpubWithCoverImageResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p><img src=\"test.png\" alt=\"test\"/></p>");

            using var pngStream = new FileStream("TestAssets\\test.png", FileMode.Open);
            doc.AddResource("test.png", EpubResourceType.PNG, pngStream, true);

            using var fs = new FileStream("Epub_ExportEpubWithCoverImageResourceTest.epub", FileMode.Create);
            doc.Export(fs);
        }

        [TestMethod]
        public void Epub_ExportEpubWithFontResourceTest()
        {
            var doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p style=\"font-family: Test;\">This is section 1.</p>", "test.css");

            using var cssStream = new FileStream("TestAssets\\test.css", FileMode.Open);
            doc.AddResource("test.css", EpubResourceType.CSS, cssStream);

            using var ttfStream = new FileStream("TestAssets\\test.ttf", FileMode.Open);
            doc.AddResource("test.ttf", EpubResourceType.TTF, ttfStream);

            using var fs = new FileStream("Epub_ExportEpubWithFontResourceTest.epub", FileMode.Create);
            doc.Export(fs);
        }
    }
}
