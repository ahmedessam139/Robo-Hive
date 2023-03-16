using System.Activities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shortcuts
{
    public partial class Clippy
    {
        private Button currentlySelectedButton;
        private string selectedAction;

        public Clippy()
        {
            InitializeComponent();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick((Button)sender);
            selectedAction = ((Button)sender).Content.ToString();
            InArgument<string> appPath = new InArgument<string>(selectedAction);
            ModelItem.Properties["TextToType"].SetValue(appPath);

            // Perform the "cut" action
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick((Button)sender);
            selectedAction = ((Button)sender).Content.ToString();
            InArgument<string> appPath = new InArgument<string>(selectedAction);
            ModelItem.Properties["TextToType"].SetValue(appPath);

            // Perform the "copy" action
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick((Button)sender);
            selectedAction = ((Button)sender).Content.ToString();
            InArgument<string> appPath = new InArgument<string>(selectedAction);
            ModelItem.Properties["TextToType"].SetValue(appPath);

            // Perform the "paste" action
        }

        private void HandleButtonClick(Button clickedButton)
        {
            // Reset the color of the previously selected button
            if (currentlySelectedButton != null)
            {
                currentlySelectedButton.Background = new SolidColorBrush(Color.FromRgb(144, 240, 146));
            }

            // Set the color of the clicked button
            clickedButton.Background = new SolidColorBrush(Colors.Yellow);

            // Update the currently selected button
            currentlySelectedButton = clickedButton;
        }
    }
}
