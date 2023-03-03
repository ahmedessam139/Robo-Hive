using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Processes_Control
{
    public sealed class close_app : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);
        }
    }
}
