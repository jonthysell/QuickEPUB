// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace QuickEPUB
{
    public class EpubSection
    {
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

        public string BodyHtml
        {
            get
            {
                return _body;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException();
                }
                _body = value;
            }
        }
        private string _body;

        public bool HasCss
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CssPath);
            }
        }

        public string CssPath
        {
            get
            {
                return _cssPath;
            }
            set
            {
                _cssPath = value?.Trim() ?? "";
            }
        }
        private string _cssPath = "";

        public EpubSection(string title, string bodyHtml, string cssPath = "")
        {
            Title = title;
            BodyHtml = bodyHtml;
            CssPath = cssPath;
        }
    }
}
