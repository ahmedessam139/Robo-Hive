using System.Activities;
using Excel;
using Microsoft.Office.Interop.Excel;
namespace Excel
{
    public class WorkbookCloseAll : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var workbooks = ExcelBot.Shared.GetApp().Workbooks;

            foreach (Workbook workbook in workbooks)
            {
                workbook.Close(false);
            }

            if (workbooks.Count == 0)
            {
                ExcelBot.Shared.Dispose();
            }
        }
    }
}
