using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;

namespace TelnetServer
{
    public class TestSession : AppSession<TestSession>
    {
        public override void HandleUnknownCommand(StringCommandInfo cmdInfo)
        {
            //Console.WriteLine("Unknow command: '{0}'", cmdInfo.Key);
        }
    }
}
