using System.Activities;
//using System.Activities.CodeActivity;
using System.ComponentModel;

namespace Excel
{
    public class WorkbookSaveAs : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorkbookNameAs { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = WorkbookName.Get(context);
            var wbNameAs = WorkbookNameAs.Get(context);

            ExcelBot.Shared.GetWorkbookByName(wbName, true).SaveAs(wbNameAs);
        }
    }
}
