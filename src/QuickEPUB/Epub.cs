// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace QuickEPUB
{
    /// <summary>
    /// An EPUB document.
    /// </summary>
    public class Epub
    {
        /// <summary>
        /// The title of this <see cref="Epub"/> document.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                _title = value;
            }
        }
        private string _title;

        /// <summary>
        /// The author of this <see cref="Epub"/> document.
        /// </summary>
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                _author = value;
            }
        }
        private string _author;

        /// <summary>
        /// The ISO 2-letter language code specifiying the language of the content in this <see cref="Epub"/> document.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The unique identifier (URL, ISBN, etc.) of this <see cref="Epub"/> document.
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// The <see cref="EpubSection"/>s in this <see cref="Epub"/> document.
        /// </summary>
        public IEnumerable<EpubSection> Sections
        {
            get
            {
                return _sections.AsEnumerable();
            }
        }
        private readonly List<EpubSection> _sections = new List<EpubSection>();

        /// <summary>
        /// The <see cref="EpubResource"/>s in this <see cref="Epub"/> document.
        /// </summary>
        public IEnumerable<EpubResource> Resources
        {
            get
            {
                return _resources.AsEnumerable();
            }
        }
        private readonly List<EpubResource> _resources = new List<EpubResource>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Epub"/> class.
        /// </summary>
        /// <param name="title">The title of this <see cref="Epub"/> document.</param>
        /// <param name="author">The author of this <see cref="Epub"/> document.</param>
        public Epub(string title, string author)
        {
            Title = title;
            Author = author;
        }

        /// <summary>
        /// Creates and adds a new <see cref="EpubSection"/> to this <see cref="Epub"/> document.
        /// </summary>
        /// <param name="title">The title of the new <see cref="EpubSection"/>.</param>
        /// <param name="bodyHtml">The HTML content of the new <see cref="EpubSection"/>, to be placed within the &lt;body&gt; tag.</param>
        /// <param name="cssPath">The relative path to the CSS file (if any) for the new <see cref="EpubSection"/>.</param>
        public void AddSection(string title, string bodyHtml, string cssPath = "")
        {
            EpubSection section = new EpubSection(title, bodyHtml, cssPath);
            _sections.Add(section);
        }

        /// <summary>
        /// Creates and adds a new <see cref="EpubResource"/> to this <see cref="Epub"/> document.
        /// </summary>
        /// <param name="path">The relative output path to store the new <see cref="EpubResource"/> file within the EPUB.</param>
        /// <param name="resourceType">The type of the new <see cref="EpubResource"/> file.</param>
        /// <param name="resourceStream">The input stream of the new <see cref="EpubResource"/> file.</param>
        public void AddResource(string path, EpubResourceType resourceType, Stream resourceStream)
        {
            EpubResource resource = new EpubResource(path, resourceType, resourceStream);
            _resources.Add(resource);
        }

        /// <summary>
        /// Exports this <see cref="Epub"/> document as an EPUB file to the given output stream.
        /// </summary>
        /// <param name="outputStream">The output stream to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when the output stream is null.</exception>
        public void Export(Stream outputStream)
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            string lang = string.IsNullOrWhiteSpace(Language) ? CultureInfo.CurrentCulture.Name : Language;
            string uid = string.IsNullOrWhiteSpace(UID) ? Guid.NewGuid().ToString() : UID;

            using (ZipArchive archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
            {
                // Add mimetype
                ZipArchiveEntry mimetype = archive.CreateEntry("mimetype", CompressionLevel.NoCompression);
                using (StreamWriter sw = new StreamWriter(mimetype.Open()))
                {
                    sw.Write(@"application/epub+zip");
                }

                // Add container.xml
                ZipArchiveEntry containerXml = archive.CreateEntry("META-INF/container.xml", CompressionLevel.Optimal);
                using (StreamWriter sw = new StreamWriter(containerXml.Open()))
                {
                    sw.Write(ContainerXmlTemplate);
                }

                // Add content.opf
                ZipArchiveEntry contentOpf = archive.CreateEntry("OEBPS/content.opf", CompressionLevel.Optimal);
                using (StreamWriter sw = new StreamWriter(contentOpf.Open()))
                {
                    StringBuilder itemSB = new StringBuilder();
                    StringBuilder spineSB = new StringBuilder();

                    for (int i = 0; i < _sections.Count; i++)
                    {
                        string sectionId = string.Format("section{0}", i + 1);
                        itemSB.AppendLine(string.Format(ContentOpfItemTemplate
                            , sectionId
                            , sectionId + ".html"
                            , "application/xhtml+xml"));

                        spineSB.AppendLine(string.Format(ContentOpfSpineItemRefTemplate, sectionId));
                    }

                    for (int i = 0; i < _resources.Count; i++)
                    {
                        string resourceId = string.Format("resource{0}", i + 1);
                        itemSB.AppendLine(string.Format(ContentOpfItemTemplate
                            , resourceId
                            , _resources[i].OutputPath
                            , _resources[i].MediaType));
                    }

                    string content = string.Format(ContentOpfTemplate
                        , Title
                        , Author
                        , uid
                        , lang
                        , itemSB.ToString()
                        , spineSB.ToString());

                    sw.Write(content);
                }

                // Add toc.ncx
                ZipArchiveEntry tocNcx = archive.CreateEntry("OEBPS/toc.ncx", CompressionLevel.Optimal);
                using (StreamWriter sw = new StreamWriter(tocNcx.Open()))
                {
                    StringBuilder navLabelSB = new StringBuilder();

                    for (int i = 0; i < _sections.Count; i++)
                    {
                        EpubSection section = _sections[i];

                        string sectionId = string.Format("section{0}", i + 1);
                        navLabelSB.AppendLine(string.Format(TocNcxNavPointTemplate
                            , sectionId
                            , (i + 1).ToString()
                            , section.Title));
                    }

                    string content = string.Format(TocNcxTemplate
                        , uid
                        , Title
                        , navLabelSB.ToString());

                    sw.Write(content);
                }

                // Add Sections
                for (int i = 0; i < _sections.Count; i++)
                {
                    string sectionId = string.Format("section{0}", i + 1);

                    ZipArchiveEntry sectionHtml = archive.CreateEntry(string.Format("OEBPS/{0}.html", sectionId), CompressionLevel.Optimal);
                    using (StreamWriter sw = new StreamWriter(sectionHtml.Open()))
                    {
                        string content = "";

                        if (_sections[i].HasCss)
                        {
                            content = string.Format(EpubSectionHtmlWithCSSTemplate
                                , _sections[i].Title
                                , _sections[i].CssPath
                                , _sections[i].BodyHtml);
                        }
                        else
                        {
                            content = string.Format(EpubSectionHtmlTemplate
                                , _sections[i].Title
                                , _sections[i].BodyHtml);
                        }

                        sw.Write(content);
                    }
                }

                // Add Resources
                for (int i = 0; i < _resources.Count; i++)
                {
                    ZipArchiveEntry resourceEntry = archive.CreateEntry(string.Format("OEBPS/{0}", _resources[i].OutputPath), CompressionLevel.Optimal);
                    using (BinaryWriter bw = new BinaryWriter(resourceEntry.Open()))
                    {
                        bw.Write(_resources[i].ResourceData, 0, _resources[i].ResourceData.Length);
                    }
                }
            }
        }

        private const string ContainerXmlTemplate = @"<?xml version=""1.0""?>
<container version=""1.0"" xmlns=""urn:oasis:names:tc:opendocument:xmlns:container"">
  <rootfiles>
    <rootfile full-path=""OEBPS/content.opf""
     media-type=""application/oebps-package+xml"" />
  </rootfiles>
</container>
";

        private const string ContentOpfTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://www.idpf.org/2007/opf"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" unique-identifier=""bookid"" version=""2.0"">
  <metadata>
    <dc:title>{0}</dc:title>
    <dc:creator>{1}</dc:creator>
    <dc:identifier id=""bookid"">{2}</dc:identifier>
    <dc:language>{3}</dc:language>
  </metadata>
  <manifest>
    <item id=""ncx"" href=""toc.ncx"" media-type=""application/x-dtbncx+xml""/>
    {4}
  </manifest>
  <spine toc=""ncx"">
    {5}
  </spine>
</package>
";

        private const string ContentOpfItemTemplate = @"<item id=""{0}"" href=""{1}"" media-type=""{2}""/>";
        private const string ContentOpfSpineItemRefTemplate = @"<itemref idref=""{0}""/>";

        private const string TocNcxTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE ncx PUBLIC ""-//NISO//DTD ncx 2005-1//EN"" ""http://www.daisy.org/z3986/2005/ncx-2005-1.dtd"">
<ncx xmlns=""http://www.daisy.org/z3986/2005/ncx/"" version=""2005-1"">
  <head>
    <meta name=""dtb:uid"" content=""{0}""/>
    <meta name=""dtb:depth"" content=""1""/>
    <meta name=""dtb:totalPageCount"" content=""0""/>
    <meta name=""dtb:maxPageNumber"" content=""0""/>
  </head>
  <docTitle>
    <text>{1}</text>
  </docTitle>
  <navMap>
    {2}
  </navMap>
</ncx>
";

        private const string TocNcxNavPointTemplate = @"    <navPoint id=""{0}"" playOrder=""{1}"">
      <navLabel>
        <text>{2}</text>
      </navLabel>
      <content src=""{0}.html""/>
    </navPoint>
";

        private const string EpubSectionHtmlTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<title>{0}</title>
</head>
<body>
{1}
</body>
</html>
";

        private const string EpubSectionHtmlWithCSSTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<title>{0}</title>
<link type=""text/css"" rel=""stylesheet"" href=""{1}"" />
</head>
<body>
{2}
</body>
</html>
";
    }
}
