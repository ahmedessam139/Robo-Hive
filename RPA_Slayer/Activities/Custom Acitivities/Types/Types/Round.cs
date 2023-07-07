using System;
using System.Activities;

namespace Types
{
    public sealed class RoundFloat : CodeActivity
    {
        public InArgument<float> Input { get; set; }
        public OutArgument<string> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            float floatValue = Input.Get(context);
            float roundedValue = (float)Math.Round(floatValue);
            string output = roundedValue.ToString();

            Output.Set(context, output);
        }
    }
}
