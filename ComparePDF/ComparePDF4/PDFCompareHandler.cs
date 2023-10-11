using DiffMatchPatch;
using Patagames.Pdf.Enums;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using System.Text;
using iText.Commons.Datastructures;
using iText.Kernel.Geom;

namespace ComparePDF4;

public class PDFCompareHandler
{
    public string ReadFile(string pdfPath)
    {
        PdfCommon.Initialize();

        //Open and load a PDF document from a file.
        using (var doc = PdfDocument.Load(pdfPath)) // C# Read PDF File
        {
            foreach (var page in doc.Pages)
            {
                //Gets number of characters in a page or -1 for error.
                //Generated characters, like additional space characters, new line characters, are also counted.
                int totalCharCount = page.Text.CountChars;

                //Extract text from page to the string
                string text = page.Text.GetText(0, totalCharCount);

                page.Dispose();

                return text;
            }
        }

        return "";
    }

    public string ComparePdfFiles(string pdfPath1, string pdfPath2)
    {
        StringBuilder compareResult = new StringBuilder();
        var text1 = ReadFile(pdfPath1);
        var text2 = ReadFile(pdfPath2);
        diff_match_patch dmp = new diff_match_patch();
        var diff = dmp.diff_main(text1, text2);
        dmp.diff_cleanupSemantic(diff);


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
        // The PDF coordinate system origin is at the bottom left corner of the page. 
        // The X-axis is pointing to the right. The Y-axis is pointing in upward direction.
        // The sizes and coordinates in this method are given in the inches.

        // Step 1: Initialize PDF library and create empty document
        // Return value: PdfDocument main class
        PdfCommon.Initialize();
        var doc = PdfDocument.CreateNew();  // Create a PDF document

        // Step 2: Add new page
        // Arguments: page width: 8.27", page height: 11.69", Unit of measure: inches
        //  The PDF unit of measure is point. There are 72 points in one inch.
        var page = doc.Pages.InsertPageAt(doc.Pages.Count, 8.27f * 72, 11.69f * 72);

        // Step 3: Add graphics and text contents to the page
        // Insert image from file using standart System.Drawing.Bitmap class
        //using (PdfBitmap logo = PdfBitmap.FromFile(@"e:\63\logo_square.png"))
        //{
        //    PdfImageObject imageObject = PdfImageObject.Create(doc, logo, 0, 0);
        //    //image resolution is 300 DPI and location is 1.69 x 10.0 inches.
        //    imageObject.Matrix = new FS_MATRIX(logo.Width * 72 / 300, 0, 0, logo.Height * 72 / 300, 1.69 * 72, 10.0 * 72);
        //    page.PageObjects.Add(imageObject);
        //}

        // Create fonts used for text objects
        PdfFont calibryBold = PdfFont.CreateFont(doc, "CalibriBold");
        // Insert text objects at 7.69"; 11.02" and font size is 25
        foreach (var paragraph in paragraphs)
        {
            PdfTextObject textObject = PdfTextObject.Create("lalala", 1.69f * 72, 11.02f * 72, calibryBold, 25);
            textObject.FillColor = FS_COLOR.Black;
            page.PageObjects.Add(textObject);
        }

        // Step 5: Generate page content and save pdf file
        // argument: PDF file name
        page.GenerateContent();
        doc.Save(destination, SaveFlags.NoIncremental);
    }

    public void CreateDocument(string[] paragraphs)
    {
        var doc = PdfDocument.CreateNew();
        float pageWidth = 8.27f;
        float pageHeight = 11.69f;
        float margin = 0.5f; // Left and right margins
        var page = AddNewPage(doc, pageWidth, pageHeight);

        float currentY = pageHeight - margin; // Initial Y coordinate

        foreach (var paragraph in paragraphs)
        {
            // Calculate the height of the text to be added
            PdfTextObject textObject = PdfTextObject.Create(paragraph, 0.51f * 72, currentY * 72, CreateCalibry(doc), 11.04f);
            float textHeight = textObject.BoundingBox.Height / 72;

            if (currentY - textHeight < margin)
            {
                // If there's not enough space for a new paragraph, create a new page
                page = AddNewPage(doc, pageWidth, pageHeight);
                currentY = pageHeight - margin;
            }

            InsertText(page, paragraph, 0.51f, currentY, CreateCalibry(doc), FS_COLOR.Black, 11.04f);
            currentY -= textHeight;
        }

        page.GenerateContent();
        doc.Save(@"C:\work\CLDB files\testComparePdf\comparePDFium.pdf", SaveFlags.NoIncremental);
    }


    private List<Line> SplitTextIntoLines(string text, PdfFont font, FS_COLOR color, float fontSize, float availableWidth)
    {
        List<Line> lines = new List<Line>();
        string remainingText = text;

        while (!string.IsNullOrEmpty(remainingText))
        {
            int maxCharacters = (int)(availableWidth / (fontSize / 2.0f));
            string lineText = remainingText.Length <= maxCharacters
                ? remainingText
                : remainingText.Substring(0, maxCharacters);

            lines.Add(new Line
            {
                Text = lineText,
                Height = fontSize,
                FontSize = fontSize
            });

            remainingText = remainingText.Remove(0, lineText.Length);
        }

        return lines;
    }



    private PdfPage AddNewPage(PdfDocument doc, float width, float height)
    {
        //PDF point is a 1/72 enches
        doc.Pages.InsertPageAt(doc.Pages.Count, width * 72, height * 72);
        return doc.Pages[doc.Pages.Count - 1];
    }

    private void InsertText(PdfPage page, string text, float x, float y, PdfFont font, FS_COLOR FS_COLOR, float fontSize = 11.04f, bool isAlignRight = false)
    {
        PdfTextObject textObject = PdfTextObject.Create(text, x * 72, y * 72, font, fontSize);
        textObject.FillColor = FS_COLOR;
        page.PageObjects.Add(textObject);
        if (isAlignRight)
        {
            float w = textObject.BoundingBox.Width;
            textObject.Location = new FS_POINTF(textObject.Location.X - w, textObject.Location.Y);

        }
    }

    private void InsertRect(PdfPage page, float x1, float y1, float x2, float y2, FS_COLOR fillFS_COLOR)
    {
        PdfPathObject pathObject = PdfPathObject.Create(FillModes.Alternate, false);
        pathObject.FillColor = fillFS_COLOR;
        pathObject.Path.Add(new FS_PATHPOINTF(x1 * 72, y1 * 72, PathPointFlags.MoveTo));
        pathObject.Path.Add(new FS_PATHPOINTF(x2 * 72, y1 * 72, PathPointFlags.LineTo));
        pathObject.Path.Add(new FS_PATHPOINTF(x2 * 72, y2 * 72, PathPointFlags.LineTo));
        pathObject.Path.Add(new FS_PATHPOINTF(x1 * 72, y2 * 72, PathPointFlags.LineTo));
        pathObject.Path.Add(new FS_PATHPOINTF(x1 * 72, y1 * 72, PathPointFlags.CloseFigure | PathPointFlags.LineTo));
        page.PageObjects.Add(pathObject);
    }

    private PdfFont CreateCalibryBold(PdfDocument doc)
    {
        return PdfFont.CreateFont(doc, "Calibri", FontCharSet.DEFAULT_CHARSET, true);
    }

    private PdfFont CreateCalibryItalic(PdfDocument doc)
    {
        return PdfFont.CreateFont(doc, "Calibri", FontCharSet.DEFAULT_CHARSET, false, true);
    }

    private PdfFont CreateCalibry(PdfDocument doc)
    {
        return PdfFont.CreateFont(doc, "Calibri");
    }
}

public class Line
{
    public string Text { get; set; }
    public float Height { get; set; }
    public float FontSize { get; set; }
}
