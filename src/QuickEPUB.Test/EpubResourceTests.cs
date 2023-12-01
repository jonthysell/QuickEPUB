// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuickEPUB.Test
{
    [TestClass]
    public class EpubResourceTests
    {
        [TestMethod]
        public void EpubResource_NewCSSTest()
        {
            using var fs = new FileStream("TestAssets\\test.css", FileMode.Open);

            var resource = new EpubResource("test.css", EpubResourceType.CSS, fs);

            Assert.AreEqual("test.css", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.CSS, resource.ResourceType);
            Assert.AreEqual("text/css", resource.MediaType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        public void EpubResource_NewImageTest()
        {
            using var fs = new FileStream("TestAssets\\test.png", FileMode.Open);

            var resource = new EpubResource("test.png", EpubResourceType.PNG, fs);

            Assert.AreEqual("test.png", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.PNG, resource.ResourceType);
            Assert.AreEqual("image/png", resource.MediaType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        public void EpubResource_NewFontTest()
        {
            using var fs = new FileStream("TestAssets\\test.ttf", FileMode.Open);

            var resource = new EpubResource("test.ttf", EpubResourceType.TTF, fs);

            Assert.AreEqual("test.ttf", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.TTF, resource.ResourceType);
            Assert.AreEqual("font/ttf", resource.MediaType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewNullOutputPathTest()
        {
            using var fs = new FileStream("TestAssets\\test.css", FileMode.Open);
            new EpubResource(null, EpubResourceType.CSS, fs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewEmptyOutputPathTest()
        {
            using var fs = new FileStream("TestAssets\\test.css", FileMode.Open);
            new EpubResource(string.Empty, EpubResourceType.CSS, fs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewWhitespaceOutputPathTest()
        {
            using var fs = new FileStream("TestAssets\\test.css", FileMode.Open);
            new EpubResource(string.Empty, EpubResourceType.CSS, fs);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewNullInputStreamTest()
        {
            new EpubResource("test.css", EpubResourceType.CSS, null);
        }
    }
}
