using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RepoSaveBackup;

// Snowy
//using SnowLib;
//using SnowLib;

namespace Main
{
    internal static class Program
    {
        public static readonly string BackupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "R.E.P.O Saves (Backup)");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SnowLib.dll");

            if (!File.Exists(dllPath))
            {
                Logger.WriteErrorAndExit("Cannot find SnowLib.dll, closing.");
            }

            Logger.Write("SnowLib.dll found, continuing.");

            EnsureBackupDirectory();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Main());
        }


        static void EnsureBackupDirectory()
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    Directory.CreateDirectory(BackupDirectory);
                    Logger.Write("Created backup directory.");
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"Failed to create backup directory: {ex.Message}");
            }
        }
    }
}
