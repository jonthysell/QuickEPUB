// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
                if (string.IsNullOrWhiteSpace(value))
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
                _resourceStream = value ?? throw new ArgumentNullException();
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

        private static readonly string[] MediaTypeMapping = new string[]
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
