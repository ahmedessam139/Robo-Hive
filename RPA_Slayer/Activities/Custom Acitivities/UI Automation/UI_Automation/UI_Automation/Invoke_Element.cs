using System;
using System.Activities;
using System.Windows.Automation;

namespace UI_Automation
{
    public sealed class InvokeElement : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> ElementName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Get the automation element by name
            string elementName = ElementName.Get(context);
            AutomationElement element = AutomationElement.RootElement.FindFirst(
                TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, elementName));

            if (element == null)
            {
                throw new ArgumentException($"Element with name '{elementName}' not found.");
            }

            // Get the invoke pattern for the element
            InvokePattern invokePattern = element.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            // Invoke the element
            invokePattern.Invoke();
        }
    }
}
