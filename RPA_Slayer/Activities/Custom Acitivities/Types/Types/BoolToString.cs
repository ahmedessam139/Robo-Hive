using System;
using System.Activities;

namespace Types
{
    public sealed class BoolToString : CodeActivity
    {
        public InArgument<bool> Input { get; set; }
        public OutArgument<string> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            bool boolValue = Input.Get(context);
            string stringValue = boolValue.ToString();

            Output.Set(context, stringValue);
        }
    }
}
