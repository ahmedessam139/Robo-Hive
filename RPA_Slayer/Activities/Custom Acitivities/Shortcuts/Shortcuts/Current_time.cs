using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortcuts
{
    public sealed class Current_DateTime : CodeActivity
    {
        public OutArgument<string> CurrentDateTime { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Get the current date and time
            DateTime currentDateTime = DateTime.Now;

            // Format the date and time as a string
            string formattedDateTime = currentDateTime.ToString();

            // Set the formatted date and time as the value of the CurrentDateTime argument
            CurrentDateTime.Set(context, formattedDateTime);
        }
    }
}