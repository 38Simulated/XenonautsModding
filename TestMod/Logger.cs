using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TestMod
{
    public static class Logger
    {
        public static void Log(object message)
        {
            UnityEngine.Debug.Log("[TestMod]" + DateTime.Now.ToString() + " " + message.ToString());
        }
    }
}
