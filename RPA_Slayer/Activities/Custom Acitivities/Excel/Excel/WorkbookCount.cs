using System;
using System.Activities;
using System.ComponentModel;
using Excel;
using Roro.Activities.Excel;

namespace Excel
{
    public sealed class WorkbookCount : CodeActivity
    {
        [Category("Output")]
        //[RequiredArgument]
        public OutArgument<int> Count { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var count = ExcelBot.Shared.GetApp().Workbooks.Count;
            Console.WriteLine(count.ToString());
            context.SetValue(Count, count);
        }
    }
}
