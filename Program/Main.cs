using System;
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
        private string SaveDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"..\LocalLow\semiwork\Repo\saves");

        private string BackupDirectory => Program.BackupDirectory;

        public Main()
        {
            // Shit
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
        }
        private void LoadSaveList()
        {
            dataGridViewRepoSaves.Rows.Clear();
            dataGridViewBackups.Rows.Clear();

            Logger.Write("Loading saves (not backups, current saves)");

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
