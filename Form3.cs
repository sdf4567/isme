using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HYAnJianDengLu
{
    public partial class Form3 : Form
    {
        string path, hphm;

        public Form3(string path, string hphm)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.path = path;
            this.hphm = hphm;
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
            pictureBox1.ImageLocation = this.path;
            label1.Text = this.hphm;
           
        }
    }
}
