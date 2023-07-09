using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using RPA_Slayer;
using System.Activities.Presentation;

namespace RPA_Slayer.Pages
{
    public partial class CloudActivities : MahApps.Metro.Controls.MetroWindow
    {
        private WorkflowDesigner wfDesigner;


        public CloudActivities(WorkflowDesigner designer)
        {
            InitializeComponent();
            LoadPackagesAsync();
            wfDesigner = designer;

        }

        private async void LoadPackagesAsync()
        {
            try
            {
                // JSON URL endpoint
                string url = "http://localhost:3000/packages";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<Package> packages = JsonConvert.DeserializeObject<List<Package>>(json);
                    DataContext = packages;


                }
                else
                {
                    MessageBox.Show("Failed to load packages. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string downloadLink = button.Tag.ToString();
            string packageName = ((Package)button.DataContext).Name;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string downloadLocation = @"..\..\Activities\Assemblies"; // Specify your desired download location here
                    string fileName = Path.GetFileName(downloadLink);
                    string filePath = Path.Combine(downloadLocation, fileName);

                    // Change button content to "Installing"
                    button.Content = "Installing";

                    // Download the file asynchronously
                    await webClient.DownloadFileTaskAsync(downloadLink, filePath);

                    // Change button content to "Installed"
                    button.Content = "Installed";

                    MessageBox.Show($"File downloaded successfully to: {filePath}");

                    // Search and remove package name from the text file
                    string textFilePath = @"..\..\DeletedItems.txt"; // Specify the path to your text file
                    string[] lines = File.ReadAllLines(textFilePath);
                    List<string> updatedLines = new List<string>();

                    foreach (string line in lines)
                    {
                        if (!line.Contains(packageName))
                        {
                            updatedLines.Add(line);
                        }
                    }

                    File.WriteAllLines(textFilePath, updatedLines);
                }
            }
            catch (Exception ex)
            {
                button.Content = "Error-install again";
                MessageBox.Show("An error occurred while downloading: " + ex.Message);
            }
        }

    }

    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}