using System.Activities;
//using System.Activities.CodeActivity;
using System.ComponentModel;

namespace Excel
{
    public class WorksheetExists : CodeActivity<bool>
    {
        [Category("Input")]
        //[RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        [Category("Input")]
        //[RequiredArgument]
        public InArgument<string> WorksheetName { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var wbName = WorkbookName.Get(context);
            var wsName = WorksheetName.Get(context);

            return ExcelBot.Shared.GetWorksheetByName(wbName, wsName, false) != null;
        }
    }
}
