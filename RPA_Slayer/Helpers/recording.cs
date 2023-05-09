using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;
using System.Xml.Linq;
using System.IO;

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
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, _rootElement, TreeScope.Descendants, OnWindowOpened);
            Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, _rootElement, TreeScope.Descendants, OnWindowClosed);
            Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent, _rootElement, TreeScope.Descendants, OnInvoked);

            Console.WriteLine("Recording started...");



        }

        private void OnWindowOpened(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;
            if (element != null)
            {
                _recordedActions.Add(element);
                Console.WriteLine("Window opened: " + element.Current.Name);
                //open window again 
                AutomationElement window = _rootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, element.Current.Name));

            }
        }

        private void OnWindowClosed(object sender, AutomationEventArgs e)
        {
            AutomationElement element = sender as AutomationElement;
            if (element != null)
            {
                _recordedActions.Add(element);
                Console.WriteLine("Window closed: " + element.Current.Name);
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
                Thread.Sleep(5000);
               
            }
        }



        public string StopRecord(string filePath)
        {
            // Read the content of the file
            string content = File.ReadAllText(filePath);

            // Split the content into lines
            string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Compute the index of the line before the last 2 lines
            int insertionIndex = Math.Max(0, lines.Length - 2);

            // Insert the elements before the line at the insertion index
            lines[insertionIndex] = string.Join(Environment.NewLine, _elementNames) + Environment.NewLine + lines[insertionIndex];

            // Join the lines back into a single string
            content = string.Join(Environment.NewLine, lines);

            // Save the modified content back to the file
            File.WriteAllText(filePath, content);

            // Return the updated content
            return content;
        }





        private void ReplayRecordedActions()
        {
            // TODO: Replay the recorded actions automatically
        }
    }
}
