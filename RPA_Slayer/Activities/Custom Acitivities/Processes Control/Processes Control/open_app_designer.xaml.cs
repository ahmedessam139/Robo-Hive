using Microsoft.Win32;
using System.Activities.Presentation.Model;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Diagnostics;

namespace Processes_Control
{
    // Interaction logic for open_app_designer.xaml
    public partial class open_app_designer
    {
        public open_app_designer()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Application files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                InArgument<string> appPath = new InArgument<string>(openFileDialog.FileName);
                ModelItem.Properties["AppPath"].SetValue(appPath);
            }
        }
    }
}
