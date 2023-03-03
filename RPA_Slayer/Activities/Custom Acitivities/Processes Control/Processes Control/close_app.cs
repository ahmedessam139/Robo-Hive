using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Processes_Control
{
    [System.ComponentModel.Designer(typeof(close_app_desginer), typeof(System.ComponentModel.Design.IDesigner))]

    public sealed class close_app : CodeActivity
    {
        // Define an activity input argument of type int
        public InArgument<int> ProcessId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the ProcessId input argument
            int processId = context.GetValue(this.ProcessId);

            try
            {
                // Get the process by its ID
                Process process = Process.GetProcessById(processId);

                // Close the process's main windowj
                process.CloseMainWindow();

                // Wait for the process to exit
                process.WaitForExit();

                // Dispose of the process
                process.Dispose();
            }
            catch (ArgumentException ex)
            {
                // The process with the specified ID was not found
                Console.WriteLine(ex.Message);
            }
        }
    }
}
