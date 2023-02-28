using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Windows.Forms;

namespace IO_Modules
{

    public class DisplayMsgBox : CodeActivity
    {
        [Category("Input")] //type input
        [RequiredArgument] //to force the user to enter input
        public InArgument<string> InputText { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Get the input text from the activity property
            string input = InputText.Get(context); 

            // Display the entered input in a message box
            MessageBox.Show(input, "Entered Input");
        }
    }
}
