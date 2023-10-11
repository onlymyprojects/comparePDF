using DiffMatchPatch;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System.Text;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;

namespace ComparePDF4;

public class PdfFileHandler
{
    public string ReadFile(string pdfPath)
    {
        var pageText = new StringBuilder();
        using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath)))
        {
            var pageNumbers = pdfDocument.GetNumberOfPages();
            for (int i = 1; i <= pageNumbers; i++)
            {
                LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                parser.ProcessPageContent(pdfDocument.GetPage(i));
                pageText.Append(strategy.GetResultantText());
            }
        }
        return pageText.ToString();
    }

    public string ComparePdfFiles()
    {
        string pdfPath1 = @"C:\work\CLDB files\testComparePdf\comparePDFile1.pdf";
        string pdfPath2 = @"C:\work\CLDB files\testComparePdf\comparePDFile2.pdf";
        StringBuilder compareResult = new StringBuilder();
        var text1 = ReadFile(pdfPath1);
        var text2 = ReadFile(pdfPath2);
        diff_match_patch dmp = new diff_match_patch();
        var diff = dmp.diff_main(text1, text2);
        dmp.diff_cleanupSemantic(diff);


        FileInfo file = new FileInfo(@"C:\work\CLDB files\testComparePdf\compareeeeeeee.pdf");
        file.Delete();
        var fileStream = file.Create();
        fileStream.Close();
        PdfDocument pdfdoc = new PdfDocument(new PdfWriter(file));
        pdfdoc.SetTagged();

        using (Document document = new Document(pdfdoc))
        {
            Paragraph combinedParagraph = new Paragraph();

            for (int i = 0; i < diff.Count; i++)
            {
                Text text = new Text(diff[i].text);
                text.SetFontSize(11f);

                // Colorize by operation type
                switch (diff[i].operation)
                {
                    case Operation.DELETE:
                        combinedParagraph.Add(text.SetFontColor(DeviceRgb.RED));
                        break;
                    case Operation.INSERT:
                        combinedParagraph.Add(text.SetFontColor(DeviceRgb.GREEN));
                        break;
                    case Operation.EQUAL:
                        combinedParagraph.Add(text.SetFontColor(DeviceRgb.BLACK));
                        break;
                    default:
                        break;
                }
            }
            document.Add(combinedParagraph);
            document.Close();
        }

        for (int i = 0; i < diff.Count; i++)
        {
            // Colorize by operation type
            switch (diff[i].operation)
            {
                case Operation.DELETE:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Operation.INSERT:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Operation.EQUAL:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
            Console.Write(diff[i].text);
        }
        Console.ForegroundColor = ConsoleColor.White;


        foreach (var d in diff)
        {
            compareResult.Append(d + "\n");
        }
        return compareResult.ToString();
    }

    public void GeneratePdf(string[] paragraphs, string destination)
    {
        FileInfo file = new FileInfo(destination);
        file.Delete();
        var fileStream = file.Create();
        fileStream.Close();
        PdfDocument pdfdoc = new PdfDocument(new PdfWriter(file));
        pdfdoc.SetTagged();
        using (Document document = new Document(pdfdoc))
        {
            foreach (var para in paragraphs)
            {
                document.Add(new Paragraph(para));
            }
            document.Close();
        }
    }
}
