using System;
using System.Activities;

namespace Types
{
    public sealed class IntToFloat : CodeActivity
    {
        public InArgument<int> Input { get; set; }
        public OutArgument<float> Output { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int intValue = Input.Get(context);
            float floatValue = Convert.ToSingle(intValue);

            Output.Set(context, floatValue);
        }
    }
}
