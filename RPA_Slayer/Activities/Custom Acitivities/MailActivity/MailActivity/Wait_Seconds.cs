using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailActivity
{
    public sealed class Wait_Seconds : CodeActivity
    {
        public InArgument<int> Seconds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int seconds = Seconds.Get(context);

            // Wait for the specified number of seconds
            Thread.Sleep(seconds * 1000);
        }
    }

}
