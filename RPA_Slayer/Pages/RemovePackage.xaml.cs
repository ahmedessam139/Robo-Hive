using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using MahApps.Metro.Controls;


namespace RPA_Slayer.Pages
{
    public partial class RemovePackage : MetroWindow
    {
        public RemovePackage()
        {
            InitializeComponent();
            LoadFilesWithDllExtension();
        }

        private void LoadFilesWithDllExtension()
        {
            string folderPath = @"..\..\Activities\Assemblies"; // Replace with the actual folder path you want to search in
            List<string> dllFiles = new List<string>();

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
    }
}
