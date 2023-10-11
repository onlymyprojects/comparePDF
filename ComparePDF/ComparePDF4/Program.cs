using ComparePDF4;
using Patagames.Pdf.Enums;

PdfFileHandler reader = new PdfFileHandler();  //Name of wrapper class for all methods shown above  
var result = reader.ComparePdfFiles().Split("\n");
reader.GeneratePdf(result, @"C:\work\CLDB files\testComparePdf\compare.pdf");
Console.WriteLine("Done..");
Console.ReadKey(true);

//PDFCompareHandler reader = new PDFCompareHandler();
//var result = reader.ComparePdfFiles(@"C:\work\CLDB files\testComparePdf\comparePDFile1.pdf", @"C:\work\CLDB files\testComparePdf\comparePDFile2.pdf").Split("\n");
//reader.GeneratePdf(result, @"C:\work\CLDB files\testComparePdf\comparePDFium.pdf");

////reader.CreateDocument(result);
//var paragraphs = new string[]
//{
//    "This is the first paragraph.",
//    "This is the second paragraph, and it's a bit longer. It will be split into multiple lines if needed to fit on the page aaaaaaaaaaa aaaaaaaaaaa aaaaaaaaaaaa aaaaaaaaaa aaaaaaaaaaa aaaaaaaaaaaaa aaaaaaaaaa aaa aaa aaaaaaa.",
//    "This is the third paragraph.",
//    // Add more paragraphs as needed
//};

//reader.CreateDocument(paragraphs);


//Console.WriteLine("Done..");
//Console.ReadKey(true);