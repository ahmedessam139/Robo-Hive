using System;
using System.Activities;
using OpenQA.Selenium;

namespace Browser_Engine
{
    public sealed class Get_Element_Text : CodeActivity
    {
        // Define an activity input argument of type IWebDriver
        public InArgument<IWebDriver> Driver { get; set; }

        // Define an activity input argument of type string
        public InArgument<string> SelectorText { get; set; }

        // Define an activity output argument of type string
        public OutArgument<string> Text { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the WebDriver object
            IWebDriver driver = Driver.Get(context);

            // Obtain the runtime value of the selector text input argument
            string selectorText = SelectorText.Get(context);

            // Find the element using the selector text
            IWebElement element = driver.FindElement(By.CssSelector(selectorText));

            // Get the text of the element
            string text = element.Text;

            // Output the text
            Text.Set(context, text);
        }
    }
}
