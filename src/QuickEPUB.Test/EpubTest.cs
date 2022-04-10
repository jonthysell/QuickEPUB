// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QuickEPUB;

namespace QuickEPUB.Test
{
    [TestClass]
    public class EpubTest
    {
        [TestMethod]
        public void EpubTest_CreateEpub()
        {
            Epub doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p>This is section 1.</p>");

            using FileStream fs = new FileStream("EpubTest_CreateEpub.epub", FileMode.Create);
            doc.Export(fs);
        }

        [TestMethod]
        public void EpubTest_CreateEpubWithResource()
        {
            Epub doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p><img src=\"test.png\" alt=\"test\"/></p>");
            doc.AddResource("test.png", EpubResourceType.PNG, new FileStream("TestAssets\\test.png", FileMode.Open));

            using FileStream fs = new FileStream("EpubTest_CreateEpubWithResource.epub", FileMode.Create);
            doc.Export(fs);
        }
    }
}
