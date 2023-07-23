using System;
using System.Activities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_Library
{
    public sealed class Read_Excel_Cells : CodeActivity
    {
        public InArgument<Excel.Worksheet> Worksheet { get; set; }

        public InArgument<int> RowIndex { get; set; }

        public InArgument<int> ColumnIndex { get; set; }

        public OutArgument<string> CellValue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Excel.Worksheet worksheet = Worksheet.Get(context);
            int rowIndex = RowIndex.Get(context);
            int columnIndex = ColumnIndex.Get(context);

            Excel.Range cell = (Excel.Range)worksheet.Cells[rowIndex, columnIndex];
            string cellValue = cell.Value2?.ToString();

            CellValue.Set(context, cellValue);
        }
    }
}
