using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RPA_Slayer.Helpers;
using RPA_Slayer.Pages;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Activities;
using System.Windows.Interop;
using MahApps.Metro.Controls;


//Tmam

namespace RPA_Slayer
{



    public partial class MainWindow : MetroWindow
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MYACTION_HOTKEY_ID = 1;

        private HwndSource hwndSource;

        public MainWindow()
        {
            InitializeComponent();

            // Attach an event handler to the SourceInitialized event
            SourceInitialized += MainWindow_SourceInitialized;
        }

       
        

        private void btnRunLoadedWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner.RunWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Errorr", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void fileMenuExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void fileMenuItem_Click_LoadWorkflow(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog(this).Value)
            {
                wfDesigner.WorkflowFilePath = openFileDialog.FileName;
                wfDesigner.AddWorkflowDesigner();

            }
        }

        private void WFHost_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void btnSaveWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner.SaveWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner.NewWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpenWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner.OpenWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFastRunWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner.FastRunWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnStopWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnBreakpointToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner.BreakPointToggle();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wfDesigner._continue();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStopDep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void sendToOrc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wfDesigner.WorkflowFilePath == wfDesigner.DefultWorkflowFilePath)
                {
                    System.Windows.MessageBox.Show("Please load a workflow first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //orchestratorCommunications orc = new orchestratorCommunications();
                //orc.ShowForm(wfDesigner.WorkflowFilePath);

                Orc_Config orc = new Orc_Config(wfDesigner.WorkflowFilePath);
                orc.Show();

            }
            catch (Exception ex)    
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
        private void sendToCloud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloudActivities ca = new CloudActivities();
                ca.Show();
            }
            catch(Exception ex)
            {
                 System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        recording rec = new recording();
        private void start_Recording(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wfDesigner.WorkflowFilePath == wfDesigner.DefultWorkflowFilePath)
                {
                    System.Windows.MessageBox.Show("Please load a workflow first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                rec.StartRecord(wfDesigner.WorkflowFilePath);


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void stop_Recording(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wfDesigner.WorkflowFilePath == wfDesigner.DefultWorkflowFilePath)
                {
                    System.Windows.MessageBox.Show("Please load a workflow first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                rec.StopRecord(wfDesigner.WorkflowFilePath);
                wfDesigner.AddWorkflowDesigner();
                



            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        #region Key_Hook_region
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            // Retrieve the window handle
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            // Register the hotkey using the window handle
            RegisterHotKey(windowHandle, MYACTION_HOTKEY_ID, 0, (int)Keys.F8);

            // Create the HwndSource instance
            hwndSource = HwndSource.FromHwnd(windowHandle);
            hwndSource.AddHook(HwndSourceHook);

            // Remove the event handler after it's done
            SourceInitialized -= MainWindow_SourceInitialized;
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unregister the hotkey
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            UnregisterHotKey(windowHandle, MYACTION_HOTKEY_ID);

            // Detach the HwndSource and remove the hook
            hwndSource.RemoveHook(HwndSourceHook);
            hwndSource.Dispose();
            hwndSource = null;

            base.OnClosed(e);
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == MYACTION_HOTKEY_ID)
            {
                rec.StopRecord(wfDesigner.WorkflowFilePath);
                wfDesigner.AddWorkflowDesigner();
            }

            return IntPtr.Zero;
        }



        #endregion

     
    }
}
