using System.Windows;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace RPA_Slayer.Pages
{
    public partial class CloudActivities : MetroWindow
    {
        public CloudActivities()
        {
            InitializeComponent();
            LoadPackages();
        }

        private void LoadPackages()
        {
            string json = @"
            {
              ""objects"": [
                {
                  ""name"": ""Object 1"",
                  ""version"": ""1.0"",
                  ""link"": ""https://example.com/object1"",
                  ""description"": ""This is the first object.""
                },
                {
                  ""name"": ""Object 2"",
                  ""version"": ""2.3"",
                  ""link"": ""https://example.com/object2"",
                  ""description"": ""This is the second object.""
                },
                {
                  ""name"": ""Object 3"",
                  ""version"": ""4.5"",
                  ""link"": ""https://example.com/object3"",
                  ""description"": ""This is the third object.""
                }
              ]
            }";

            var jsonObject = JsonConvert.DeserializeObject<RootObject>(json);

            // Set the DataContext for data binding
            DataContext = jsonObject.objects;
        }
    }

    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
    }

    public class RootObject
    {
        public Package[] objects { get; set; }
    }
}
