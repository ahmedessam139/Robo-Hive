using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace RPA_Slayer
{
    public partial class LoginWindow : Window
    {
        private const string ServerUrl = "http://localhost:3000";
        private const string TokenFilePath = "tokens.json";

        public LoginWindow()
        {
            InitializeComponent();
            CheckTokenAndNavigate();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var payload = new Dictionary<string, string>
            {
                { "email", usernameTextBox.Text },
                { "password", passwordBox.Password }
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{ServerUrl}/auth/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonConvert.DeserializeObject<TokenData>(responseContent);
                        SaveTokens(tokenData);

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                ClearPassword();
            }
        } 

        private void CheckTokenAndNavigate()
        {
            var tokenData = RetrieveTokens();
            if (tokenData != null && ValidateTokens(tokenData))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                this.Close();
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }

        private bool ValidateTokens(TokenData tokenData)
        {
            var payload = new Dictionary<string, string>
            {
                { "accessToken", tokenData.AccessToken },
                { "refreshToken", tokenData.RefreshToken }
                
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = client.PostAsync($"{ServerUrl}/auth/valid-token", content).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Token validation failed: " + ex.Message);
                return false;
            }
        }

        private void SaveTokens(TokenData tokenData)
        {
            try
            {
                var jsonTokens = JsonConvert.SerializeObject(tokenData);
                File.WriteAllText(TokenFilePath, jsonTokens);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save tokens: " + ex.Message);
            }
        }

        private TokenData RetrieveTokens()
        {
            try
            {
                if (File.Exists(TokenFilePath))
                {
                    var jsonTokens = File.ReadAllText(TokenFilePath);
                    var tokenData = JsonConvert.DeserializeObject<TokenData>(jsonTokens);
                    return tokenData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve tokens: " + ex.Message);
            }

            return null;
        }

        private void ClearPassword()
        {
            passwordBox.Clear();
        }

       

        private void forgetPassword_click(object sender, RoutedEventArgs e)
        {

        }
        private void signUp_click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class TokenData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public string name { get; set; }
    }
}
