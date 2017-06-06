using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_AntrasDarbas_AG
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static Mutex mutex = new Mutex(false, "programa");


        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Programa jau paleista");
            }
        }
    }
}
