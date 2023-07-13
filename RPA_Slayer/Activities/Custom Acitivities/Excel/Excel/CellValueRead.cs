using System.Activities;
using Microsoft.Office.Interop.Excel;
using System;

namespace Excel
{
    public sealed class CellValueRead : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        [RequiredArgument]
        public InArgument<string> WorksheetName { get; set; }

        [RequiredArgument]
        public InArgument<string> Cell { get; set; }

        public OutArgument<string> Value { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = WorkbookName.Get(context);
            var wsName = WorksheetName.Get(context);
            var cell = Cell.Get(context);

            Application excelApp = new Application();
            Workbook workbook = excelApp.Workbooks.Open(wbName);
            Worksheet worksheet = workbook.Worksheets[wsName];

            Range range = worksheet.Range[cell];
            string value = range.Value?.ToString();

            workbook.Close(false, Type.Missing, Type.Missing);
            excelApp.Quit();

            Value.Set(context, value);
        }
    }
}
