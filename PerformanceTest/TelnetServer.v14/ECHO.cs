using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;

namespace TelnetServer
{
    public class ECHO : StringCommandBase
    {
        public override void ExecuteCommand(AppSession session, StringCommandInfo commandInfo)
        {
            session.SendResponse(commandInfo.Data);
        }
    }
}
