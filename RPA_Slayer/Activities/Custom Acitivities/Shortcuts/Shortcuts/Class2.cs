using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;





namespace Shortcuts
{

    public class msgbox : CodeActivity
    {
        [Category("output")]
        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);


        protected override void Execute(CodeActivityContext context)
        {
            MessageBox(IntPtr.Zero, "hello", "Message", 0);
            //throw new NotImplementedException();
        }
    }
}
