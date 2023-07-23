using System;
using System.Activities;
using OpenQA.Selenium;

namespace Browser_Engine
{
    public sealed class Multi_Click : CodeActivity
    {
        // Define an activity input argument of type IWebDriver
        public InArgument<IWebDriver> Driver { get; set; }

        // Define an activity input argument of type string
        public InArgument<string> Selector { get; set; }

        // Define an activity input argument of type int
        public InArgument<int> NumberOfClicks { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the IWebDriver object
            IWebDriver driver = Driver.Get(context);

            // Obtain the runtime value of the Selector input argument
            string selectorString = Selector.Get(context);

            // Create a By object based on the selector string
            By selector = By.CssSelector(selectorString);

            // Obtain the runtime value of the NumberOfClicks input argument
            int numberOfClicks = NumberOfClicks.Get(context);

            // Find the element to click using the selector
            IWebElement element = driver.FindElement(selector);

            // Perform multiple clicks
            for (int i = 0; i < numberOfClicks; i++)
            {
                element.Click();
            }
        }
    }
}
