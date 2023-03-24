using System.Activities;
using Excel;

namespace Excel
{
    public sealed class CellValueRead : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> WorkbookName { get; set; }

        [RequiredArgument]
        public InArgument<string> WorksheetName { get; set; }

        [RequiredArgument]
        public InArgument<string> Cell { get; set; }

        public OutArgument<string> Value { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var wbName = WorkbookName.Get(context);
            var wsName = WorksheetName.Get(context);
            var range = Cell.Get(context);

            var value = ExcelBot.Shared.GetRange(wbName, wsName, range).Value?.ToString() ?? string.Empty;

            Value.Set(context, value);
        }
    }
}
