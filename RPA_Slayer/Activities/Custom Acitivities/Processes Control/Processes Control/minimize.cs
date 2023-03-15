using System;
using System.Activities;
using System.Runtime.InteropServices;

namespace Processes_Control
{
    public sealed class minimize : CodeActivity
    {
        // Define an activity input argument of type int for the process ID
        [RequiredArgument]
        public InArgument<int> ProcessId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int processId = context.GetValue(this.ProcessId);

            // Get the process object from its ID
            var process = System.Diagnostics.Process.GetProcessById(processId);

            // Minimize the process's main window
            ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
        }

        // Import the ShowWindow method and the SW_MINIMIZE constant from the user32.dll library
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MINIMIZE = 6;
    }
}
