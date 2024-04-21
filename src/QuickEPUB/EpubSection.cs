// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace QuickEPUB
{
    /// <summary>
    /// A section of HTML content within an EPUB.
    /// </summary>
    public class EpubSection
    {
        /// <summary>
        /// The title of this <see cref="EpubSection"/>.
        /// </summary>
        public string Title
        {
            get =>_title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException();
                _title = value;
            }
        }
        private string _title;

        /// <summary>
        /// The HTML content of this <see cref="EpubSection"/>, to be placed within the &lt;body&gt; tag.
        /// </summary>
        public string BodyHtml
        {
            get =>_body;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException();
                _body = value;
            }
        }
        private string _body;

        /// <summary>
        /// Gets a value that indicates whether or not this <see cref="EpubSection"/> has a CSS file.
        /// </summary>
        public bool HasCss =>!string.IsNullOrWhiteSpace(CssPath);

        /// <summary>
        /// The relative path to the CSS file (if any) for this <see cref="EpubSection"/>.
        /// </summary>
        public string CssPath
        {
            get => _cssPath;
            set => _cssPath = value?.Trim() ?? string.Empty;
        }
        private string _cssPath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpubSection"/> class.
        /// </summary>
        /// <param name="title">The title of this <see cref="EpubSection"/>.</param>
        /// <param name="bodyHtml">The HTML content of this <see cref="EpubSection"/>, to be placed within the &lt;body&gt; tag.</param>
        /// <param name="cssPath">The relative path to the CSS file (if any) for this <see cref="EpubSection"/>.</param>
        public EpubSection(string title, string bodyHtml, string cssPath = "")
        {
            Title = title;
            BodyHtml = bodyHtml;
            CssPath = cssPath;
        }
    }
}
