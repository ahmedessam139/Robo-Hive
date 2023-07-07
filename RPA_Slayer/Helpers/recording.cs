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
        private bool _isKeyboardRecordingActive = false;
        private LowLevelKeyboardHook _keyboardHook;
        private List<string> word = new List<string>();

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
            //Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);

            // Enable global keyboard recording
            _isKeyboardRecordingActive = true;
            _keyboardHook = new LowLevelKeyboardHook();
            _keyboardHook.KeyDownEvent += OnKeyDown;
            _keyboardHook.Install();

            Console.WriteLine("Recording started...");
        }

        private void OnInvoked(object sender, AutomationEventArgs e)
        {
            try
            {
                AutomationElement element = sender as AutomationElement;
                Console.WriteLine("Invoked element AutomationId------>: {0}", element.Current.AutomationId);

                if (word.Count > 0)
                {
                    AddWordToList();
                    word.Clear();
                }


                if (true)
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
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in OnInvoked: {0}", ex.Message);
            }
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            try
            {
                if (word.Count > 0)
                {
                    AddWordToList();
                    word.Clear();
                }
                AutomationElement focusedElement = sender as AutomationElement;
                string elementName = focusedElement.Current.Name;
                Console.WriteLine("Focus changed to: " + elementName);
                Thread.Sleep(500);


                if (focusedElement != null)
                {
                    _recordedActions.Add(focusedElement);
                    // Save the element name in XML format to the list
                    string xmlElementName = string.Format("<u:FocusElement ElementName=\"{0}\" />", elementName);
                    _elementNames.Add(xmlElementName);
                    // Print the accumulated list of element names
                    Console.WriteLine(string.Join(Environment.NewLine, _elementNames));
                    Console.WriteLine(_elementNames);
                    // CLICK ON ELEMENT
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in OnFocusChanged: {0}", ex.Message);
            }
        }

        private void OnKeyDown(object sender, LowLevelKeyboardEventArgs e)
        {
            if (_isKeyboardRecordingActive)
            {
                string key = e.Key.ToString();

                if (key.Length == 1 && char.IsLetterOrDigit(key[0]))
                {
                    word.Add(key);
                    string fullWord = string.Join("", word);
                    Console.WriteLine("Current word: " + fullWord);
                }
            }
        }

        private void AddWordToList()
        {
            string fullWord = string.Join("", word);
            string formattedWord = $"<i:KeyboardControl Text=\"{fullWord}\" />";
            _elementNames.Add(formattedWord);

        }

        private void MinimizeAllWindows()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                IntPtr hWnd = process.MainWindowHandle;
                ShowWindow(hWnd, 2); // Minimize window sfawertva adf
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
            overlayForm.Size = new Size(400, 100); // Set the desired size of the overlay formss
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

        public string StopRecord(string filePath)
        {

            if (word.Count > 0)
            {
                AddWordToList();
                word.Clear();
            }
            // Disable global keyboard recording
            _isKeyboardRecordingActive = false;
            _keyboardHook.Uninstall();

            // Remove all event handlers
            Automation.RemoveAllEventHandlers();

            // Destroy the overlay form
            DestroyOverlayForm();

            // Read the content of the file
            string content = File.ReadAllText(filePath);
            // Split the content into lines
            List<string> lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            // Compute the index of the line where you want to insert the new lines
            int insertionIndex = Math.Max(0, Math.Min(lines.Count, 3));

            // Append the new lines at the desired index
            //lines.InsertRange(insertionIndex, new[] { " xmlns:u=\"clr-namespace:UI_Automation;assembly=UI_Automation\"", " xmlns:i=\"clr-namespace:IO_Modules;assembly=IO-Modules\"" });
            
            int insertionIndex1 = Math.Max(0, lines.Count - 17);
            // Insert the elements before the line at the insertion index
            lines[insertionIndex1] = string.Join("" + Environment.NewLine, _elementNames) + Environment.NewLine + lines[insertionIndex1];
            // Remove all elements (_elementNames)
            _elementNames.Clear();
            // Join the lines back into a single string
            content = string.Join(Environment.NewLine, lines);
            // Save the modified content back to the file
            File.WriteAllText(filePath, content);

            // Return the updated content
            return content;
        }


        // Low-level keyboard hook class
        public class LowLevelKeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public event EventHandler<LowLevelKeyboardEventArgs> KeyDownEvent;

        public LowLevelKeyboardHook()
        {
            _proc = HookCallback;
        }

        public void Install()
        {
            _hookID = SetHook(_proc);
        }

        public void Uninstall()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KeyDownEvent?.Invoke(this, new LowLevelKeyboardEventArgs((Keys)vkCode));
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #region DLL Imports

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }

    // Event arguments for low-level keyboard eventsfdsa
    public class LowLevelKeyboardEventArgs : EventArgs
    {
        public Keys Key { get; }

        public LowLevelKeyboardEventArgs(Keys key)
        {
            Key = key;
        }
    }
}
}
