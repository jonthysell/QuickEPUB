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
            using FileStream fs = new FileStream("TestAssets\\test.css", FileMode.Open);

            EpubResource resource = new EpubResource("test.css", EpubResourceType.CSS, fs);

            Assert.AreEqual("test.css", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.CSS, resource.ResourceType);
            Assert.AreEqual("text/css", resource.MediaType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        public void EpubResource_NewImageTest()
        {
            using FileStream fs = new FileStream("TestAssets\\test.png", FileMode.Open);

            EpubResource resource = new EpubResource("test.png", EpubResourceType.PNG, fs);

            Assert.AreEqual("test.png", resource.OutputPath);
            Assert.AreEqual(EpubResourceType.PNG, resource.ResourceType);
            Assert.AreEqual("image/png", resource.MediaType);
            Assert.IsNotNull(resource.ResourceData);
            Assert.IsTrue(resource.ResourceData.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewNullOutputPathTest()
        {
            using FileStream fs = new FileStream("TestAssets\\test.css", FileMode.Open);
            new EpubResource(null, EpubResourceType.CSS, fs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewEmptyOutputPathTest()
        {
            using FileStream fs = new FileStream("TestAssets\\test.css", FileMode.Open);

            new EpubResource("", EpubResourceType.CSS, fs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewWhitespaceOutputPathTest()
        {
            using FileStream fs = new FileStream("TestAssets\\test.css", FileMode.Open);

            new EpubResource("", EpubResourceType.CSS, fs);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EpubResource_NewNullInputStreamTest()
        {
            new EpubResource("test.css", EpubResourceType.CSS, null);
        }
    }
}
