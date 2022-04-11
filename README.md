# QuickEPUB #

[![CI Build](https://github.com/jonthysell/QuickEPUB/actions/workflows/ci.yml/badge.svg)](https://github.com/jonthysell/QuickEPUB/actions/workflows/ci.yml)

QuickEPUB is an open-source .NET library for generating simple eBooks in the EPUB format.

EPUB is a very powerful and flexible format for publishing eBooks. Most open-source libraries for creating EPUBs are just as powerful and flexible, but often require you to really understand how EPUBs work in order to use them effectively.

QuickEPUB is for developers that want a quick and easy way to take HTML content and export simple EPUB files from their apps.

## Usage ##

1. Create an `Epub` instance, specifying the title and author of the book
2. (Optional) Specify a language and/or unique identifier (ISBN, URL, whatever)
3. Add sections of HTML content, each of which will get an entry in the table of contents
4. (Optional) Add any CSS/image resources that are referenced in the HTML
5. Export the instance to a file

### Sample Code ###

```cs
// Create an Epub instance
Epub doc = new Epub("Book Title", "Author Name");

// Adding sections of HTML content
doc.AddSection("Chapter 1", "<p>Lorem ipsum dolor sit amet...</p>");

// Adding sections of HTML content (that reference image files)
doc.AddSection("Chapter 2", "<p><img src=\"image.jpg\" alt=\"Image\"/></p>");

// Adding images that are referenced in any of the sections
doc.AddResource("image.jpg", EpubResourceType.JPEG, new FileStream("image.jpg", FileMode.Open));

// Adding sections of HTML content (that use a custom CSS stylesheet)
doc.AddSection("Chapter 3", "<p class="body-text">Lorem ipsum dolor sit amet...</p>", "custom.css");

// Add any resources referenced in the HTML content
doc.AddResource("custom.css", EpubResourceType.CSS, new FileStream("custom.css", FileMode.Open));

// Export the result
using (FileStream fs = new FileStream("sample.epub", FileMode.Create))
{
    doc.Export(fs);
}
```

The end result will be an EPUB named "sample.epub" with three sections in its table of contents:

1. Chapter 1
2. Chapter 2
3. Chapter 3

The EPUB will also contain the two specified resources: "image.jpg"" and "custom.css".

## Build ##

Building this project requires a PC with the [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) installed.

Then you should be able to run the following command to build QuickEPUB from its source folder:

```cmd
dotnet build ./src/QuickEPUB.sln
```

## Errata ##

QuickEPUB is open-source under the MIT license.

Copyright (c) 2016-2022 Jon Thysell.
