using System;
using System.Activities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_Library
{
    public sealed class Open_Excel_Workbook : CodeActivity
    {
        public InArgument<string> FilePath { get; set; }

        public OutArgument<Excel.Worksheet> Worksheet { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string filePath = FilePath.Get(context);

            var excelApp = new Excel.Application();
            var workbook = excelApp.Workbooks.Open(filePath);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            Worksheet.Set(context, worksheet);
        }
    }
}
