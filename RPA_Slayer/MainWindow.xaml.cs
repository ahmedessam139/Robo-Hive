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
//Tmam

namespace RPA_Slayer
{
    

    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                //import the class from the Helpers folder

                //acces methode from orchestratorConfig.cs file 
                orchestratorConfig orc = new orchestratorConfig();
                orc.ShowForm();


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
    }
}
