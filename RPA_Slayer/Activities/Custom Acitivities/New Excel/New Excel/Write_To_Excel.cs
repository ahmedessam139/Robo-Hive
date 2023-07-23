using System;
using System.Activities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_Library
{
    public sealed class Write_To_Excel : CodeActivity
    {
        public InArgument<Excel.Worksheet> Worksheet { get; set; }

        public InArgument<int> RowIndex { get; set; }

        public InArgument<int> ColumnIndex { get; set; }

        public InArgument<string> ValueToWrite { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Excel.Worksheet worksheet = Worksheet.Get(context);
            int rowIndex = RowIndex.Get(context);
            int columnIndex = ColumnIndex.Get(context);
            string valueToWrite = ValueToWrite.Get(context);

            Excel.Range cell = (Excel.Range)worksheet.Cells[rowIndex, columnIndex];
            cell.Value2 = valueToWrite;
        }
    }
}
