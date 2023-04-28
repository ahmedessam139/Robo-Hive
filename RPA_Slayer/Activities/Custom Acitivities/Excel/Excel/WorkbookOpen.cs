using Excel;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace Excel
{

    public class WorkbookOpen : CodeActivity
    {

        [Category("Input")] //type input
        [RequiredArgument] //to force the user to enter input
        public InArgument<string> workbookName { get; set; }

        [Category("Output")] //type output
        public OutArgument<string> Name { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = this.workbookName.Get(context);


            ExcelBot.Shared.GetApp().Workbooks.Open(wbName);
            string fileName = Path.GetFileName(wbName);
            Name.Set(context, fileName);
            Console.WriteLine(wbName);
            Console.WriteLine(fileName);

            // pass the fileName to the next activity
            context.SetValue(Name, fileName);

            if (ExcelBot.Shared.GetApp().Workbooks.Count == 0)
            {
                ExcelBot.Shared.Dispose();
            }
        }
    }
}
