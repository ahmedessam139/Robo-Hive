using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace RPA_Slayer.Helpers
{
    public class orchestratorCommunications
    {
        public string packageName { get; set; }
        public string machineName { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string xamlPath { get; set; }


        //set machinName to fixed value "test"
        
        
        public async Task sendToOrc()
        {
            machineName = "test";
            using (var httpClient = new HttpClient())
            {
                //var apiUrl = "http://localhost:8000/api/upload";
                var apiUrl = "http://35.242.197.187/api/packages/create";


                var payload = new
                {
                    packageName = packageName,
                    xamlFile = Convert.ToBase64String(File.ReadAllBytes(xamlPath)),
                    //machineName = machineName,
                    date = date,
                    time = time,
                    //xamlFile = Convert.ToString(File.ReadAllText(xamlPath)),
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                //print content to console
                Console.WriteLine(jsonPayload);

                try
                {
                    var response = await httpClient.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Data has been successfully sent to the server.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"An error occurred while sending data to the server. Error message: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        public void ShowForm(string WorkflowFilePath )
        {
            var form = new Form();
            form.Icon = Properties.Resources.Logo_icon;
            form.Text = "Send To Orc";
            form.Width = 500;
            form.Height = 300;
            form.StartPosition = FormStartPosition.CenterScreen;



            var nameLabel = new Label();
            nameLabel.Text = "Package Name:";
            nameLabel.Left = 20;
            nameLabel.Top = 20;
            form.Controls.Add(nameLabel);

            var nameTextBox = new TextBox();
            nameTextBox.Left = 200;
            nameTextBox.Top = 20;
            form.Controls.Add(nameTextBox);

            var dateLabel = new Label();
            dateLabel.Text = "Date:";
            dateLabel.Left = 20;
            dateLabel.Top = 50;
            form.Controls.Add(dateLabel);

            var datePicker = new DateTimePicker();
            datePicker.Format = DateTimePickerFormat.Short;
            datePicker.Left = 200;
            datePicker.Top = 50;
            form.Controls.Add(datePicker);

            var timeLabel = new Label();
            timeLabel.Text = "Time:";
            timeLabel.Left = 20;
            timeLabel.Top = 80;
            form.Controls.Add(timeLabel);

            var timePicker = new DateTimePicker();
            timePicker.Format = DateTimePickerFormat.Time;
            timePicker.ShowUpDown = true;
            timePicker.Left = 200;
            timePicker.Top = 80;
            form.Controls.Add(timePicker);

            var xamlButton = new Button();
            xamlButton.Text = "Select XAML File";
            xamlButton.Left = 20;
            xamlButton.Top = 110;
            form.Controls.Add(xamlButton);

            var xamlLabel = new Label();
            xamlLabel.Left = 120;
            xamlLabel.Top = 110;
            xamlLabel.AutoSize = true;
            xamlLabel.Text = WorkflowFilePath;
            xamlPath = xamlLabel.Text;
            form.Controls.Add(xamlLabel);

            xamlButton.Click += (sender, e) =>
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "XAML files (*.xaml) | *.xaml";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xamlPath = openFileDialog.FileName;
                        xamlLabel.Text = xamlPath;
                    }
                }
            };

            

            var sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Left = 20;
            sendButton.Top = 170;
            form.Controls.Add(sendButton);

            sendButton.Click += async (sender, e) =>
            {
                packageName = nameTextBox.Text;
                DateTime dateTime = new DateTime(
                    datePicker.Value.Year,
                    datePicker.Value.Month,
                    datePicker.Value.Day,
                    timePicker.Value.Hour,
                    timePicker.Value.Minute,
                    timePicker.Value.Second);

                date = dateTime.ToString("yy-MM-dd");
                time = dateTime.ToString("HH:mm:ss");
                await sendToOrc();
            };  

            form.ShowDialog();

        }



    }
}
        

