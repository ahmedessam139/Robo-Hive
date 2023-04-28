using System.Activities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel
{
    public sealed class CellValueClear : CodeActivity
    {
        public InArgument<string> WorkbookName { get; set; }

        public InArgument<string> WorksheetName { get; set; }

        public InArgument<string> Cell { get; set; }

        public InArgument<bool> ClearContentsOnly { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string wbName = context.GetValue(this.WorkbookName);
            string wsName = context.GetValue(this.WorksheetName);
            string range = context.GetValue(this.Cell);
            bool clearContentsOnly = context.GetValue(this.ClearContentsOnly);

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = false;
            app.DisplayAlerts = false;

            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Open(wbName);
            Microsoft.Office.Interop.Excel.Worksheet ws = wb.Worksheets[wsName];

            Microsoft.Office.Interop.Excel.Range rng = ws.Range[range];

            if (clearContentsOnly)
            {
                rng.ClearContents();
            }
            else
            {
                rng.Clear();
            }

            wb.Save();
            wb.Close();

            app.Quit();
        }
    }
}
