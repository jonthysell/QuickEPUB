// 
// EpubTest.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2016 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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

            using (FileStream fs = new FileStream("EpubTest_CreateEpub.epub", FileMode.Create))
            {
                doc.Export(fs);
            }
        }

        [TestMethod]
        public void EpubTest_CreateEpubWithResource()
        {
            Epub doc = new Epub("Test Title", "Test Author");
            doc.AddSection("Section 1", "<p><img src=\"test.png\" alt=\"test\"/></p>");
            doc.AddResource("test.png", EpubResourceType.PNG, new FileStream("test.png", FileMode.Open));

            using (FileStream fs = new FileStream("EpubTest_CreateEpubWithResource.epub", FileMode.Create))
            {
                doc.Export(fs);
            }
        }
    }
}
