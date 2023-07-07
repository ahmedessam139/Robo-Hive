using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public string tokensPath { get; set; }

        public Orc_Config(string path)
        {
            InitializeComponent();
            Path = path;
            xamlLabel.Text = Path;
            tokensPath = "tokens.json";
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an object with the data from the UI
            var data = new
            {
                packageName = nameTextBox.Text,
                description = descriptionTextBox.Text,
                xamlFile = Convert.ToBase64String(File.ReadAllBytes(Path)),
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

                // Read the tokens JSON file
                string tokensJson = File.ReadAllText(tokensPath);

                // Parse the JSON to retrieve the access token
                var tokensData = JsonConvert.DeserializeObject<dynamic>(tokensJson);
                string bearerToken = tokensData.AccessToken;

                // Create a HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Set the base address of the server
                    client.BaseAddress = new Uri("http://34.155.103.216");

                    // Add bearer token to the HttpClient headers
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    // Create the HttpContent with JSON data
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync("/api/packages/create/", content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Data sent successfully!");
                        this.Close();

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
