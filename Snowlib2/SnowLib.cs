using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowLib
{
    public class SaveEntry
    {
        public string DateTimeFormatted { get; set; }
        public string FolderName { get; set; }
    }

    public class SaveManager
    {
        private string SaveDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"..\LocalLow\semiwork\Repo\saves");

        private string BackupDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "R.E.P.O Saves (Backup)");

        public void EnsureBackupFolderExists()
        {
            if (!Directory.Exists(BackupDirectory))
                Directory.CreateDirectory(BackupDirectory);
        }

        public List<SaveEntry> GetSaveList()
        {
            List<SaveEntry> saves = new List<SaveEntry>();

            if (!Directory.Exists(SaveDirectory))
                return saves;

            foreach (string path in Directory.GetDirectories(SaveDirectory))
            {
                string folderName = Path.GetFileName(path);
                string timestamp = folderName.Replace("REPO_SAVE_", "");
                string formatted = "Invalid Format";

                if (DateTime.TryParseExact(timestamp, "yyyy_MM_dd_HH_mm_ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    formatted = dt.ToString("f");
                }

                saves.Add(new SaveEntry
                {
                    DateTimeFormatted = formatted,
                    FolderName = folderName
                });
            }

            return saves;
        }

        public void BackupSaves()
        {
            CopyDirectory(SaveDirectory, BackupDirectory);
        }

        public void RestoreSaves()
        {
            CopyDirectory(BackupDirectory, SaveDirectory);
        }

        public void OpenSaveFolder()
        {
            Process.Start("explorer.exe", SaveDirectory);
        }

        public void OpenBackupFolder()
        {
            Process.Start("explorer.exe", BackupDirectory);
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, targetDir));
            }

            foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceDir, targetDir), true);
            }
        }
    }

    public class SnowLib
    {
        public void Output(string message)
        {
            //Console.WriteLine(message);
            Debug.WriteLine(message); // Swapped to .NET Standard 2.0, so we have to use Debug.


            //Debug.WriteLine(message);
            //MessageBox.Show(message, "SnowLib");
        }
        public void test()
        {
            Output("Test. Meow!");
        }
    }
}
