using System;
using System.Activities;

namespace Types
{
    public sealed class FloorFloatA : CodeActivity
    {
        public InArgument<float> Input { get; set; }
        public OutArgument<float> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            float floatValue = Input.Get(context);
            float flooredValue = (float)Math.Floor(floatValue);

            Output.Set(context, flooredValue);
        }
    }
}
