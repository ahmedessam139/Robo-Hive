using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Diagnostics;

namespace Processes_Control

{
    [System.ComponentModel.Designer(typeof(open_app_designer), typeof(System.ComponentModel.Design.IDesigner))]

    public sealed class open_app : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> AppPath { get; set; }

        public OutArgument<int> ProcessId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string appPath = context.GetValue(this.AppPath);

            try
            {
                Process process = Process.Start(appPath);
                context.SetValue(ProcessId, process.Id);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur while starting the process
                Console.WriteLine("Error: " + ex.Message);
                context.SetValue(ProcessId, -1); // Return -1 as process ID to indicate an error
            }
        }
    }
}
