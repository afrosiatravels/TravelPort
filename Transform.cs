using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Travelport
{
    class Transform
    {
        String baseFolder;
        String InFolder;
        String OutFolder;
        String ArchiveFolder;

        public void run()
        {
            try {
                Log.Write("-----------------------------------------------------------------------------------");
                CreateFolders();
                List<Setting> settings = new List<Setting>();
                var Files = Directory.GetFiles(Settings.Get("SourceFileFolder"), Settings.Get("SourceFileMask"));
                String OutFile = "";
                Boolean bSendFile = Settings.Get("SendToBSPLink").ToUpper().Equals("TRUE");
                ArchiveFolder = Path.Combine(baseFolder, "Archive");
                foreach (String s in Files)
                {
                    OutFile = Process(s);
                    if (bSendFile)
                    {
                        SendFile(OutFile);
                    }
                    else
                    {
                        Log.Write("SendToBSPLink not set to true, file not sent to BSPLink");
                    }
                    FileInfo fi = new FileInfo(OutFile);
                    Log.Write("Archiving " + OutFile);
                    FileMove(fi, ArchiveFolder);
                }
            }
            catch(Exception ex)
            {
                Log.Execption(ex);
            }
            finally
            {
                Log.Write("==================================================================================");
            }
        }
        private void FileMove(FileInfo Fi, String TargetFolder)
        {
            try { 
            String TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            String TargetFile = Path.Combine(TargetFolder, Fi.Name);
            if (File.Exists(TargetFile)){
                File.Move(TargetFile, TargetFile + "_" + TimeStamp);
            }
            Fi.MoveTo(Path.Combine(TargetFolder, Fi.Name));
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
        }
        private void CreateFolders()
        {
            Log.Write("Checking Folder Exits and Creating if required");

            baseFolder = Settings.Get("BaseProcessingFolder");
            InFolder = Path.Combine(baseFolder, "In");
            OutFolder = Path.Combine(baseFolder, "Out");
            ArchiveFolder = Path.Combine(baseFolder, "Archive");

            try
            {
                if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);
                if (!Directory.Exists(InFolder)) Directory.CreateDirectory(InFolder);
                if (!Directory.Exists(OutFolder)) Directory.CreateDirectory(OutFolder);
                if (!Directory.Exists(ArchiveFolder)) Directory.CreateDirectory(ArchiveFolder);
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
           


            Log.Write("Checking Folder Exits completed");

        }

        private String Process(string s)
        {
            String OutFileName = "";
            try
            { 
            Log.Write("Getting file: " + s);
            FileInfo fi = new FileInfo(s);
            FileMove(fi, InFolder);
            Log.Write("Processing : " + Path.Combine(InFolder, fi.Name));
            String Countrycode = Settings.Get("CountryCode");
            OutFileName = Settings.Get("OutputFileName");
            OutFileName = OutFileName.Replace("[CC]", Countrycode);
            OutFileName = OutFileName.Replace("[yyyyMMdd]", DateTime.Now.ToString("yyyyMMdd"));
            int counter = 0;
            String sCounter = counter.ToString().PadLeft(3, '0');
            String OutFileTemplate = OutFileName;
            OutFileName = OutFileName.Replace("[999]", sCounter);
            while (File.Exists(Path.Combine(ArchiveFolder, OutFileName)))
            {
                counter++;
                sCounter = counter.ToString().PadLeft(3, '0');
                OutFileName = OutFileTemplate.Replace("[999]", sCounter);
            }
            OutFileName = Path.Combine(OutFolder, OutFileName);
            Log.Write("Input File is " + Path.Combine(InFolder, fi.Name));
            Log.Write("Output File is " + OutFileName);
            Log.Write("Starting conversion");
            using (StreamReader sr = new StreamReader(Path.Combine(InFolder, fi.Name)))
            {
                using (StreamWriter sw = new StreamWriter(OutFileName, false))
                {
                    string line;
                    int lineno = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineno++;
                        if (lineno == 1)
                        {
                            line = line.Replace("WSPN", "WEBL");
                            line = line.Replace("TEST", "PROD");
                        }
                        else
                        {
                            if (line.Length > 248)
                            {
                                    if (line.StartsWith("2"))
                                    {
                                        line = line.Substring(0, 246) + Countrycode + line.Substring(248);
                                    }
                            }
                            else
                            {
                                    line = "";
                            }
                        }
                        if(line.Length > 10)
                            {
                                sw.WriteLine(line);
                            }
                       

                    }
                }
            }
            Log.Write("Finished " +  s);
            FileMove(fi, ArchiveFolder);
            Log.Write("Archived " + s);
            
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
            return OutFileName;
        }

        private void SendFile(String FilePath)
        {
            try { 
            Log.Write("Sending " + FilePath);
            
            String SFTPVanDyke = Settings.Get("SFTPVanDyke");
            String SFTPTarget = Settings.Get("SFTPTarget");

            SFTP.SendFileToBSPlink(FilePath, SFTPTarget, SFTPVanDyke);
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
        }

   }
}
