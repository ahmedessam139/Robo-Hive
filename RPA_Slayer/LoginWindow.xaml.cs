using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA_Slayer
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if username & password are correct
            if (usernameTextBox.Text == "Hoda" && passwordBox.Password == "sad")
            {
                // Navigate to MainWindow.xaml
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // Close the LoginWindow.xaml
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect username or password.");
            }
        }


    }
}
