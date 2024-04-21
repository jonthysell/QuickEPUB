# QuickEPUB #

![License](https://img.shields.io/github/license/jonthysell/QuickEPUB.svg) [![NuGet](https://img.shields.io/nuget/v/QuickEPUB.svg)](https://www.nuget.org/packages/QuickEPUB) [![CI Build](https://github.com/jonthysell/QuickEPUB/actions/workflows/ci.yml/badge.svg)](https://github.com/jonthysell/QuickEPUB/actions/workflows/ci.yml)

QuickEPUB is an open-source .NET library for generating simple eBooks in the EPUB format.

EPUB is a very powerful and flexible format for publishing eBooks. Most open-source libraries for creating EPUBs are just as powerful and flexible, but often require you to really understand how EPUBs work in order to use them effectively.

QuickEPUB is for developers that want a quick and easy way to take HTML content and export simple EPUB files from their apps.

## Install ##

QuickEPUB is published on NuGet Gallery: https://www.nuget.org/packages/QuickEPUB

Use this command in the NuGet Package Manager console to install QuickEPUB manually:

```ps
Install-Package QuickEPUB
```

## Usage ##

Using QuickEPUB is as easy as:

1. Create an `Epub` instance, specifying the title and author of the book
2. (Optional) Specify a language and/or unique identifier (ISBN, URL, whatever)
3. Add sections of HTML content, each of which will get an entry in the table of contents
4. (Optional) Add any CSS/image resources that are referenced in the HTML
5. Export the instance to a file

### Sample Code ###

```cs
// Create an Epub instance
var doc = new Epub("Book Title", "Author Name");

// Adding sections of HTML content
doc.AddSection("Chapter 1", "<p>Lorem ipsum dolor sit amet...</p>");

// Adding sections of HTML content (that reference image files)
doc.AddSection("Chapter 2", "<p><img src=\"image.jpg\" alt=\"Image\"/></p>");

// Adding images that are referenced in any of the sections
using (var jpgStream = new FileStream("image.jpg", FileMode.Open))
{
    doc.AddResource("image.jpg", EpubResourceType.JPEG, jpgStream);
}

// Adding sections of HTML content (that use a custom CSS stylesheet)
doc.AddSection("Chapter 3", "<p class=\"body-text\">Lorem ipsum dolor sit amet...</p>", "custom.css");

// Add the CSS file referenced in the HTML content
using (var cssStream = new FileStream("custom.css", FileMode.Open))
{
    doc.AddResource("custom.css", EpubResourceType.CSS, cssStream);
}

// Export the result
using (var fs = new FileStream("sample.epub", FileMode.Create))
{
    doc.Export(fs);
}
```

This sample code will create an EPUB named `sample.epub` with three sections in its table of contents:

1. Chapter 1
2. Chapter 2
3. Chapter 3

The EPUB will also contain the two specified resource files: `image.jpg` and `custom.css`.

## Build ##

Building QuickEPUB requires:

1. A PC with the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
2. The [QuickEPUB source](https://github.com/jonthysell/QuickEPUB)

Then you should be able to run the following command to build QuickEPUB from within its source folder:

```cmd
dotnet build ./src/QuickEPUB.sln
```

## Test ##

With the above setup, you should be able to run the following command to test QuickEPUB from within its source folder:

```cmd
dotnet test ./src/QuickEPUB.sln
```

## Errata ##

QuickEPUB is open-source under the MIT license.

Copyright (c) 2016-2023 Jon Thysell.
