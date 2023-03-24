using System.Activities;
//using System.Activities.CodeActivity;
using System.ComponentModel;

namespace Excel
{
    public class WorksheetDelete : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> WorksheetName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = WorkbookName.Get(context);
            var wsName = WorksheetName.Get(context);

            ExcelBot.Shared.GetWorksheetByName(wbName, wsName, true).Delete();
        }
    }
}
