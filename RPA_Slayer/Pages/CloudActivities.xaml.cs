﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using RPA_Slayer;

namespace RPA_Slayer.Pages
{
    public partial class CloudActivities : MahApps.Metro.Controls.MetroWindow
    {
        
        public CloudActivities()
        {
            InitializeComponent();
            LoadPackagesAsync();
        }

        private async void LoadPackagesAsync()
        {
            try
            {
                // JSON URL endpoint
                string url = "https://my-json-server.typicode.com/shehata1999/json/packages";

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

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string downloadLink = button.Tag.ToString();

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string downloadLocation = @"..\..\Activities\Assemblies"; // Specify your desired download location here
                    string fileName = Path.GetFileName(downloadLink);
                    string filePath = Path.Combine(downloadLocation, fileName);

                    webClient.DownloadFile(downloadLink, filePath);

                    MessageBox.Show($"File downloaded successfully to: {filePath}");
                }
            }
            catch (Exception ex)
            {
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
