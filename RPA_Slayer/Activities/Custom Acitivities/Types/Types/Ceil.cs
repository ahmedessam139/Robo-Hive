using System;
using System.Activities;

namespace Types
{
    public sealed class CeilFloat : CodeActivity
    {
        public InArgument<float> Input { get; set; }
        public OutArgument<float> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            float floatValue = Input.Get(context);
            float ceiledValue = (float)Math.Ceiling(floatValue);

            Output.Set(context, ceiledValue);
        }
    }
}
