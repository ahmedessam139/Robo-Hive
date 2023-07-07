using System;
using System.Activities;

namespace Types
{
    public sealed class FloatToInt : CodeActivity
    {
        public InArgument<float> Input { get; set; }
        public OutArgument<int> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            float floatValue = Input.Get(context);
            int intValue = Convert.ToInt32(floatValue);

            Output.Set(context, intValue);
        }
    }
}
