using System.Activities;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;
using Excel;

namespace Excel
{
    public class CellValuePaste : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorksheetName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Cell { get; set; }

        [Category("Input")]
        public InArgument<bool> PasteValuesOnly { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string wbName = WorkbookName.Get(context);
            string wsName = WorksheetName.Get(context);
            string range = Cell.Get(context);
            bool pasteType = PasteValuesOnly.Get(context);

            Microsoft.Office.Interop.Excel.Worksheet worksheet = ExcelBot.Shared.GetWorksheetByName(wbName, wsName,true);
            Microsoft.Office.Interop.Excel.Range cell = worksheet.Range[range];

            if (pasteType)
            {
                cell.PasteSpecial(Microsoft.Office.Interop.Excel.XlPasteType.xlPasteValuesAndNumberFormats);
            }
            else
            {
                cell.PasteSpecial();
            }
        }
    }
}
