using GroupDocs.Comparison;

namespace ComparePdfFilesUsingCSharp
{
    class Program
    {
        public static void Main(string[] args) // Main function to compare PDF documents using C#
        {
            // Remove the watermark in output PDF document by adding license
            //string licensePath = "GroupDocs.Comparison.lic";
            //GroupDocs.Comparison.License lic = new GroupDocs.Comparison.License();
            //lic.SetLicense(licensePath);

            using (Comparer comparer = new Comparer(@"C:\work\CLDB files\testComparePdf\comparePDFile2.pdf"))
            {
                comparer.Add(@"C:\work\CLDB files\testComparePdf\comparePDFile1.pdf");
                comparer.Compare(@"C:\work\CLDB files\testComparePdf\result.pdf");
            }
            Console.WriteLine("Done");
        }
    }
}