using System;
using System.Net.Http;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

// Snowy
using SnowLib;
using RepoSaveBackup;

namespace Main
{
    public partial class Main : Form
    {
        /*
        Directorys.

        Repo saves - C:\Users\Snowy\AppData\locallow\semiwork\Repo\saves
        Backup saves - C:\Users\Snowy\Documents\R.E.P.O Saves (Backup)

        */

        private string SaveDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"..\LocalLow\semiwork\Repo\saves");
        private string BackupDirectory => Program.BackupDirectory;

        // Versions
        private const string versionUrl = "https://raw.githubusercontent.com/Snow2Code/R.E.P.O.-Save-Backup/refs/heads/main/version.txt";
        private const string latestReleaseUrl = "https://github.com/Snow2Code/R.E.P.O.-Save-Backup/releases/latest";

        public Main()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            // this.MinimizeBox = false;

            InitializeComponent();


            dataGridViewRepoSaves.Columns.Add("SaveDate", "Date and Time");
            dataGridViewRepoSaves.Columns.Add("FolderName", "Folder Name");
            dataGridViewBackups.Columns.Add("SaveDate", "Date and Time");
            dataGridViewBackups.Columns.Add("FolderName", "Folder Name");

            dataGridViewRepoSaves.ReadOnly = true;
            dataGridViewRepoSaves.AllowUserToAddRows = false;
            dataGridViewRepoSaves.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            LoadSaveList();
            ProgramLoaded();
        }

        private async void ProgramLoaded()
        {
            Version localVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version remoteVersion = await GetRemoteVersion();

            if (remoteVersion == null)
            {
                Console.WriteLine("Meow! Cannot get latest github version!");
                //MessageBox.Show("Could not check for updates. You're on your own.", "Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (remoteVersion > localVersion)
            {
                Console.WriteLine("Meow! Version out of date!");

                DialogResult result = MessageBox.Show(
                    $"A new version is available.\nYou're on {localVersion}, and the latest is {remoteVersion}.\n\nIt is recommended to use the latest version. But not a requirement.",
                    "Update Available",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = latestReleaseUrl,
                        UseShellExecute = true
                    });
                    
                    // Excuse me, we don't want to forcefully close the app! What if the user is too lazy to download the latest version??
                    //Application.Exit();
                } else {
                    // Continue with current version
                }
            } else {
                // Everything's up to date
            }
        }

        private async Task<Version> GetRemoteVersion()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string versionString = await client.GetStringAsync(versionUrl);
                    return new Version(versionString.Trim());
                } catch {
                    return null;
                }
            }
        }
        
        private void LoadSaveList()
        {
            dataGridViewRepoSaves.Rows.Clear();
            dataGridViewBackups.Rows.Clear();

            // This shouldn't save just repo saves because I added the backup saves grid thing.
            Logger.Write("Loading saves");

            if (!Directory.Exists(SaveDirectory))
                return;

            var dirs_repo = Directory.GetDirectories(SaveDirectory).Select(Path.GetFileName).OrderByDescending(name => name);
            var dirs_backup = Directory.GetDirectories(BackupDirectory).Select(Path.GetFileName).OrderByDescending(name => name);

            // R.E.P.O. Saves Grid View
            foreach (var dirName in dirs_repo)
            {
                // Try to extract the date/time
                string dateTimeFormatted = "Invalid Format";

                if (dirName.StartsWith("REPO_SAVE_"))
                {
                    string timestampPart = dirName.Substring("REPO_SAVE_".Length);

                    if (DateTime.TryParseExact(timestampPart, "yyyy_MM_dd_HH_mm_ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime parsedTime))
                    {
                        // Nice readable format like "May 19, 2025 11:03:06 PM"
                        dateTimeFormatted = parsedTime.ToString("f");
                    }
                }

                dataGridViewRepoSaves.Rows.Add(dateTimeFormatted, dirName);
            }

            // Backup Saves
            foreach (var dirName in dirs_backup)
            {
                // Try to extract the date/time
                string dateTimeFormatted = "Invalid Format";

                if (dirName.StartsWith("REPO_SAVE_"))
                {
                    string timestampPart = dirName.Substring("REPO_SAVE_".Length);

                    if (DateTime.TryParseExact(timestampPart, "yyyy_MM_dd_HH_mm_ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime parsedTime))
                    {
                        // Nice readable format like "May 19, 2025 11:03:06 PM"
                        dateTimeFormatted = parsedTime.ToString("f");
                    }
                }

                dataGridViewBackups.Rows.Add(dateTimeFormatted, dirName);
            }
        }


        private void buttonBackup_Click(object sender, EventArgs e)
        {
            var confirm1 = MessageBox.Show("Are you SURE you want to restore the backup?\nThis cannot be undone.", "Restore Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm1 != DialogResult.Yes) return;

            var confirm2 = MessageBox.Show("Seriously, this will overwrite all existing saves.\nNo way to undo it. Proceed?", "Final Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm2 != DialogResult.Yes) return;

            // Restore: Copy from backup to save folder
            try
            {
                CopyDirectory(BackupDirectory, SaveDirectory);
                MessageBox.Show("Restore complete. Thank Snowy later.");
                Logger.Write("Restore Backup Saves (override repo saves) successful.");
                LoadSaveList(); // Refresh
            }
            catch (Exception ex)
            {
                Logger.Write("Restore Backup Saves (override repo saves) failed, error message: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonOpenSaves_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", SaveDirectory);
        }

        private void buttonOpenBackup_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", BackupDirectory);
        }

        private void CopyDirectory(string source, string target)
        {
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, target));
            }

            foreach (string filePath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                string destFile = filePath.Replace(source, target);
                File.Copy(filePath, destFile, true);
            }
        }

        private void buttonBackupNow_Click(object sender, EventArgs e)
        {
            try
            {
                CopyDirectory(SaveDirectory, BackupDirectory);
                MessageBox.Show("Backup created");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void repoSaves_Label_Click(object sender, EventArgs e)
        {

            Process.Start("explorer.exe", SaveDirectory);
        }

        private void backupSaves_Label_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", BackupDirectory);
        }
    }
}
