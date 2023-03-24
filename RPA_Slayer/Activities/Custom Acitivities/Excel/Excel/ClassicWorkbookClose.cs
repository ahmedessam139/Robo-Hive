using System;
using System.Activities;
using System.Runtime.InteropServices;
namespace Excel
{
    public class ClassicWorkbookClose : CodeActivity 
    {
        public InArgument<string> WorkbookName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string wbName = WorkbookName.Get(context);
            Console.WriteLine(wbName);
            
            ExcelBot.Shared.GetWorkbookByName(wbName, true).Close();

            if (ExcelBot.Shared.GetApp().Workbooks.Count == 0)
            {
                ExcelBot.Shared.Dispose();
            }
        }
    }
}
