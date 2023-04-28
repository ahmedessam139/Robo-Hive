using System;
using System.Activities;
using Excel;

namespace Excel
{
    public class WorkbookCreate : CodeActivity
    {
        public OutArgument<String> WorkbookName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = ExcelBot.Shared.GetApp().Workbooks.Add().Name.ToString();
            //count_sheets += 1;

            context.SetValue(this.WorkbookName, wbName);
        }
    }
}