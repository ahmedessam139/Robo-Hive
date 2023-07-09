using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

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
    }

    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
    }
}
