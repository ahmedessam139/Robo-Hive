using System;
using System.Activities;
using OpenQA.Selenium;

namespace Browser_Engine
{
    public sealed class Click_Element : CodeActivity
    {
        // Define an activity input argument of type IWebDriver
        public InArgument<IWebDriver> Driver { get; set; }

        // Define an activity input argument of type string
        public InArgument<string> Selector { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the IWebDriver object
            IWebDriver driver = Driver.Get(context);

            // Obtain the runtime value of the Selector input argument
            string selectorString = Selector.Get(context);

            // Create a By object based on the selector string
            By selector = By.CssSelector(selectorString);

            // Find the element to click using the selector
            IWebElement element = driver.FindElement(selector);

            // Click the element
            element.Click();
        }
    }
}
