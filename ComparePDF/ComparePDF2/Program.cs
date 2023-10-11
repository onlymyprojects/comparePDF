using Aspose.Words;

var docA = new Document("C:\\work\\CLDB files\\testComparePdf\\comparePDFile1.pdf");
var docB = new Document("C:\\work\\CLDB files\\testComparePdf\\comparePDFile2.pdf");

// There should be no revisions before comparison.
docA.AcceptAllRevisions();
docB.AcceptAllRevisions();

docB.Compare(docA, "Author Name", DateTime.Now);
docB.Save("C:\\work\\CLDB files\\testComparePdf\\Output2.pdf");