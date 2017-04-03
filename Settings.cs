using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelport
{
    class Settings
    {
        public static String Get(String key)
        {
            String rtv = " ";

            try
            {
                List<Setting> Settings = new List<Setting>();
                Settings = LoadSettings();

                foreach (Setting s in Settings)
                {
                    if (s.Key == key)
                    {
                        rtv = s.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
            return rtv;

        }

        private static List<Setting> LoadSettings()
        {
            List<Setting> Settings = new List<Setting>();
            String ExeFolder = Utils.ExeFolder();
            String SettingsFolder = Path.Combine(ExeFolder, "Settings");
            String Settingsfile = Path.Combine(SettingsFolder, "Settings.txt");

            try
            {
                if (!Directory.Exists(SettingsFolder))
                {
                    Directory.CreateDirectory(SettingsFolder);
                }
                if (!File.Exists(Settingsfile))
                {
                    CreateSettingFile(Settingsfile);
                }
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
            return LoadSettingFile(Settingsfile);
        }

        private static List<Setting> LoadSettingFile(string settingsfile)
        {
            List<Setting> settings = new List<Setting>();

            try
            {
                using (StreamReader sr = new StreamReader(settingsfile))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().Length > 0)
                        {
                            if (line.Split('=').Length >= 2 && !line.StartsWith("#"))
                            {
                                settings.Add(new Setting(line.Split('=')[0], line.Substring(line.IndexOf("=") + 1)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
            return settings;
        }

        private static void CreateSettingFile(String Settingsfile)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Settingsfile))
                {
                    sw.WriteLine(@"# Settings File");
                    sw.WriteLine(@"# Lines beginning with a # will be ignored");
                    sw.WriteLine(@"# Settings are in the form of a Key=Value");
                    sw.WriteLine(@"# i.e. MyPath=C:\AnyFolderPath");
                    sw.WriteLine(@"");
                    sw.WriteLine(@"SourceFileFolder=" + Path.Combine(Utils.ExeFolder(), "FTPIN"));
                    sw.WriteLine(@"SourceFileMask=*.txt");
                    sw.WriteLine(@"BaseProcessingFolder=" + Path.Combine(Utils.ExeFolder(), "Process"));
                    sw.WriteLine(@"OutputFileName=[CC]ewWEBLINK_[yyyyMMdd]_1400_[999]");
                    sw.WriteLine(@"CountryCode=IE");
                    sw.WriteLine(@"SendToBSPLink=false");
                    sw.WriteLine(@"SFTPTarget=sftp://user:password@127.0.0.1:2/Work/_Drop/LiatTest");
                    sw.WriteLine(@"SFTPVanDyke=C:\Program Files (x86)\VanDyke Software\SecureFX\sfxcl.exe");
                }
            }
            catch (Exception ex)
            {
                Log.Execption(ex);
            }
        }

    }
}