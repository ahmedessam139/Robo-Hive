using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.IO;
using System.Windows.Input;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Diagnostics;

namespace RPA_Slayer.Helpers
{
    public class recording
    {
        private AutomationElement _rootElement;
        private List<AutomationElement> _recordedActions = new List<AutomationElement>();
        private List<string> _elementNames = new List<string>();

        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;

        private Form overlayForm;
        private Label overlayLabel;

        public void StartRecord(string workflowFilePath)
        {
            // Minimize all windows and show the desktop
            MinimizeAllWindows();

            // Find the root element of the desktop
            _rootElement = AutomationElement.RootElement;

            // Create and show the overlay form
            CreateOverlayForm();

            // Subscribe to UI automation events
            Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent, _rootElement, TreeScope.Descendants, OnInvoked);
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);

            Console.WriteLine("Recording started...");
        }

        private void MinimizeAllWindows()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                IntPtr hWnd = process.MainWindowHandle;
                ShowWindow(hWnd, 2); // Minimize window
            }
        }

        private void CreateOverlayForm()
        {
            overlayForm = new Form();
            overlayForm.FormBorderStyle = FormBorderStyle.None;
            overlayForm.ShowInTaskbar = false;
            overlayForm.BackColor = Color.Black;
            overlayForm.TransparencyKey = Color.Black;
            overlayForm.TopMost = true;
            overlayForm.Size = new Size(400, 100); // Set the desired size of the overlay form
            overlayForm.StartPosition = FormStartPosition.Manual;
            overlayForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - overlayForm.Width, Screen.PrimaryScreen.WorkingArea.Bottom - overlayForm.Height); // Position the form in the bottom right corner

            overlayLabel = new Label();
            overlayLabel.ForeColor = Color.White;
            overlayLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            overlayLabel.AutoSize = true;
            overlayLabel.Location = new Point(40, 10);
            overlayLabel.Text = "Recording......\n\nPress F8 to stop recording";

            Label dotLabel = new Label();
            dotLabel.ForeColor = Color.Red;
            dotLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            dotLabel.AutoSize = true;
            dotLabel.Location = new Point(10, 10);
            dotLabel.Text = "●"; // Red dot symbol

            overlayForm.Controls.Add(overlayLabel);
            overlayForm.Controls.Add(dotLabel);
            overlayForm.Show();

            SetWindowLong(overlayForm.Handle, GWL_EXSTYLE, GetWindowLong(overlayForm.Handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_NOACTIVATE);
            SetLayeredWindowAttributes(overlayForm.Handle, 0, 128, 0x2);
        }


        private void DestroyOverlayForm()
        {
            overlayForm.Close();
            overlayForm.Dispose();
            overlayForm = null;
            overlayLabel = null;
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            try
            {
                AutomationElement focusedElement = sender as AutomationElement;

                if (focusedElement != null)
                {
                    Console.WriteLine("Focused element AutomationId------>: {0}", focusedElement.Current.AutomationId);
                    Console.WriteLine("Focused element Name------>: {0}", focusedElement.Current.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in OnFocusChanged: {0}", ex.Message);
            }
        }

        private void OnInvoked(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;
            if (element != null)
            {
                _recordedActions.Add(element);
                // Save the element name in XML format to the list
                string xmlElementName = string.Format("<u:InvokeElement ElementName=\"{0}\" />", element.Current.Name);
                _elementNames.Add(xmlElementName);
                // Print the accumulated list of element names
                Console.WriteLine(string.Join(Environment.NewLine, _elementNames));
                Console.WriteLine(_elementNames);
                // CLICK ON ELEMENT
                Thread.Sleep(200);
            }
        }

        public string StopRecord(string filePath)
        {
            // Remove all event handlers
            Automation.RemoveAllEventHandlers();

            // Destroy the overlay form
            DestroyOverlayForm();


            // Read the content of the file
            string content = File.ReadAllText(filePath);
            // Split the content into lines
            string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            // Compute the index of the line before the last 2 lines
            int insertionIndex = Math.Max(0, lines.Length - 2);
            // Insert the elements before the line at the insertion index
            lines[insertionIndex] = string.Join("" + Environment.NewLine, _elementNames) + Environment.NewLine + lines[insertionIndex];
            // Remove all elements (_elementNames)
            _elementNames.Clear();
            // Join the lines back into a single string
            content = string.Join(Environment.NewLine, lines);
            // Save the modified content back to the file
            File.WriteAllText(filePath, content);

            // Return the updated content
            return content;
        }

       
    }
}
