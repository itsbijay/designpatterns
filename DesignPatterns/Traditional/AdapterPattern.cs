namespace DesignPatterns.Traditional.Adapter
{
    /*******************************************************************************
     * Adapter Pattern
     *******************************************************************************
     * What is it?
     * One of original GoF patterns to share classes with incompatible interfaces
     * 
     * Real-world Examples:
     *  - 3rd party libraries
     *  - Integrations
     *  - Multiple user interfaces
     *  - COM / interop libraries
     * 
     * Demo
     *  - To share code, abstracted "IReport" for common exports of HTML table
     *  - We already have code to export to word/excel, but need 3rd party for PDF
     * 
     *******************************************************************************/

    public interface IReport { void Export(); }
    public class WordReport : IReport { public void Export() { } }
    public class ExcelReport : IReport { public void Export() { } }

    public class CustomPdfExporter { public void Save() { } } // Does not implement IReport
    public class PdfAdapterReport : IReport
    {
        protected CustomPdfExporter _exporter = new CustomPdfExporter();
        public void Export() => _exporter.Save(); // Adapter to make PDF library work with existing code
    }

}
