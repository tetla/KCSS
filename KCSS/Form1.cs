using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace KCSS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string xmlPath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
            if (!System.IO.File.Exists(xmlPath))
            {
                System.IO.File.Create(xmlPath);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckWindow Cwin = new CheckWindow();
            Point Wp = Cwin.KanCollePosition();
            textBox1.Text = Wp.X.ToString();
            textBox2.Text = Wp.Y.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(800, 480);
            Rectangle rect = new Rectangle(int.Parse(textBox1.Text), int.Parse(textBox2.Text), 800, 480);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            Form2 form2 = new Form2();
            form2.Show();
            form2.pictureBox1.Image = bmp;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
