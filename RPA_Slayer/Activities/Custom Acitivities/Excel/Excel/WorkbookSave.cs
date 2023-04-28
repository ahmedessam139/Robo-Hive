using Excel;
using System.Activities;
//using System.Activities.CodeActivity;
using System.ComponentModel;

namespace Excel
{
    public class WorkbookSave : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = WorkbookName.Get(context);

            ExcelBot.Shared.GetWorkbookByName(wbName, true).Save();
        }
    }
}
