using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RPA_Slayer.Pages
{
    public partial class RemovePackage : MetroWindow
    {
        private List<string> dllFiles;

        public RemovePackage()
        {
            InitializeComponent();
            LoadFilesWithDllExtension();
        }

        private void LoadFilesWithDllExtension()
        {
            string folderPath = @"..\..\Activities\Assemblies"; // Replace with the actual folder path you want to search in
            dllFiles = new List<string>();

            // Check if the specified folder exists
            if (Directory.Exists(folderPath))
            {
                // Get all files in the folder with the .dll extension
                string[] files = Directory.GetFiles(folderPath, "*.dll");

                // Add the file names to the list
                foreach (string file in files)
                {
                    dllFiles.Add(Path.GetFileName(file));
                }
            }

            // Display the file names in the ListView control
            fileListView.ItemsSource = dllFiles;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string selectedItem = (string)button.Tag;

            // Implement the logic to remove the selected item from the directories on the disk
            string directoryPath1 = @"..\..\Activities\Assemblies"; // Replace with the actual directory path
            string directoryPath2 = @"..\..\bin\Debug"; // Replace with the actual directory path

            string filePath1 = Path.Combine(directoryPath1, selectedItem);
            string filePath2 = Path.Combine(directoryPath2, selectedItem);

            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
                Console.WriteLine("Deleted file from directory 1: " + filePath1);
            }

            if (File.Exists(filePath2))
            {
                ForceDeleteFile(filePath2);
                Console.WriteLine("Deleted file from directory 2: " + filePath2);
            }

            // Save the deleted item to a text file
            string logFilePath = @"..\..\DeletedItems.txt"; // Replace with the actual path for the log file

            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(selectedItem);
                }
                Console.WriteLine("Deleted item saved to log file: " + selectedItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving deleted item to log file: " + ex.Message);
            }

            // Remove the selected item from the list and update the ListView
            dllFiles.Remove(selectedItem);
            fileListView.Items.Refresh();
            MessageBoxResult result = MessageBox.Show("Do you want to restart the app now?", "Restart App", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Restart the app
                System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                // Continue with the app execution
                // Handle the logic accordingly
            }

           
           
        }
        private void ForceDeleteFile(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // File stream is opened with write access, which allows us to release the file lock
                }
                File.Delete(filePath);
                Console.WriteLine("File deleted successfully: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting file: " + ex.Message);
            }
        }

    }
}
