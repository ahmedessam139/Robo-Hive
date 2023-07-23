using System;
using System.Activities;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Browser_Engine
{
    public sealed class Iframe_download : CodeActivity
    {
        // Define an activity input argument of type IWebDriver
        public InArgument<IWebDriver> Driver { get; set; }

        // Define an activity input argument of type IWebElement (representing the iframe element)
        public InArgument<IWebElement> IframeElement { get; set; }

        // Define an activity input argument of type string (file path to save HTML content)
        public InArgument<string> FilePath { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the IWebDriver object
            IWebDriver driver = Driver.Get(context);

            // Obtain the runtime value of the IWebElement (iframe) input argument
            IWebElement iframe = IframeElement.Get(context);

            // Obtain the runtime value of the file path input argument
            string filePath = FilePath.Get(context);

            // Switch to the iframe
            driver.SwitchTo().Frame(iframe);

            // Get the page source of the iframe
            string downloadedContent = driver.PageSource;

            // Switch back to the main page (outside the iframe)
            driver.SwitchTo().DefaultContent();

            // Save the downloaded page content to the specified file path
            File.WriteAllText(filePath, downloadedContent);
        }
    }
}
