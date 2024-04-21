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
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException();
                _title = value;
            }
        }
        private string _title;

        /// <summary>
        /// The author of this <see cref="Epub"/> document.
        /// </summary>
        public string Author
        {
            get => _author;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException();
                _author = value;
            }
        }
        private string _author;

        /// <summary>
        /// The ISO 2-letter language code specifying the language of the content in this <see cref="Epub"/> document.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The unique identifier (URL, ISBN, etc.) of this <see cref="Epub"/> document.
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// The title for the Table of Contents page for this <see cref="Epub"/> document.
        /// </summary>
        public string TableOfContentsTitle { get; set; } = "Table of Contents";

        /// <summary>
        /// The <see cref="EpubSection"/>s in this <see cref="Epub"/> document.
        /// </summary>
        public IEnumerable<EpubSection> Sections => _sections.AsEnumerable();
        private readonly List<EpubSection> _sections = new();

        /// <summary>
        /// The <see cref="EpubResource"/>s in this <see cref="Epub"/> document.
        /// </summary>
        public IEnumerable<EpubResource> Resources =>_resources.AsEnumerable();
        private readonly List<EpubResource> _resources = new();

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
            var section = new EpubSection(title, bodyHtml, cssPath);
            _sections.Add(section);
        }

        /// <summary>
        /// Creates and adds a new <see cref="EpubResource"/> to this <see cref="Epub"/> document.
        /// </summary>
        /// <param name="path">The relative output path to store the new <see cref="EpubResource"/> file within the EPUB.</param>
        /// <param name="resourceType">The type of the new <see cref="EpubResource"/> file.</param>
        /// <param name="resourceStream">The input stream of the new <see cref="EpubResource"/> file.</param>
        /// <param name="isCover">The flag indicating whether to mark the new <see cref="EpubResource"/> file as a cover.</param>
        public void AddResource(string path, EpubResourceType resourceType, Stream resourceStream, bool isCover = false)
        {
            var resource = new EpubResource(path, resourceType, resourceStream, isCover);
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
                throw new ArgumentNullException(nameof(outputStream));

            string lang = string.IsNullOrWhiteSpace(Language) ? CultureInfo.CurrentCulture.Name : Language;
            string uid = string.IsNullOrWhiteSpace(UID) ? Guid.NewGuid().ToString() : UID;

            using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create);

            // Add mimetype
            ZipArchiveEntry mimetype = archive.CreateEntry("mimetype", CompressionLevel.NoCompression);
            using (var streamWriter = new StreamWriter(mimetype.Open()))
            {
                streamWriter.Write(@"application/epub+zip");
            }

            // Add container.xml
            ZipArchiveEntry containerXml = archive.CreateEntry("META-INF/container.xml", CompressionLevel.Optimal);
            using (var streamWriter = new StreamWriter(containerXml.Open()))
            {
                streamWriter.Write(ContainerXmlTemplate);
            }

            // Add content.opf
            ZipArchiveEntry contentOpf = archive.CreateEntry("EPUB/content.opf", CompressionLevel.Optimal);
            using (var streamWriter = new StreamWriter(contentOpf.Open()))
            {
                var itemStringBuilder = new StringBuilder();
                var spineStringBuilder = new StringBuilder();

                for (int i = 0; i < _sections.Count; i++)
                {
                    string sectionId = string.Format("section{0}", i + 1);
                    itemStringBuilder.AppendLine(string.Format(
                        ContentOpfItemTemplate,
                        sectionId,
                        $"{sectionId}.html",
                        "application/xhtml+xml"));

                    spineStringBuilder.AppendLine(string.Format(ContentOpfSpineItemRefTemplate, sectionId));
                }

                for (int i = 0; i < _resources.Count; i++)
                {
                    string resourceId = string.Format("resource{0}", i + 1);
                    itemStringBuilder.AppendLine(string.Format(
                        _resources[i].IsCover ? ContentOpfCoverItemTemplate : ContentOpfItemTemplate,
                        resourceId,
                        _resources[i].OutputPath,
                        _resources[i].MediaType));
                }

                string content = string.Format(
                    ContentOpfTemplate,
                    Title,
                    Author,
                    uid,
                    lang,
                    DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                    itemStringBuilder.ToString(),
                    spineStringBuilder.ToString());

                streamWriter.Write(content);
            }

            // Add toc.ncx
            ZipArchiveEntry tocNcx = archive.CreateEntry("EPUB/toc.ncx", CompressionLevel.Optimal);
            using (var streamWriter = new StreamWriter(tocNcx.Open()))
            {
                var navLabelStringBuilder = new StringBuilder();

                for (int i = 0; i < _sections.Count; i++)
                {
                    EpubSection section = _sections[i];

                    string sectionId = string.Format("section{0}", i + 1);
                    navLabelStringBuilder.AppendLine(string.Format(
                        TocNcxNavPointTemplate,
                        sectionId,
                        (i + 1).ToString(),
                        section.Title));
                }

                string content = string.Format(
                    TocNcxTemplate,
                    uid,
                    Title,
                    navLabelStringBuilder.ToString());

                streamWriter.Write(content);
            }

            // Add nav.html
            ZipArchiveEntry navHtml = archive.CreateEntry("EPUB/nav.html", CompressionLevel.Optimal);
            using (var streamWriter = new StreamWriter(navHtml.Open()))
            {
                var navListItemStringBuilder = new StringBuilder();

                for (int i = 0; i < _sections.Count; i++)
                {
                    EpubSection section = _sections[i];

                    string sectionId = string.Format("section{0}", i + 1);
                    navListItemStringBuilder.AppendLine(string.Format(
                        EpubNavHtmlItemTemplate,
                        sectionId,
                        section.Title));
                }

                string content = string.Format(
                    EpubNavHtmlTemplate,
                    TableOfContentsTitle,
                    navListItemStringBuilder.ToString());

                streamWriter.Write(content);
            }

            // Add Sections
            for (int i = 0; i < _sections.Count; i++)
            {
                string sectionId = string.Format("section{0}", i + 1);

                ZipArchiveEntry sectionHtml = archive.CreateEntry(
                    string.Format("EPUB/{0}.html", sectionId),
                    CompressionLevel.Optimal);

                string content = _sections[i].HasCss
                    ? string.Format(
                        EpubSectionHtmlWithCSSTemplate,
                        _sections[i].Title,
                        _sections[i].CssPath,
                        _sections[i].BodyHtml)
                    : string.Format(
                        EpubSectionHtmlTemplate,
                        _sections[i].Title,
                        _sections[i].BodyHtml);

                using var streamWriter = new StreamWriter(sectionHtml.Open());
                streamWriter.Write(content);
            }

            // Add Resources
            for (int i = 0; i < _resources.Count; i++)
            {
                ZipArchiveEntry resourceEntry = archive.CreateEntry(
                    string.Format("EPUB/{0}", _resources[i].OutputPath),
                    CompressionLevel.Optimal);
                using var binaryWriter = new BinaryWriter(resourceEntry.Open());
                binaryWriter.Write(_resources[i].ResourceData, 0, _resources[i].ResourceData.Length);
            }
        }

        private const string ContainerXmlTemplate =
            """
            <?xml version="1.0"?>
            <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
              <rootfiles>
                <rootfile full-path="EPUB/content.opf" media-type="application/oebps-package+xml"/>
              </rootfiles>
            </container>
            """;

        private const string ContentOpfTemplate =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://www.idpf.org/2007/opf" xmlns:dc="http://purl.org/dc/elements/1.1/" unique-identifier="bookid" version="3.0">
              <metadata>
                <dc:title>{0}</dc:title>
                <dc:creator>{1}</dc:creator>
                <dc:identifier id="bookid">{2}</dc:identifier>
                <dc:language>{3}</dc:language>
                <meta property="dcterms:modified">{4}</meta>
              </metadata>
              <manifest>
                <item id="nav" href="nav.html" media-type="application/xhtml+xml" properties="nav" />
                <item id="ncx" href="toc.ncx" media-type="application/x-dtbncx+xml"/>
            {5}  </manifest>
              <spine toc="ncx">
            {6}  </spine>
            </package>
            """;

        private const string ContentOpfItemTemplate =
            """    <item id="{0}" href="{1}" media-type="{2}"/>""";

        private const string ContentOpfCoverItemTemplate =
            """    <item id="{0}" href="{1}" media-type="{2}" properties="cover-image"/>""";

        private const string ContentOpfSpineItemRefTemplate =
            """    <itemref idref="{0}"/>""";

        private const string TocNcxTemplate =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <!DOCTYPE ncx PUBLIC "-//NISO//DTD ncx 2005-1//EN" "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd">
            <ncx xmlns="http://www.daisy.org/z3986/2005/ncx/" version="2005-1">
              <head>
                <meta name="dtb:uid" content="{0}"/>
                <meta name="dtb:depth" content="1"/>
                <meta name="dtb:totalPageCount" content="0"/>
                <meta name="dtb:maxPageNumber" content="0"/>
              </head>
              <docTitle>
                <text>{1}</text>
              </docTitle>
              <navMap>
            {2}  </navMap>
            </ncx>
            """;

        private const string TocNcxNavPointTemplate =
            """
                <navPoint id="{0}" playOrder="{1}">
                  <navLabel>
                    <text>{2}</text>
                  </navLabel>
                  <content src="{0}.html"/>
                </navPoint>
            """;

        private const string EpubNavHtmlTemplate =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <!DOCTYPE html>
            <html xmlns="http://www.w3.org/1999/xhtml" xmlns:epub="http://www.idpf.org/2007/ops">
            <head>
              <title>{0}</title>
            </head>
            <body>
              <nav epub:type="toc">
                <h1>{0}</h1>
                <ol>
                {1}    </ol>
              </nav>
            </body>
            </html>
            """;

        private const string EpubNavHtmlItemTemplate =
            """      <li><a href="{0}.html">{1}</a></li>""";

        private const string EpubSectionHtmlTemplate =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <!DOCTYPE html>
            <html xmlns="http://www.w3.org/1999/xhtml">
            <head>
              <title>{0}</title>
            </head>
            <body>
            {1}
            </body>
            </html>
            """;

        private const string EpubSectionHtmlWithCSSTemplate =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <!DOCTYPE html>
            <html xmlns="http://www.w3.org/1999/xhtml">
            <head>
              <title>{0}</title>
              <link type="text/css" rel="stylesheet" href="{1}"/>
            </head>
            <body>
            {2}
            </body>
            </html>
            """;
    }
}
