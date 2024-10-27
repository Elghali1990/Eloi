namespace mef.db.plf.edition.generation.License
{
    public static class LicenseAspose
    {

        public static void setLicenseAsposeWords(string path)
        {
            Aspose.Words.License licWord = new Aspose.Words.License();
            licWord.SetLicense(path);

        }

        public static void setLicenseAsposeExcel(string path)
        {
            Aspose.Cells.License licCell = new Aspose.Cells.License();
            licCell.SetLicense(path);

        }

        public static void setLicenseAsposeBarCode(string path)
        {
            Aspose.BarCode.License licBarCode = new Aspose.BarCode.License();
            licBarCode.SetLicense(path);

        }

        public static void setLicenseAsposePDF(string path)
        {
            Aspose.Pdf.License licPDF = new Aspose.Pdf.License();
            licPDF.SetLicense(path);

        }

        public static void setLicenseAsposeTotal(string path)
        {
            setLicenseAsposeWords(path);
            setLicenseAsposeExcel(path);
            setLicenseAsposeBarCode(path);
            setLicenseAsposePDF(path);
        }
    }
}
