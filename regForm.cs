using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HYAnJianDengLu
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            SoftReg softReg = new SoftReg(); //实例化一个

            
            string aa = softReg.GetMNum();
          
            textBox1.Text = aa;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey hklm = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("mySoftWare").CreateSubKey("Register.INI").CreateSubKey(textBox2.Text);
            RegistryKey hkSoftWare = hklm.OpenSubKey("SOFTWARE\\mySoftWare\\Register.INI", true);

            hklm.Close();
            //hkSoftWare.Close();




            MessageBox.Show("注册完成，请重启软件验证");
        }
    }
}
