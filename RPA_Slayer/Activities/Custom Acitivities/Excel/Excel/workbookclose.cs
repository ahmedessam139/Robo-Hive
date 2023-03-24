using System;
using System.Activities;
using System.ComponentModel;
using Excel;
using Microsoft.Office.Interop.Excel;

namespace Excel
{ 
    public class WorkbookClose : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Name { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = this.Name.Get(context);    
            Console.WriteLine(wbName);

            ExcelBot.Shared.GetWorkbookByName(wbName, false).Close();

            if (ExcelBot.Shared.GetApp().Workbooks.Count == 0)
            {
                ExcelBot.Shared.Dispose();
            }
        }
    }
}