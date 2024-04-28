using CSharpGameServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGameServer.PC
{
    public class PC
    {
        private ulong sessionId = 0;
        private ulong pcId = 0;

        PC(ulong inSessionId)
        {
            sessionId = inSessionId;
        }

        public void OnPCIdCompleted(ulong inPCId)
        {
            pcId = inPCId;
        }
    }
}
