using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RPA_Slayer.Pages
{
    /// <summary>
    /// Interaction logic for Orc_Config.xaml
    /// </summary>
    public partial class Orc_Config : MetroWindow
    {
        public string Path { get; set; }

        public Orc_Config(string path)
        {
            InitializeComponent();
            Path = path;
            xamlLabel.Text = Path;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an object with the data from the UI
            var data = new
            {
                packageName = nameTextBox.Text,
                packageDescription = descriptionTextBox.Text,
                xamlFile = File.ReadAllText(Path)
            };

            // Convert the data to JSON
            string json = JsonConvert.SerializeObject(data);

            // Send the JSON data to the server
            await SendData(json);
        }

        private async Task SendData(string json)
        {
            try
            {
                Console.WriteLine("Sending data to server...");
                // Create a HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Set the base address of the server
                    client.BaseAddress = new Uri("http://localhost:4000");

                    // Create the HttpContent with JSON data
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync("/post", content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Data sent successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Failed to send data. Error: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

      
    }
}
