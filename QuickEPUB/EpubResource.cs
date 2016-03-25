// 
// EpubResource.cs
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

namespace QuickEPUB
{
    public class EpubResource
    {
        public string Path
        {
            get
            {
                return _path;
            }
            private set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                _path = value;
            }
        }
        private string _path;

        public EpubResourceType ResourceType { get; private set; }

        public Stream ResourceStream
        {
            get
            {
                return _resourceStream;
            }
            private set
            {
                if (null == value)
                {
                    throw new ArgumentNullException();
                }
                _resourceStream = value;
            }
        }
        private Stream _resourceStream;

        public string MediaType
        {
            get
            {
                return MediaTypeMapping[(int)ResourceType];
            }
        }

        public EpubResource(string path, EpubResourceType resourceType, Stream resourceStream)
        {
            Path = path;
            ResourceType = resourceType;
            ResourceStream = resourceStream;
        }

        private static string[] MediaTypeMapping = new string[]
        {
            "text/css",
            "image/jpeg",
            "image/gif",
            "image/png",
            "image/svg+xml",
        };
    }

    public enum EpubResourceType
    {
        CSS = 0,
        JPEG,
        GIF,
        PNG,
        SVG
    }
}
