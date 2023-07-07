using System;
using System.Activities;

namespace Types
{
    public sealed class StringToInt : CodeActivity
    {
        public InArgument<string> Input { get; set; }
        public OutArgument<int> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string stringValue = Input.Get(context);
            int intValue;
            bool success = int.TryParse(stringValue, out intValue);

            if (!success)
            {
                throw new ArgumentException("Invalid input. Unable to convert string to int.");
            }

            Output.Set(context, intValue);
        }
    }
}
