using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Travelport
{
    class Log
    {
        public static void Write(String Message)
        {
            String msg= DateTime.Now.ToString("yyyyMMddHHmmss") + " : " + Message;

            if (!Directory.Exists(LogPath()))
            {
                Directory.CreateDirectory(LogPath());
            }
            using (StreamWriter sw = new StreamWriter(LogFile(),true))
            {
                sw.WriteLine(msg);
            }

        }
        public static void Execption(Exception ex)
        {
            Write(ex.Message);
            if(ex.InnerException != null)
            {
                Write("*** InnerExeception ***");
                Write(ex.InnerException.ToString());
            }
        }
        public static String LogPath()
        {
            String ExeFolder = Utils.ExeFolder();
            return Path.Combine(ExeFolder, "Log");
            
        }
        public static String LogFile()
        {
            String ExeFolder = Utils.ExeFolder();
            return Path.Combine(LogPath(), DateTime.Now.ToString("yyyyMMdd") + ".txt");

        }

    }
}
