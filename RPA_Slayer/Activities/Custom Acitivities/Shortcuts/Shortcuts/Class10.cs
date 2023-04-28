using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Windows.Forms;


namespace Shortcuts
{
    public class SaveAs : CodeActivity
    {
        //[Category("output")]
        protected override void Execute(CodeActivityContext context)
        {
            System.Threading.Thread.Sleep(1000);

            // Simulate the Ctrl+A keyboard shortcut to select all text in the active window
            SendKeys.SendWait("^(+s)");
            //throw new NotImplementedException();
        }
    }
}
