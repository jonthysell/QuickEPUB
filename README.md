# QuickEPUB #

QuickEPUB is an open-source .NET Portable Class Library for generating simple ebooks in the EPUB format.

EPUB is a very powerful and flexible format for publishing e-books. Most open-source libraries for creating EPUBS are just as powerful and flexible, but therefore require you to really understand how EPUBs work in order to use them.

QuickEPUB is for developers that want a quick and easy way to export simple EPUB files from their apps.

## Sample Usage ##

`
Epub doc = new Epub("Test Title", "Test Author");
doc.AddSection("Chapter 1", "<p>This is chapter 1.</p>");
doc.AddSection("Chapter 2", "<p>This is chapter 2.</p>");

using (FileStream fs = new FileStream("test.epub", FileMode.Create))
{
    doc.Export(fs);
}
`
