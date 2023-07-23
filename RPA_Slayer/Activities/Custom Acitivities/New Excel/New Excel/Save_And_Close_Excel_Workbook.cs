using System;
using System.Activities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_Library
{
    public sealed class Save_And_Close_Excel_Workbook : CodeActivity
    {
        public InArgument<Excel.Worksheet> Worksheet { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Excel.Worksheet worksheet = Worksheet.Get(context);
            Excel.Workbook workbook = worksheet.Parent as Excel.Workbook;

            workbook.Save();
            workbook.Close();
        }
    }
}
