using System;
using static System.Net.Mime.MediaTypeNames;
using System.Activities;
using System.IO;

namespace Excel
{
    public class ClassicWorkbookOpen : CodeActivity
    {
        [RequiredArgument]
        public InArgument<String> WorkbookName { get; set; }
        //public OutArgument<string> WorkbookNameOut { get; set; }
        public OutArgument<String> WorkbookNameOut { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = context.GetValue(this.WorkbookName);

            ExcelBot.Shared.GetApp().Workbooks.Open(wbName);
            
            //string bookName = Path.GetFileName(wbName);
            //context.SetValue(this.WorkbookNameOut, bookName);
            //Console.WriteLine(bookName);

            if (ExcelBot.Shared.GetApp().Workbooks.Count == 0)
            {
                ExcelBot.Shared.Dispose();
            }
            context.SetValue(this.WorkbookNameOut, wbName);
            Console.WriteLine("this is context rn:");
            Console.WriteLine(context);
        }
    }
}
