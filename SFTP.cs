using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Travelport
{
    class SFTP
    {
        public static void SendFileToBSPlink(String FileName, String SFTPTarget, String SFXCL_EXE)
        {
            Log.Write("Starting new process SendFileToBSPlink");

            String args;
            Process p = new Process();

            try
            {
                p.StartInfo.FileName = SFXCL_EXE;

                if (!File.Exists(p.StartInfo.FileName))
                {
                    Log.Write("VanDyke Software SFXCL not found, File not sent : " + SFXCL_EXE);
                    throw new System.ArgumentException("VanDyke Software SFXCL not found, File(s) not sent", SFXCL_EXE);
                }

                args = "/Overwrite always /AcceptHostKeys /DefaultType binary /Log " + "\"" + Log.LogFile() + "\" " + "\"" + FileName + "\" " + SFTPTarget;

                p.StartInfo.Arguments = args;

                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                p.Start();

                Boolean exitcode = p.WaitForExit(120 * 1000);

                p.Close();
                p.Dispose();

                Log.Write("Send File To BSP Link Complete with an exit code of " + exitcode.ToString());
            }
            catch (Exception ex)
            {
                throw new SystemException("Send File To BSP Link Failed ", ex);
            }

        }
    }
}