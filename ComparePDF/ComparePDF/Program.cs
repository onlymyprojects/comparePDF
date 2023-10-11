using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

var ExtractedPDFToString1 = ExtractTextFromPdf(@"C:\\work\\CLDB files\\testComparePdf\\comparePDFile1.pdf");
var ExtractedPDFToString2 = ExtractTextFromPdf(@"C:\\work\\CLDB files\\testComparePdf\\comparePDFile2.pdf");

IEnumerable<string> onlyA = ExtractedPDFToString1.Except(ExtractedPDFToString2).ToList();
IEnumerable<string> onlyB = ExtractedPDFToString2.Except(ExtractedPDFToString1).ToList();

foreach (var lin in onlyA)
{
    Console.WriteLine("\nBeging");
    Console.WriteLine(lin);
    Console.WriteLine("End");
}

Console.WriteLine("\n**************************\n");

foreach (var lin in onlyB)
{
    Console.WriteLine("\nBeging");
    Console.WriteLine(lin);
    Console.WriteLine("End");
}

static string[] ExtractTextFromPdf(string path)
{
    using (PdfReader reader = new PdfReader(path))
    {
        StringBuilder text = new StringBuilder();

        for (int i = 1; i <= reader.NumberOfPages; i++)
        {
            text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
        }

        return text.ToString().Split(new[] { '\r', '\n' });
    }
}