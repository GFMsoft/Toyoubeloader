using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ToyoubeLoader
{

    //Diese Klasse erleichtert mir das Lesen und Schreiben von ini-Dateien.
    //Wichtig, wenn wir Settings im Hauptprogramm speichern und lesen wollen.
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
         string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
          string key, string def, StringBuilder retVal,
          int size, string filePath);

        //Konstruktor mit Eigenschaft path
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        //Methode zum Schreiben des Ini-Files
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        //Methode zum Lesen des Ini-Files
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }


}

