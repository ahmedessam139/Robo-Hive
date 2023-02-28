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
    public class KeyboardControl : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string text = Text.Get(context);
            SendKeys.SendWait(text);
        }
    }
}
