using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace IO_Modules
{

    public class MouseControl : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<int> X { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<int> Y { get; set; }

        [Category("Input")]
        public bool LeftClick { get; set; }

        [Category("Input")]
        public bool RightClick { get; set; }

        [Category("Input")]
        public bool DoubleClick { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Get input arguments for X and Y coordinates
            int x = X.Get(context);
            int y = Y.Get(context);

            // Move the mouse cursor to the specified coordinates
            Cursor.Position = new System.Drawing.Point(x, y);

            // Check if left, right, or double click should be performed
            if (LeftClick)
            {
                // Perform a left mouse click
                DoMouseClick(MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP, DoubleClick);
            }
            else if (RightClick)
            {
                // Perform a right mouse click
                DoMouseClick(MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP, DoubleClick);
            }
        }

        // Simulate a mouse click
        private void DoMouseClick(int downFlag, int upFlag, bool doubleClick)
        {
            if (doubleClick)
            {
                // Perform a double-click by sending two sets of mouse down/up events
                mouse_event(downFlag, 0, 0, 0, 0);
                mouse_event(upFlag, 0, 0, 0, 0);
                mouse_event(downFlag, 0, 0, 0, 0);
                mouse_event(upFlag, 0, 0, 0, 0);
            }
            else
            {
                // Perform a single-click
                mouse_event(downFlag, 0, 0, 0, 0);
                mouse_event(upFlag, 0, 0, 0, 0);
            }
        }

        // Import the mouse_event function from user32.dll
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        // Constants for various mouse events
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
    }
}
