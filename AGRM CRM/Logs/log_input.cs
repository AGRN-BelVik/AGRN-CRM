using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AGRM_CRM.Logs
{
    internal class log_input
    {
        public void InputLog(string txt, string type)
        {
            string n = ""; string txtType = "stable";
            string Date = DateTime.Now.ToString("dd_MM_yyyy");
            try{if (File.ReadAllText($"Log\\Logs_{Date}.txt").Length > 0) n = "\n";}
            catch{}
            if (type == "Error") txtType = "ERROR_";
            File.AppendAllText($"Log\\Logs_{Date}.txt", $"{n}{DateTime.Now} |_{txtType}_| {txt}");
        }
    }
}
