// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO;

namespace QuickEPUB
{
    /// <summary>
    /// A resource file stored within an EPUB.
    /// </summary>
    public class EpubResource
    {
        /// <summary>
        /// The relative output path to store this <see cref="EpubResource"/> file within the EPUB.
        /// </summary>
        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                _outputPath = value;
            }
        }
        private string _outputPath;

        /// <summary>
        /// The type of this <see cref="EpubResource"/> file.
        /// </summary>
        public EpubResourceType ResourceType { get; private set; }

        /// <summary>
        /// The data of this <see cref="EpubResource"/> file.
        /// </summary>
        public byte[] ResourceData
        {
            get
            {
                return _resourceData;
            }
            private set
            {
                _resourceData = value ?? throw new ArgumentNullException();
            }
        }
        private byte[] _resourceData = null;

        /// <summary>
        /// The MIME type of this <see cref="EpubResource"/> file.
        /// </summary>
        public string MediaType
        {
            get
            {
                return MediaTypeMapping[(int)ResourceType];
            }
        }

        /// <summary>
        /// The flag indicating whether to mark this <see cref="EpubResource"/> file as a cover.
        /// </summary>
        public bool IsCover
        {
            get
            {
                return
                    _isCover &&
                    (ResourceType == EpubResourceType.JPEG ||
                     ResourceType == EpubResourceType.GIF ||
                     ResourceType == EpubResourceType.PNG ||
                     ResourceType == EpubResourceType.SVG);
            }
            private set
            {
                _isCover = value;
            }
        }
        private bool _isCover = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpubResource"/> class.
        /// </summary>
        /// <param name="outputPath">The relative output path to store this <see cref="EpubResource"/> file within the EPUB.</param>
        /// <param name="resourceType">The type of this <see cref="EpubResource"/> file.</param>
        /// <param name="inputStream">The input stream of this <see cref="EpubResource"/> file.</param>
        /// <param name="isCover">The flag indicating whether to mark the new <see cref="EpubResource"/> file as a cover.</param>
        /// <exception cref="ArgumentNullException">Thrown when the input stream is null.</exception>
        public EpubResource(string outputPath, EpubResourceType resourceType, Stream inputStream, bool isCover = false)
        {
            OutputPath = outputPath;
            ResourceType = resourceType;

            if (inputStream is null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            using (var ms = new MemoryStream())
            {
                inputStream.CopyTo(ms);
                ResourceData = ms.ToArray();
            }

            IsCover = isCover;
        }

        private static readonly string[] MediaTypeMapping = new string[]
        {
            "text/css",
            "image/jpeg",
            "image/gif",
            "image/png",
            "image/svg+xml",
            "font/ttf",
            "font/otf",
        };
    }

    /// <summary>
    /// Used to determine the type of an <see cref="EpubResource"/> file.
    /// </summary>
    public enum EpubResourceType
    {
        /// <summary>
        /// A CSS text file.
        /// </summary>
        CSS = 0,
        /// <summary>
        /// A JPEG image file.
        /// </summary>
        JPEG,
        /// <summary>
        /// A GIF image file.
        /// </summary>
        GIF,
        /// <summary>
        /// A PNG image file.
        /// </summary>
        PNG,
        /// <summary>
        /// A SVG image file.
        /// </summary>
        SVG,
        /// <summary>
        /// A TTF font file.
        /// </summary>
        TTF,
        /// <summary>
        /// An OTF font file.
        /// </summary>
        OTF,
    }
}
