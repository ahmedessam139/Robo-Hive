using Excel;
using System;
using System.Activities;

namespace Roro.Activities.Excel
{
    public class MacroRun : CodeActivity<string>
    {
        public InArgument<string> Macro { get; set; }

        public InArgument<string> Param1 { get; set; }

        public InArgument<string> Param2 { get; set; }

        public InArgument<string> Param3 { get; set; }

        public InArgument<string> Param4 { get; set; }

        protected override string Execute(CodeActivityContext context)
        {
            var macro = this.Macro.Get(context);
            var param1 = this.Param1.Get(context);
            var param2 = this.Param2.Get(context);
            var param3 = this.Param3.Get(context);
            var param4 = this.Param4.Get(context);

            var result = string.Empty;

            // Build an array of parameter values.
            var parameters = new object[]
            {
                param1, param2, param3, param4
            };

            // Remove null parameters from the end of the array.
            while (parameters.Length > 0 && parameters[parameters.Length - 1] == null)
            {
                Array.Resize(ref parameters, parameters.Length - 1);
            }

            // Call the "Run" method with the parameter array.
            result = ExcelBot.Shared.GetApp().Run(macro, parameters);

            return result;
        }
    }
}
