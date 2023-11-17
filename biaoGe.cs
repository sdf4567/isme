using FastReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HYAnJianDengLu
{
    public class biaoGe
    {

        public static void shenQingBiao(DataTable dt, string path01)
        {

            // 获得当前程序的运行路径
            string path = Application.StartupPath;

            // 定义报表
            Report report = new Report();
            string strDirectory = path + "\\ReportFiles";

            // 判断文件路径是否存在，不存在则创建文件夹
            if (!Directory.Exists(strDirectory))
            {
                // 不存在就创建目录
                Directory.CreateDirectory(strDirectory);
            }
            // 判断文件是否存在 
            if (!File.Exists(strDirectory + "\\sqb02.frx"))
            {
                report.FileName = strDirectory + "\\sqb02.frx";
            }
            else
            {
                report.Load(strDirectory + "\\sqb02.frx");
            }




            // 创建报表文件的数据源
            DataSet ds = new DataSet();
            //给报表绑定数据源
            DataTable dtSource = dt.Copy();
            dtSource.TableName = "ProductDetail";
            ds.Tables.Add(dtSource);
            //给报表装载数据源
            report.RegisterData(ds);

            //向报表推送二维码位置，不写这句会出现二维码不准确的情况
            ((PictureObject)report.FindObject("ewmView")).ImageLocation = path01;



            //判断是否同时按下Ctrl和鼠标左键
            if ((int)Control.ModifierKeys == (int)Keys.Control)
            {
                // 打开设计界面
                report.Design();

            }
            else
            {
                //直接打印
                report.PrintPrepared();
                report.PrintSettings.ShowDialog = false;
                report.Print();

                report.Dispose();
                // 打开预览界面
                //report.Show();
                Thread.Sleep(1000);

            }


        }

        public static void waijianzhengmian(DataTable dt)
        {

            // 获得当前程序的运行路径
            string path = Application.StartupPath;
            // 定义报表
            Report report = new Report();
            string strDirectory = path + "\\ReportFiles";

            // 判断文件路径是否存在，不存在则创建文件夹
            if (!Directory.Exists(strDirectory))
            {
                // 不存在就创建目录
                Directory.CreateDirectory(strDirectory);
            }

            // 判断文件是否存在 
            if (!File.Exists(strDirectory + "\\anjianwaijianzhengmian.frx"))
            {
                report.FileName = strDirectory + "\\anjianwaijianzhengmian.frx";
            }
            else
            {
                report.Load(strDirectory + "\\anjianwaijianzhengmian.frx");
            }

            // 创建报表文件的数据源
            DataSet ds = new DataSet();
            //给报表绑定数据源
            DataTable dtSource = dt.Copy();
            dtSource.TableName = "ProductDetail";
            ds.Tables.Add(dtSource);
            //给报表装载数据源
            report.RegisterData(ds);

            //判断是否同时按下Ctrl和鼠标左键
            if ((int)Control.ModifierKeys == (int)Keys.Control)
            {

                // 打开设计界面
                report.Design();

            }
            else
            {
                //直接打印
                report.PrintPrepared();
                report.PrintSettings.ShowDialog = false;
                report.Print();

                report.Dispose();
                // 打开预览界面
                //report.Show();
                Thread.Sleep(1000);

            }
        }

        public static void waijianfangmian(DataTable dt,string path01)
        {

            // 获得当前程序的运行路径
            string path = Application.StartupPath;
            // 定义报表
            Report report = new Report();
            string strDirectory = path + "\\ReportFiles";

            // 判断文件路径是否存在，不存在则创建文件夹
            if (!Directory.Exists(strDirectory))
            {
                // 不存在就创建目录
                Directory.CreateDirectory(strDirectory);
            }

            // 判断文件是否存在 
            if (!File.Exists(strDirectory + "\\anjianwaijianfanmian.frx"))
            {
                report.FileName = strDirectory + "\\anjianwaijianfanmian.frx";
            }
            else
            {
                report.Load(strDirectory + "\\anjianwaijianfanmian.frx");
            }

            // 创建报表文件的数据源
            DataSet ds = new DataSet();
            //给报表绑定数据源
            DataTable dtSource = dt.Copy();
            dtSource.TableName = "ProductDetail";
            ds.Tables.Add(dtSource);
            //给报表装载数据源
            report.RegisterData(ds);

            //向报表推送二维码位置，不写这句会出现二维码不准确的情况
            ((PictureObject)report.FindObject("ewmView")).ImageLocation = path01;

            //判断是否同时按下Ctrl和鼠标左键
            if ((int)Control.ModifierKeys == (int)Keys.Control)
            {

                // 打开设计界面
                report.Design();

            }
            else
            {
                //直接打印
                report.PrintPrepared();
                report.PrintSettings.ShowDialog = false;
                report.Print();

                report.Dispose();
                // 打开预览界面
                //report.Show();
                Thread.Sleep(1000);

            }
        }


    }
}
