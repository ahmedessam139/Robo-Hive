using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace RPA_Slayer
{
    public partial class LoginWindow : MetroWindow
    {
        private const string ServerUrl = "http://34.155.103.216.nip.io:8080";
        private const string TokenFilePath = "tokens.json";
        private const string KC_CLIENT_ID = "register";
        private const string KC_CLIENT_SECRET = "Gc6mjXoPJPuUZ5LQci1Daczcsnnnng5U";

        public LoginWindow()
        {
            InitializeComponent();
            CheckTokenAndNavigate();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var payload = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", KC_CLIENT_ID },
                { "client_secret", KC_CLIENT_SECRET },
                { "username", usernameTextBox.Text },
                { "password", passwordBox.Password }
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var content = new FormUrlEncodedContent(payload);


                    var response = await client.PostAsync($"{ServerUrl}/realms/orch/protocol/openid-connect/token", content);
                    Console.WriteLine("Success");
                    Console.WriteLine(response.Content.ReadAsStringAsync());
                    Thread.Sleep(5000);

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
            if (tokenData != null)
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
                { "accessToken", tokenData.access_token },
                { "refreshToken", tokenData.access_token }
                
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
            HttpContext.Current.Response.Redirect("http://34.155.103.216.nip.io:8080/realms/orch/login-actions/registration?client_id=orch&tab_id=odPdEq7K_cY");


        }
    }

    public class TokenData
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }

    }
}
