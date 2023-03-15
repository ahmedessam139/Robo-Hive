using System;
using System.Activities;
using System.Runtime.InteropServices;

namespace Processes_Control
{
    public sealed class focus_on : CodeActivity
    {
        // Define an activity input argument of type int for the process ID
        [RequiredArgument]
        public InArgument<int> ProcessId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int processId = context.GetValue(this.ProcessId);

            // Get the process object from its ID
            var process = System.Diagnostics.Process.GetProcessById(processId);

            // Focus on the process's main window
            SetForegroundWindow(process.MainWindowHandle);
        }

        // Import the SetForegroundWindow method from the user32.dll library
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
