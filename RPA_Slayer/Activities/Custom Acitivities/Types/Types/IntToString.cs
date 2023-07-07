using System;
using System.Activities;

namespace Types
{
    public sealed class IntToString : CodeActivity
    {
        public InArgument<int> Input { get; set; }
        public OutArgument<string> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int inputValue = Input.Get(context);
            string stringValue = inputValue.ToString();

            Output.Set(context, stringValue);
        }
    }
}
