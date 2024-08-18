using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer.HTTPModules
{
    interface IHTTPModule
    {
        bool CheckRequestURL(string c);

        byte[] Process(string u, Dictionary<string, string> args);
    }
}
