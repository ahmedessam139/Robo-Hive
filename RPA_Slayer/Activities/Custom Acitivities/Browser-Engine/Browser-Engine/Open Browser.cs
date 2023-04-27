using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Threading;

namespace Browser_Engine
{
    public sealed class Open_Browser : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> Url { get; set; }

        // Define an activity input argument of type string
        public InArgument<string> DriverPath { get; set; }

        // Define an activity output argument of type string
        public OutArgument<string> BrowserId { get; set; }

        // Define a property to store the WebDriver object
        private IWebDriver driver;

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Url input argument
            string url = Url.Get(context);

            // Obtain the runtime value of the driver path input argument
            string driverPath = DriverPath.Get(context);

            // Create a new instance of EdgeDriver with the specified driver path
            this.driver = new EdgeDriver(driverPath);

            // Navigate to the specified URL
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

            // Generate a unique identifier for the browser instance
            string browserId = Guid.NewGuid().ToString();

            // Output the browser identifier
            this.BrowserId.Set(context, browserId);
        }
    }
}
