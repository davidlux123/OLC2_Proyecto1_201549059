using _OLC2_Proyecto1.src;
using _OLC2_Proyecto1.src.Gramatica;
using _OLC2_Proyecto1.src.Interfaces;
using Irony;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _OLC2_Proyecto1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //string entrada = File.ReadAllText("C:\\Users\\David Lux\\Desktop\\OLC2\\Proyecto1\\[OLC2]Proyecto1\\src\\entrada.txt");

        }
    }
}
