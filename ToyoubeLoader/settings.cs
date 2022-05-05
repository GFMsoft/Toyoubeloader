using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToyoubeLoader
{
     
    public partial class settings : Form
    {
        private readonly Main? youtube_Loader;

        public settings(Main aufrufer)
        {
            youtube_Loader = aufrufer;
            InitializeComponent();        
        }

        private void settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            youtube_Loader.Show();
            Close();
        }




        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            IniFile SettingsiniWriter = new IniFile(Application.StartupPath + @"\settings.ini");


            //State für Autocatch speichern
            if (EinstellungenCheckBox1.Checked == true)
            {
                SettingsiniWriter.IniWriteValue("Global", "Autocatch", "true");
            }
            else
            {
                SettingsiniWriter.IniWriteValue("Global", "Autocatch", "false");
            }

            if (EinstellungenTxtBoxDownloadpath.Text != SettingsiniWriter.IniReadValue("Global", "deploypath"))
            {
                SettingsiniWriter.IniWriteValue("Global", "deploypath", EinstellungenTxtBoxDownloadpath.Text);
            }

            //Einstellungen für Youtube_Loader neu laden
            youtube_Loader.initytloader();

            //Feedback an User und dann zurück zu Mainform
            MessageBox.Show("Einstellungen gespeichert!", "Gespeichert", MessageBoxButtons.OK, MessageBoxIcon.Information);
            youtube_Loader.Show();
            Close();

        }

        private void btnSettingsDownloadPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.InitialDirectory = @"c:\";
            folderBrowserDialog.Description = "Downloadverzeichnis wählen!";

            DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (Directory.Exists(folderBrowserDialog.SelectedPath) == true)
            {
                EinstellungenTxtBoxDownloadpath.Text = folderBrowserDialog.SelectedPath + "\\";
            }
            else
            {
                MessageBox.Show("Fehler - Bitte wählen Sie ein anderes Verzeichnis.");
            }
        }

        private void btnExitSettings_Click(object sender, EventArgs e)
        {
            youtube_Loader.Show();
            Close();
        }

        private void EinstellungenTxtBoxDownloadpath_MouseHover(object sender, EventArgs e)
        {
            EinstellungenLabelTooltip.Text = "Dies ist Ihr Downloadverzeichnis.";
        }

        private void btnSettingsDownloadPath_MouseHover(object sender, EventArgs e)
        {
            EinstellungenLabelTooltip.Text = "Neues Verzeichnis wählen.";
        }

        private void EinstellungenCheckBox1_MouseHover(object sender, EventArgs e)
        {
            EinstellungenLabelTooltip.Text = "Schalten Sie die Autocatch-Funktion ein und aus.";
        }

        private void btnSaveSettings_MouseHover(object sender, EventArgs e)
        {
            EinstellungenLabelTooltip.Text = "Aktuelle Einstellungen speichern.";
        }

        private void btnExitSettings_MouseHover(object sender, EventArgs e)
        {
            EinstellungenLabelTooltip.Text = "Einstellungen verlassen.";
        }

        //Einstellungen laden 
        //Anschließend Einstellungen setzen
        private void settings_Load(object sender, EventArgs e)
        {

            //Beim Laden des "Einstellungs-Forms" müssen ja die ausgelesenen Werte auf das Form übertragen werden.
            //Hier also Autocatch berücksichtigen
            IniFile SettingsInireader2 = new IniFile(Application.StartupPath + @"\settings.ini");
            if (SettingsInireader2.IniReadValue("Global", "Autocatch") == "true")
            {
                EinstellungenCheckBox1.Checked = true;
            }
            else
            {
                EinstellungenCheckBox1.Checked = false;
            }

            //Downloadpath aus Settings.ini lesen und in Textbox eintragen.
            EinstellungenTxtBoxDownloadpath.Text = SettingsInireader2.IniReadValue("Global", "deploypath");

        }
    }
}
