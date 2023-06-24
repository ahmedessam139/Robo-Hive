using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.IO;
using System.Windows.Input;

namespace RPA_Slayer.Helpers
{
    public class recording
    {
        private AutomationElement _rootElement;
        private List<AutomationElement> _recordedActions = new List<AutomationElement>();
        private List<string> _elementNames = new List<string>();

        public void StartRecord(string workflowFilePath)
        {
            // Find the root element of the desktop
            _rootElement = AutomationElement.RootElement;

            // Subscribe to UI automation events 
            Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent, _rootElement, TreeScope.Descendants, OnInvoked);
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);

            Console.WriteLine("Recording started...");
        }

        private void recordKeyPressing(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Key pressed: {0}", e.Key);
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            try
            {
                AutomationElement focusedElement = sender as AutomationElement;

                if (focusedElement != null)
                {
                    Console.WriteLine("Focused element AutomationId------>: {0}", focusedElement.Current.AutomationId);
                    Console.WriteLine("Focused element Name------>: {0}", focusedElement.Current.Name);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in OnFocusChanged: {0}", ex.Message);
            }
        }



        private void OnInvoked(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;
            if (element != null)
            {
                _recordedActions.Add(element);
                // Save the element name in XML format to the list
                string xmlElementName = string.Format("<u:InvokeElement ElementName=\"{0}\" />", element.Current.Name);
                _elementNames.Add(xmlElementName);
                // Print the accumulated list of element names
                Console.WriteLine(string.Join(Environment.NewLine, _elementNames));
                Console.WriteLine(_elementNames);
                // CLICK ON ELEMENT
                Thread.Sleep(200);
            }
        }

       

        public string StopRecord(string filePath)
        {
            //remove All event handlers
            Automation.RemoveAllEventHandlers();
            // Read the content of the file
            string content = File.ReadAllText(filePath);
            // Split the content into lines
            string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            // Compute the index of the line before the last 2 lines
            int insertionIndex = Math.Max(0, lines.Length - 2);            
            // Insert the elements before the line at the insertion index
            lines[insertionIndex] = string.Join("" + Environment.NewLine, _elementNames) + Environment.NewLine + lines[insertionIndex];
            //remove all elemnts (_elementNames)
            _elementNames.Clear();
            // Join the lines back into a single string
            content = string.Join(Environment.NewLine, lines);
            // Save the modified content back to the file
            File.WriteAllText(filePath, content);

            // Return the updated content
            return content;
        }

    }
}
