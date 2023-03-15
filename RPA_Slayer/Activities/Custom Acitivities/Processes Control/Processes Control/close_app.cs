using System;
using System.Activities;
using System.Diagnostics;

namespace Processes_Control
{
    [System.ComponentModel.Designer(typeof(close_app_desginer), typeof(System.ComponentModel.Design.IDesigner))]
    public sealed class close_app : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> ProcessId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int processId = ProcessId.Get(context);

            try
            {
                Process process = Process.GetProcessById(processId);
                process.Kill();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur while killing the process
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
