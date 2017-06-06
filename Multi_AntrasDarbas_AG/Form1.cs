using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;

namespace Multi_AntrasDarbas_AG
{
    public partial class Form1 : Form
    {
        private static Mutex mutex = new Mutex();


        private string kelias;
        private string archyvokelias;
        private int current = 0;

        ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent pauseEvent = new ManualResetEvent(true);




        public Form1()
        {
            InitializeComponent();
        }

        private void RunEncrypt()
        {
            archyvokelias = kelias + @"\archyvas.zip";

            AES aes = new AES();
            HashCalc hash = new HashCalc();
            string txtpath = @"C:\Users\Arnas\Desktop\md5.txt";

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                foreach (var item in listBox1.Items)
                {
                        pauseEvent.WaitOne();

                        if (shutdownEvent.WaitOne(0))
                            break;

                        aes.EncryptFile(item.ToString(), "123");

                        if (!File.Exists(txtpath))
                        {
                            File.Create(txtpath).Dispose();
                            using (StreamWriter sw = File.AppendText(txtpath))
                            {
                                sw.WriteLine(hash.checkMD5(item.ToString()));
                            }
                        }

                        else if (File.Exists(txtpath))
                        {
                            using (StreamWriter sw = File.AppendText(txtpath))
                            {
                                sw.WriteLine(hash.checkMD5(item.ToString()));
                            }
                        }
                        current++;
                        Invoke(new EventHandler(delegate { progressBar1.Value = (100 * current / listBox1.Items.Count); }));
                    }                                  
            }

            else
            {
                MessageBox.Show("Procesas jau vyksta");
            }
        }


        private void RunDecrypt()
        {


            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                AES aes = new AES();

                foreach (var item in listBox1.Items)
                {
                    pauseEvent.WaitOne();

                    if (shutdownEvent.WaitOne(0))
                        break;

                    current++;
                    aes.DecryptFile(item.ToString(), "123");
                    Invoke(new EventHandler(delegate { progressBar1.Value = (100 * current / listBox1.Items.Count); }));

                }
            }
            else
            {
                MessageBox.Show("Procesas jau vyksta");
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(RunEncrypt);
            thread.Start();
            thread.IsBackground = true;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            Thread thread = new Thread(RunDecrypt);
            thread.Start();
            thread.IsBackground = true;
            //Invoke(new EventHandler(delegate { progressBar1.Value = 0; }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                kelias = fbd.SelectedPath;
                txtDir.Text = kelias;

                string[] files = Directory.GetFiles(kelias);
                string[] folders = Directory.GetDirectories(kelias);

                foreach (string file in files)
                {
                    listBox1.Items.Add(file);
                }

                foreach (string folder in folders)
                {
                    listBox1.Items.Add(folder);
                }
            }
            else
            {
                MessageBox.Show("Neteisingas pasirinkimas");
            }
        }


        private void btnPause_Click(object sender, EventArgs e)
        {
            pauseEvent.Reset();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            pauseEvent.Set();
        }

        private void btnTerminate_Click(object sender, EventArgs e)
        {
            //shutdownEvent.Set();
            //thread.Join();
            //stop = true;
        }

    }
}
