using System;
using System.Activities;
using OpenQA.Selenium;

namespace Browser_Engine
{
    public sealed class Close_Browser : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<IWebDriver> Driver { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the WebDriver object
            IWebDriver driver = Driver.Get(context);

            // Close the browser window
            driver.Close();

            // Quit the WebDriver and free up resources
            driver.Quit();
        }
    }
}
