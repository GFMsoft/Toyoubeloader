// ToyoubeLoader by Ferdinand Marx - 2022
// Gui für Youtube-DL
// Diese Software steuert Youtube-DL

// Diese Software kann selbstständig keine Youtubevideos runterladen oder konvertieren.
// Youtube-DL.exe, ffmpeg.exe, ffplay.exe und ffprobe.exe müssen im Programmverzeichnis (Application.StartupPath) liegen!

// Load Yotube-DL.exe, ffmpeg.exe, ffplay.exe and ffprobe.exe only from official websites - VALIDATE SHA256 fingerprint and signature!
// This software can't load or convert content form youtube on it's own.
// Toyoubeloader is only a frontend for youtube-dl.
// Youtube-dl.exe must be installed into the startuppath of Toyoubeloader.



using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ToyoubeLoader
{
    public partial class Main : Form
    {
        //Form-Title mit Versionsinformation setzen
        public string version = "1.6";
        public string prgname = "Toyoubeloader";
        public override string Text { get => base.Text; set => base.Text = $"{prgname} - Version {version}"; }

//       ___ ___                    __              
//      |   |   |.-----.----.-----.|__|.-----.-----.
//      |   |   ||  -__|   _|__ --||  ||  _  |     |
//       \_____/ |_____|__| |_____||__||_____|__|__|

        // Application.ProductVersion und AutoIncrement macht Probleme.
        // Deshalb hier manuelle Versionierung.
        // Auf GUI immer nur Haupt- und Nebenversionsnummer
        // Resvision- und Buildnummer intern.

        // Internal: 1.6.0.1
        // External: 1.6
        // var version beachten!
        // bei commit **INTERNAL** mit angeben!



        SynchronizationContext _syncContext;
        public static int lineCount = 0;
        public static StringBuilder output = new StringBuilder();
        public string std_data;
        public string DeployPath;
        public string Autocatch;



        public Main()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            initytloader(); //Init System
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            //Nur Download starten, wenn Anzahl der Links in ListBox nicht 0            
            if (listBox.Items.Count != 0)
            {
                progressBar1.Value = 0;
                progressBar1.Maximum = listBox.Items.Count;

                //Alle Links aus Listbox in Array eintragen
                string[] linkarray = new string[listBox.Items.Count];
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    linkarray[i] = listBox.Items[i].ToString();
                }

                for (int i = 0; i < linkarray.Length; i++)
                {
                    progressBar1.PerformStep();


                    if (checklink(linkarray[i]) == true)
                    {
                        download(linkarray[i]);
                    }
                    else
                    {
                        MessageBox.Show("Fehler! - Download konnte nicht gestartet werden!");
                    }
                }

                //Info an User, dass Download fertig.
                MessageBox.Show("Download complete!", "Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Sie müssen Links eintragen, um einen Download starten zu können!", "Fehler - Keine Links", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnsettings_Click(object sender, EventArgs e)
        {
            settings form2 = new(this);
            form2.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {


            textBox.Text = textBox.Text + "Prüfe Link auf Gültigkeit!" + Environment.NewLine;
            string checklinkval = textBoxLink.Text;

            //Prüfen ob Link gültig
            if (checklink(textBoxLink.Text) == true)
            {

                // Get-Title - Prozess - Dieser Prozess holt den Titel aus dem gegebenen Link.
                //----------------------------------------------------------------------------------

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "youtube-dl.exe";
                    process.StartInfo.Arguments = $"--ignore-errors --get-title {textBoxLink.Text}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.OutputDataReceived += (sender, args) => Displaytitle(args.Data);
                    process.ErrorDataReceived += (sender, args) => Displaytitle(args.Data);


                    try
                    {
                        process.Start();
                    }
                    catch (Win32Exception w)
                    {
                        MessageBox.Show(w.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    finally
                    {
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        process.Close();
                    }




                }
                //Wenn verfügbar, dann Titel aus STDOUT holen und anzeigen.
                void Displaytitle(string output)
                {
                    _syncContext.Post(_ => textBox.AppendText(output), null);
                }
                //----------------------------------------------------------------------------------



                listBox.Items.Add(textBoxLink.Text);
                textBoxLink.Text = "";
                textBox.Text = textBox.Text + "Link akzeptiert!" + Environment.NewLine;

            }
            else
            {
                if (textBoxLink.Text != "")
                {
                    textBox.Text = $"{textBoxLink.Text} Ungültig!" + Environment.NewLine;
                    textBoxLink.Text = "";
                }
                else
                {
                    textBox.Text = $"Bitte geben Sie einen gültigen Link ein!" + Environment.NewLine;
                }

            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                listBox.Items.RemoveAt(listBox.SelectedIndex);
            }
            catch (ArgumentOutOfRangeException w)
            {
                if (listBox.Items.Count == 0)
                {
                    MessageBox.Show("Fehler - Es gibt keine Links, die entfernt werden können!", "Fehler - Keine Links enthalten", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Fehler - Es konnte kein Link entfernt werden!", "Fehler - Link löschen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //Diese Methode initialisiert das System/Programm
        public void initytloader()
        {

            //Settings.ini erzeugen, wenn nicht vorhanden
            //Wenn vorhanden, dann auslesen und Werte ermitteln - Deploypath dann direkt in var DeployPath
            if (File.Exists(Application.StartupPath + @"\settings.ini") == true)
            {
                IniFile SettingsIniReader = new IniFile(Application.StartupPath + @"\settings.ini");
                DeployPath = SettingsIniReader.IniReadValue("Global", "deploypath");
                Autocatch = SettingsIniReader.IniReadValue("Global", "autocatch");
            }
            else
            {
                IniFile SettingsIni = new IniFile(Application.StartupPath + @"\settings.ini");
                SettingsIni.IniWriteValue("Global", "deploypath", Application.StartupPath);
                SettingsIni.IniWriteValue("Global", "autocatch", "false");

            }

            //Prüfen ob notwendige Files im Programmverzeichnis liegen
            if (File.Exists(Application.StartupPath + "ffmpeg.exe") != true)
            {
                MessageBox.Show($"Fehler - ffmpeg.exe befindet sich nicht in \n {Application.StartupPath} \n Funktion des Programms ist gestört! \n Bitte Dateien nachliefern!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (File.Exists(Application.StartupPath + "ffplay.exe") != true)
            {
                MessageBox.Show($"Fehler - ffplay.exe befindet sich nicht in \n {Application.StartupPath} \n Funktion des Programms ist gestört! \n Bitte Dateien nachliefern!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (File.Exists(Application.StartupPath + "ffprobe.exe") != true)
            {
                MessageBox.Show($"Fehler - ffprobe.exe befindet sich nicht in \n {Application.StartupPath} \n Funktion des Programms ist gestört! \n Bitte Dateien nachliefern!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (File.Exists(Application.StartupPath + "youtube-dl.exe") != true)
            {
                MessageBox.Show($"Fehler - youtube-dl.exe befindet sich nicht in \n {Application.StartupPath} \n Funktion des Programms ist gestört!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show($"Programm wird geschlossen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }


        //Diese Methode startet den Download
        public string download(string link)
        {

            textBox.Text = "Download wird gestartet!";
            textBox.Text = "BITTE WARTEN!";
            Application.DoEvents();

            //Downloadpfad aus Settings.ini holen.
            IniFile settingsreader4 = new IniFile(Application.StartupPath + @"\settings.ini");
            DeployPath = settingsreader4.IniReadValue("Global", "deploypath");
            if (Directory.Exists(DeployPath) == true)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "youtube-dl.exe";
                    process.StartInfo.Arguments = $"--ignore-errors -x --audio-format mp3 -o {DeployPath}%(title)s.%(ext)s --audio-quality 0 --prefer-ffmpeg --ffmpeg-location ffmpeg.exe {link}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.OutputDataReceived += (sender, args) => Display(args.Data);
                    process.ErrorDataReceived += (sender, args) => Display(args.Data);


                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    process.Close();


                    listBox.Items.Clear();
                    textBox.AppendText(Environment.NewLine + "Download complete!");

                }

                void Display(string output)
                {
                    _syncContext.Post(_ => textBox.AppendText(Environment.NewLine + output), null);

                }


                return link + " - Download complete";
            }
            else
            {
                MessageBox.Show($"Fehler! - Deploypath: {DeployPath} exisitiert nicht!");
                return "Error - Deploypath existiert nicht!";
            }


        }



        //Diese Methode prüft, ob der übergebene Link ein Youtube-Link ist.
        //Erstmal nur relativ simple aus Zeitmangel
        //Methode wird mit RegEx erweitert.
        public bool checklink(string link)
        {


            if (link.Contains("youtube.com") || link.Contains("youtube.de") || link.Contains("youtu.be"))
            {
                if (link == "www.youtube.de/XYZ")
                {
                    return false;
                }

                return true;

            }
            else
            {
                return false;
            }

        }

        //Autocatchmethode
        //Vielleicht implementiere ich irgendwann einmal einen Thread, der dann die Links aus dem Clip holt.
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (Autocatch == "true")
            {
                if (Clipboard.ContainsText())
                {
                    if (Clipboard.GetText().Contains("youtube.de") || Clipboard.GetText().Contains("youtube.com") || Clipboard.GetText().Contains("youtu.be"))
                    {


                        if (checklink(Clipboard.GetText()) == true)
                        {
                            listBox.Items.Add(Clipboard.GetText());
                            textBox.Text = textBox.Text + "Link akzeptiert!" + Environment.NewLine;

                        }


                        Clipboard.Clear();

                    }
                }
            }



        }

        private void textBoxLink_MouseClick(object sender, MouseEventArgs e)
        {
            textBoxLink.Text = "";
        }
    }
}