using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using OpenQA.Selenium.DevTools.V112.Network;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using System.IO.Compression;
using System.Drawing.Imaging;
using QRCoder;
using System.Linq;
using System.Windows.Forms.VisualStyles;

namespace HYAnJianDengLu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetAllInitInfo(this.Controls[0]);
            Control.CheckForIllegalCrossThreadCalls = false;
        }


        public static string zhuye = ConfigurationManager.ConnectionStrings["zhuye"].ConnectionString;
        public static string link = ConfigurationManager.ConnectionStrings["link"].ConnectionString;
        public static string jiancheng = ConfigurationManager.ConnectionStrings["jiancheng"].ConnectionString;
        public static string denglu = ConfigurationManager.ConnectionStrings["denglu"].ConnectionString;
        public string hphm = string.Empty;
        public string erweimaPath = string.Empty;

        // ChromeDriver webDriver;

        IWebDriver webDriver = null;


        Root info = null;
        SerialPort sp = new SerialPort();
        Form2 f2 = new Form2();
        SoftReg softReg = new SoftReg();
        Root copyInfo = null;
        string zxbh = string.Empty;
        string phone = string.Empty;
        string jsondata = string.Empty;


        // 引入GetAsyncKeyState函数
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);


        #region 设置窗体自适应的代码

        double formWidth;//窗体原始宽度
        double formHeight;//窗体原始高度
        double scaleX;//水平缩放比例
        double scaleY;//垂直缩放比例
        Dictionary<string, string> ControlsInfo = new Dictionary<string, string>();//控件中心Left,Top,控件Width,控件Height,控件字体Size

        public void GetAllInitInfo(Control ctrlContainer)
        {
            if (ctrlContainer.Parent == this)//获取窗体的高度和宽度
            {
                formWidth = Convert.ToDouble(ctrlContainer.Width);
                formHeight = Convert.ToDouble(ctrlContainer.Height);
            }
            foreach (Control item in ctrlContainer.Controls)
            {
                if (item.Name.Trim() != "")
                {
                    //添加信息：键值：控件名，内容：据左边距离，距顶部距离，控件宽度，控件高度，控件字体。
                    ControlsInfo.Add(item.Name, (item.Left + item.Width / 2) + "," + (item.Top + item.Height / 2) + "," + item.Width + "," + item.Height + "," + item.Font.Size);
                }
                if ((item as UserControl) == null && item.Controls.Count > 0)
                {
                    GetAllInitInfo(item);
                }
            }

        }

        private void ControlsChaneInit(Control ctrlContainer)
        {
            scaleX = (Convert.ToDouble(ctrlContainer.Width) / formWidth);
            scaleY = (Convert.ToDouble(ctrlContainer.Height) / formHeight);
        }
        /// <summary>
        /// 改变控件大小
        /// </summary>
        /// <param name="ctrlContainer"></param>
        public void ControlsChange(Control ctrlContainer)
        {
            double[] pos = new double[5];//pos数组保存当前控件中心Left,Top,控件Width,控件Height,控件字体Size
            foreach (Control item in ctrlContainer.Controls)//遍历控件
            {
                if (item.Name.Trim() != "")//如果控件名不是空，则执行
                {
                    if ((item as UserControl) == null && item.Controls.Count > 0)//如果不是自定义控件
                    {
                        ControlsChange(item);//循环执行
                    }
                    string[] strs = ControlsInfo[item.Name].Split(',');//从字典中查出的数据，以‘，’分割成字符串组

                    for (int i = 0; i < 5; i++)
                    {
                        pos[i] = Convert.ToDouble(strs[i]);//添加到临时数组
                    }
                    double itemWidth = pos[2] * scaleX;     //计算控件宽度，double类型
                    double itemHeight = pos[3] * scaleY;    //计算控件高度
                    item.Left = Convert.ToInt32(pos[0] * scaleX - itemWidth / 2);//计算控件距离左边距离
                    item.Top = Convert.ToInt32(pos[1] * scaleY - itemHeight / 2);//计算控件距离顶部距离
                    item.Width = Convert.ToInt32(itemWidth);//控件宽度，int类型
                    item.Height = Convert.ToInt32(itemHeight);//控件高度
                    try
                    {

                        item.Font = new Font(item.Font.Name, float.Parse((pos[4] * Math.Min(scaleX, scaleY)).ToString()));//字体

                    }
                    catch
                    {


                    }
                }
            }

        }





        #endregion

        #region 填表的函数
        //base64转字符串
        public static string Base64ToString(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            string plainString = System.Text.Encoding.UTF8.GetString(bytes);
            return plainString;
        }
        //msg
        public void msg(string str)
        {
            textBox1.AppendText(str + "\r\n");
        }


        public static bool ContainsSubstring(string inputString, string substring)
        {
            bool containsPattern = inputString.Contains(substring);
            return containsPattern;
        }

        public static string RemoveSubstring(string inputString, string substring)
        {
            string modifiedString = inputString.Replace(substring, "");
            return modifiedString;
        }

        #region HY填表
        //信息填表
        public void infoTianBiao(Root info)
        {
            if (webDriver != null)
            {
                ReadOnlyCollection<IWebElement> iframes = webDriver.FindElements(By.TagName("iframe"));
                for (int i = 0; i < iframes.Count; i++)
                {
                    //切换到iframe
                    webDriver.SwitchTo().Frame(iframes[i]);
                    try
                    {
                        //cph 号牌号码
                        IWebElement cph = webDriver.FindElement(By.Id("cph")); // 在当前的iframe中找到表单元素
                        cph.Clear(); // 清空原来的输入值
                        cph.SendKeys(info.words_result.号牌号码.words); // 填写新的输入值

                        ////pzlbid 号牌种类
                        ////定位到<select> 元素
                        //IWebElement pzlbid = webDriver.FindElement(By.Id("pzlbid"));
                        ////使用SelectElement类创建<select> 元素的包装器
                        //SelectElement pzlbid1 = new SelectElement(pzlbid);
                        //pzlbid1.SelectByValue("01");

                        //dph 车架号码
                        IWebElement dph = webDriver.FindElement(By.Id("dph")); // 在当前的iframe中找到表单元素
                        dph.Clear(); // 清空原来的输入值
                        dph.SendKeys(info.words_result.车辆识别代号.words); // 填写新的输入值

                        //jclbid 检测类别
                        IWebElement jclbid = webDriver.FindElement(By.Id("jclbid"));
                        //使用SelectElement类创建<select> 元素的包装器
                        SelectElement jclbid1 = new SelectElement(jclbid);
                        jclbid1.SelectByValue("0007");

                        ////xszbh 行驶证证芯编号
                        //IWebElement xszbh = webDriver.FindElement(By.Id("xszbh")); // 在当前的iframe中找到表单元素
                        //xszbh.Clear(); // 清空原来的输入值
                        //xszbh.SendKeys("123"); // 填写新的输入值

                        //dw 机动车所有人
                        IWebElement dw = webDriver.FindElement(By.Id("dw")); // 在当前的iframe中找到表单元素
                        dw.Clear(); // 清空原来的输入值
                        dw.SendKeys(info.words_result.所有人.words); // 填写新的输入值

                        //czdz 车主地址
                        IWebElement czdz = webDriver.FindElement(By.Id("czdz")); // 在当前的iframe中找到表单元素
                        czdz.Clear(); // 清空原来的输入值
                        czdz.SendKeys(info.words_result.住址.words); // 填写新的输入值

                        //czdz 车主地址
                        IWebElement QXMCID = webDriver.FindElement(By.Id("QXMCID")); // 在当前的iframe中找到表单元素
                        QXMCID.Clear(); // 清空原来的输入值
                        QXMCID.SendKeys("301800"); // 填写新的输入值


                        //cllbx 车辆类型1
                        IWebElement cllbx = webDriver.FindElement(By.Id("cllbx"));
                        //使用SelectElement类创建<select> 元素的包装器
                        SelectElement cllbx1 = new SelectElement(cllbx);
                        cllbx1.SelectByValue("K");


                        //cllbxid 车辆类型2
                        IWebElement cllbxid = webDriver.FindElement(By.Id("cllbxid"));
                        //使用SelectElement类创建<select> 元素的包装器
                        SelectElement cllbxid1 = new SelectElement(cllbxid);
                        cllbxid1.SelectByValue("K33");



                        ////pzlbid 号牌种类
                        ////定位到<select> 元素
                        //IWebElement pzlbid = webDriver.FindElement(By.Id("pzlbid"));
                        ////使用SelectElement类创建<select> 元素的包装器
                        //SelectElement pzlbid1 = new SelectElement(pzlbid);
                        //pzlbid1.SelectByValue("01");

                        //HBryxjg 供油方式
                        IWebElement HBryxjg = webDriver.FindElement(By.Id("HBryxjg"));
                        //使用SelectElement类创建<select> 元素的包装器
                        SelectElement HBryxjg1 = new SelectElement(HBryxjg);
                        HBryxjg1.SelectByValue("2");


                    }
                    catch
                    {

                    }
                    webDriver.SwitchTo().DefaultContent(); // 切换回默认的上下文
                }
            }

        }

        //证芯编号填表
        public void zhegnXinTianBiao(string str)
        {
            if (webDriver != null)
            {
                ReadOnlyCollection<IWebElement> iframes = webDriver.FindElements(By.TagName("iframe"));

                for (int i = 0; i < iframes.Count; i++)
                {
                    //切换到iframe
                    webDriver.SwitchTo().Frame(iframes[i]);
                    try
                    {
                        //xszbh 行驶证证芯编号
                        IWebElement xszbh = webDriver.FindElement(By.Id("xszbh")); // 在当前的iframe中找到表单元素
                        xszbh.Clear(); // 清空原来的输入值
                        xszbh.SendKeys(str); // 填写新的输入值


                    }
                    catch
                    {

                    }
                    webDriver.SwitchTo().DefaultContent(); // 切换回默认的上下文
                }

            }
        }

        //送检单位
        public void songjianTianBiao(string str)
        {
            if (webDriver != null)
            {
                ReadOnlyCollection<IWebElement> iframes = webDriver.FindElements(By.TagName("iframe"));

                for (int i = 0; i < iframes.Count; i++)
                {
                    //切换到iframe
                    webDriver.SwitchTo().Frame(iframes[i]);
                    try
                    {
                        string[] s = str.Split('-');

                        //xszbh 送检人
                        IWebElement sjdw = webDriver.FindElement(By.Id("sjdw")); // 在当前的iframe中找到表单元素
                        sjdw.Clear(); // 清空原来的输入值
                        sjdw.SendKeys(s[0]); // 填写新的输入值

                        //xszbh 送检人电话
                        IWebElement sjdh = webDriver.FindElement(By.Id("czdh")); // 在当前的iframe中找到表单元素
                        sjdh.Clear(); // 清空原来的输入值
                        sjdh.SendKeys(s[1]); // 填写新的输入值

                        //号牌号码
                        IWebElement hphm = webDriver.FindElement(By.XPath("//*[@id=\"cph\"]"));
                        // 在当前的iframe中找到表单元素
                        hphm.Clear(); // 清空原来的输入值
                        hphm.SendKeys(info.words_result.号牌号码.words); // 填写新的输入值

                        //住址
                        IWebElement zhudi = webDriver.FindElement(By.XPath("//*[@id=\"czdz\"]"));
                        // 在当前的iframe中找到表单元素
                        zhudi.Clear(); // 清空原来的输入值
                        zhudi.SendKeys(info.words_result.住址.words); // 填写新的输入值



                    }
                    catch
                    {

                    }
                    webDriver.SwitchTo().DefaultContent(); // 切换回默认的上下文
                }

            }
        }


        #endregion

        #region 保定填表
        public void baodingTianBiao(Root info)
        {
            if (webDriver != null)
            {

                try
                {
                    //号牌号码
                    IWebElement hphm = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[4]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    hphm.Clear(); // 清空原来的输入值
                    hphm.SendKeys(info.words_result.号牌号码.words); // 填写新的输入值

                    //车辆识别代号
                    IWebElement vin = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[6]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    vin.Clear(); // 清空原来的输入值
                    vin.SendKeys(info.words_result.车辆识别代号.words); // 填写新的输入值



                    //初次登记日期
                    char[] ss = info.words_result.注册日期.words.ToCharArray();
                    string date = ss[0].ToString() + ss[1].ToString() + ss[2].ToString() + ss[3].ToString() + "-" + ss[4].ToString() + ss[5].ToString() + "-" + ss[6].ToString() + ss[7].ToString();

                    IWebElement zcDate = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[8]/div/div/input")); // 在当前的iframe中找到表单元素
                    zcDate.Clear(); // 清空原来的输入值
                    zcDate.SendKeys(date); // 填写新的输入值

                    //检验有效期止
                    IWebElement djDate = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[9]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    djDate.Clear(); // 清空原来的输入值
                    djDate.SendKeys(date); // 填写新的输入值



                    //所有人
                    IWebElement syr = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[3]/div[14]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    syr.Clear(); // 清空原来的输入值
                    syr.SendKeys(info.words_result.所有人.words); // 填写新的输入值



                    //邮寄地址
                    IWebElement zhuzhi = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[3]/div[16]/div/div[1]/textarea")); // 在当前的iframe中找到表单元素
                    zhuzhi.Clear(); // 清空原来的输入值
                    zhuzhi.SendKeys(info.words_result.住址.words); // 填写新的输入值


                    textBox2.Text = info.words_result.号牌号码.words;
                    syr.Click();
                    ////车辆类型
                    //IWebElement cllx = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[2]/div[2]/div/div/div/input")); // 在当前的iframe中找到表单元素
                    //cllx.Clear(); // 清空原来的输入值
                    //cllx.SendKeys(info.words_result.车辆类型.words); // 填写新的输入值


                    //xszbh 行驶证证芯编号
                    //IWebElement xszbh = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[7]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    //xszbh.Clear(); // 清空原来的输入值
                    //xszbh.SendKeys("12312312312"); // 填写新的输入值
                }
                catch
                {


                }
            }



        }

        public void baodinghuayandenglu(Root info)
        {


            if (webDriver != null)
            {
                ReadOnlyCollection<IWebElement> iframes = webDriver.FindElements(By.TagName("iframe"));

                for (int i = 0; i < iframes.Count; i++)
                {
                    //切换到iframe
                    webDriver.SwitchTo().Frame(iframes[i]);
                    try
                    {
                        //号牌号码
                        IWebElement hphm = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[1]/div/div[5]/div/div[2]/div/div/input"));
                        // 在当前的iframe中找到表单元素
                        hphm.Clear(); // 清空原来的输入值
                        hphm.SendKeys(info.words_result.号牌号码.words); // 填写新的输入值


                    }
                    catch
                    {

                    }
                    webDriver.SwitchTo().DefaultContent(); // 切换回默认的上下文
                }

            }





        }


        public void baodingZhengXinBiao(string str)
        {
            if (webDriver != null)
            {
                try
                {
                    //xszbh 行驶证证芯编号
                    IWebElement xszbh = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[7]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    xszbh.Clear(); // 清空原来的输入值
                    xszbh.SendKeys(str); // 填写新的输入值
                }
                catch
                {


                }
            }
        }

        public void baodingSongJianRen(string str)
        {
            string[] ss = str.Split(new char[] { '-' });


            if (webDriver != null)
            {
                try
                {

                    //送检人
                    IWebElement sjr = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[3]/div[12]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    sjr.Clear(); // 清空原来的输入值
                    sjr.SendKeys(ss[0]); // 填写新的输入值

                    //送检人电话
                    IWebElement sjrphone = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[3]/div[13]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    sjrphone.Clear(); // 清空原来的输入值
                    sjrphone.SendKeys(ss[1]); // 填写新的输入值

                    //所有人电话
                    IWebElement syrphone = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[3]/div[15]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    syrphone.Clear(); // 清空原来的输入值
                    syrphone.SendKeys(ss[1]); // 填写新的输入值
                }
                catch
                {


                }
            }
        }

        #endregion

        #region 天津填表
        public void tianJinTianBiao(Root info)
        {
            if (webDriver != null)
            {
                try
                {
                    //号牌号码
                    IWebElement hphm = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[1]/div/div[5]/div/div[2]/div/div/input"));
                    // 在当前的iframe中找到表单元素
                    hphm.Clear(); // 清空原来的输入值
                    hphm.SendKeys(info.words_result.号牌号码.words); // 填写新的输入值

                    //车辆识别代号
                    IWebElement vin = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[1]/div[2]/div/div/div[1]/div/input")); // 在当前的iframe中找到表单元素
                    vin.Clear(); // 清空原来的输入值
                    vin.SendKeys(info.words_result.车辆识别代号.words); // 填写新的输入值



                    //初次登记日期
                    char[] ss = info.words_result.注册日期.words.ToCharArray();
                    string date = ss[0].ToString() + ss[1].ToString() + ss[2].ToString() + ss[3].ToString() + "-" + ss[4].ToString() + ss[5].ToString() + "-" + ss[6].ToString() + ss[7].ToString();
                    IWebElement zcDate = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[1]/div[2]/div/div/div[11]/div/div/div[1]/div/input"));
                    // 在当前的iframe中找到表单元素
                    zcDate.Clear(); // 清空原来的输入值
                    zcDate.SendKeys(date); // 填写新的输入值

                    //检验有效期止
                    //IWebElement djDate = webDriver.FindElement(By.XPath("//*[@id=\"app\"]/section/section/section/main/div/form/div[2]/div/div/div[1]/div[9]/div/div[1]/input")); // 在当前的iframe中找到表单元素
                    //djDate.Clear(); // 清空原来的输入值
                    //djDate.SendKeys(date); // 填写新的输入值

                    //所有人
                    IWebElement syr = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[1]/div[2]/div/div/div[16]/div/div[1]/div/input")); // 在当前的iframe中找到表单元素
                    syr.Clear(); // 清空原来的输入值
                    syr.SendKeys(info.words_result.所有人.words); // 填写新的输入值



                    //邮寄地址
                    IWebElement zhuzhi = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[3]/div[2]/div/div/div[5]/div/input")); // 在当前的iframe中找到表单元素
                    zhuzhi.Clear(); // 清空原来的输入值
                    zhuzhi.SendKeys(info.words_result.住址.words); // 填写新的输入值


                    textBox2.Text = info.words_result.号牌号码.words;

                    //301800
                    IWebElement youbian = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[3]/div[2]/div/div/div[6]/div/input")); // 在当前的iframe中找到表单元素
                    youbian.Clear(); // 清空原来的输入值
                    youbian.SendKeys("301800"); // 填写新的输入值
                }
                catch
                {


                }
            }



        }

        public void tianJinZhengXinBiao(string str)
        {
            if (webDriver != null)
            {
                ///html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[1]/div[2]/div/div/div[22]/div/input

                try
                {
                    //xszbh 行驶证证芯编号
                    IWebElement xszbh = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[1]/div[2]/div/div/div[22]/div/input")); // 在当前的iframe中找到表单元素
                    xszbh.Clear(); // 清空原来的输入值
                    xszbh.SendKeys(str); // 填写新的输入值
                }
                catch
                {


                }
            }
        }

        public void tianJinSongJianRen(string str)
        {
            string[] ss = str.Split(new char[] { '-' });


            if (webDriver != null)
            {
                try
                {

                    //送检人
                    IWebElement sjr = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[3]/div[2]/div/div/div[1]/div/input")); // 在当前的iframe中找到表单元素
                    sjr.Clear(); // 清空原来的输入值
                    sjr.SendKeys(ss[0]); // 填写新的输入值

                    //送检人电话
                    IWebElement sjrphone = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[3]/div[2]/div/div/div[3]/div/input")); // 在当前的iframe中找到表单元素
                    sjrphone.Clear(); // 清空原来的输入值
                    sjrphone.SendKeys(ss[1]); // 填写新的输入值

                    //所有人电话
                    IWebElement syrphone = webDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div/div[2]/div/div/form[2]/div[1]/div/div[3]/div[2]/div/div/div[8]/div/input")); // 在当前的iframe中找到表单元素
                    syrphone.Clear(); // 清空原来的输入值
                    syrphone.SendKeys(ss[1]); // 填写新的输入值
                }
                catch
                {


                }
            }
        }




        #endregion

        //读取txt
        public List<string> readtxt(string path)
        {
            List<string> list = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        list.Add(line);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }

            return list;

        }



        //串口接收函数
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            jsondata = string.Empty;
            zxbh = string.Empty;

            // string s2 = string.Empty;
            //textBox1.Text = string.Empty;

            //等待数据全部接收完成  但是这里可能会有问题  因为有延时，程序在等待的时候，串口设备会又连续发送数据的时候就会影响后续的业务逻辑
            //Thread.Sleep(1000);

            //接收数据
            int n = sp.BytesToRead;
            byte[] buf = new byte[n];
            sp.Read(buf, 0, n);

            //读取完成后清空缓冲区中的数据，避免下次接收到错误的数据                     
            sp.DiscardInBuffer();

            //jsondata = Encoding.UTF8.GetString(buf);
            yewuluoji(Encoding.UTF8.GetString(buf));





        }

        public void yewuluoji(string jsondata)
        {

            //这里解决一下扫码枪的/000026问题
            if (ContainsSubstring(jsondata, "\\000026"))
            {
                jsondata = RemoveSubstring(jsondata, "\\000026");
            }

            ReadOnlyCollection<string> win = webDriver.WindowHandles;

           

            try
            {
                //判断一下长度，确认是征芯标号 还是 信息
                if (jsondata.Length > 29)
                {
                    //msg(Base64ToString(jsondata));
                    //如果info不是空的就清空
                    if (info != null)
                    {
                        info = null;
                    }

                    string s1 = Base64ToString(jsondata);

                    //车辆信息
                    info = JsonConvert.DeserializeObject<Root>(s1);

                    //给复制粘贴
                    copyInfo = info;
                    textBox1.Text = string.Empty;
                    textBox1.AppendText(copyInfo.words_result.号牌号码.words.ToString() + "==》车辆信息准备完毕\r\n");
                    textBox2.Text = copyInfo.words_result.号牌号码.words.ToString();

                    //HY填表
                    //infoTianBiao(info);

                    //保定填表
                    foreach (var item in win)
                    {
                        webDriver.SwitchTo().Window(item);
                        if (webDriver.Url == zhuye)
                        {
                            baodingTianBiao(info);
                            break;
                        }
                    }




                    //天津填表
                    //tianJinTianBiao(info);
                }
                else
                {
                    //如果zxbh不是空的就清空
                    if (zxbh != "")
                    {
                        zxbh = "";
                    }
                    //证芯编号 HY填表
                    zhegnXinTianBiao(jsondata);

                    //证芯标号 保定填表
                    foreach (var item in win)
                    {
                        webDriver.SwitchTo().Window(item);
                        if (webDriver.Url == zhuye)
                        {
                            baodingZhengXinBiao(jsondata);
                            break;
                        }
                    }
                    


                    //证芯标号 天津
                    //tianJinZhengXinBiao(jsondata);

                    //给复制粘贴
                    zxbh = jsondata;
                    if (copyInfo != null)
                    {
                        textBox1.AppendText(copyInfo.words_result.号牌号码.words.ToString() + "==》证芯标号准备完毕\r\n");
                    }

                }

            }
            catch (Exception ex)
            {
                msg(ex.ToString());
            }
        }
        #endregion============================

        #region 查询库的函数====================================

        //分析数据fst
        public string fstData(DataTable dt)
        {
            vehicleInfo vi = new vehicleInfo();

            hphm = dt.Rows[0]["NumberPlate"].ToString();



            //第1行
            //车架号0
            vi.vin = dt.Rows[0]["FactoryNo"].ToString();
            //车牌号0
            vi.hphm = dt.Rows[0]["NumberPlate"].ToString();
            //号牌种类0
            vi.haoPaiZhongLei = dt.Rows[0]["NumberKindID"].ToString();
            //车辆类别0
            #region 判断车辆类别
            string cllbid = "";
            if (dt.Rows[0]["CarSort"].ToString().Contains("客"))
            {
                cllbid = "K";
            }
            else if (dt.Rows[0]["CarSort"].ToString().Contains("货"))
            {
                cllbid = "H";
            }
            else if (dt.Rows[0]["CarSort"].ToString().Contains("挂"))
            {
                cllbid = "D";
            }
            else if (dt.Rows[0]["CarSort"].ToString().Contains("摩"))
            {
                cllbid = "M";
            }
            else
            {
                cllbid = "Z";
            }
            #endregion
            vi.cheLiangLeiBie = cllbid;


            //第2行
            //车辆类型0
            vi.cheLiangLeiXing = dt.Rows[0]["CarKindID"].ToString();
            //使用性质0
            vi.shiYongXingZhi = "2";
            //所属区县0
            #region 所属区县判断
            string xzqh = "";

            //if (dt.Rows[0]["CZDZ"].ToString().Contains("香河县"))
            //{
            //    xzqh = "131024";
            //}
            //else
            //{
            //    xzqh = "XXXXXX";
            //}
            #region 作废的


            //if (dt.Rows[0]["QXMC"].ToString() == "市辖区")
            //{
            //    xzqh = "131001";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "安次区")
            //{
            //    xzqh = "131002";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "广阳区")
            //{
            //    xzqh = "131003";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "固安县")
            //{
            //    xzqh = "131022";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "永清县")
            //{
            //    xzqh = "131023";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "香河县")
            //{
            //    xzqh = "131024";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "大城县")
            //{
            //    xzqh = "131025";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "文安县")
            //{
            //    xzqh = "131026";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "大厂回族自治县")
            //{
            //    xzqh = "131028";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "霸州市")
            //{
            //    xzqh = "131081";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "三河市")
            //{
            //    xzqh = "131082";
            //}
            //else
            //{
            //    xzqh = "XXXXXX";
            //}
            #endregion
            #endregion
            vi.suoShuQuXian = xzqh;
            //是否弄农用车0
            vi.jinRuChengZhenNongYongChe = "0";


            //第3行
            //车主电话0
            vi.cheZhuDianHua = dt.Rows[0]["PhoneNO"].ToString();
            // 登记日期0
            #region 登记日期判断
            DateTime dj = Convert.ToDateTime(dt.Rows[0]["RegisterDate"].ToString() ?? "");
            string djrq = dj.ToString("yyyy-MM-dd");
            #endregion
            vi.dengJiDate = djrq;
            // 出厂日期0
            #region 出厂日期判断
            DateTime cc = Convert.ToDateTime(dt.Rows[0]["FactoryDate"].ToString() ?? "");
            string ccrq = cc.ToString("yyyy-MM-dd");
            #endregion
            vi.chuChangDate = ccrq;
            //使用情况0   2：行驶在用车
            vi.shiYongQingKuang = dt.Rows[0]["CarKindStateID"].ToString();


            //第4行
            //车主单位0
            vi.cheZhuDanWei = dt.Rows[0]["Owner"].ToString();
            //车主地址0
            vi.cheZhuDiZhi = dt.Rows[0]["address"].ToString() ?? "";


            //第5行
            //发动机排量0
            #region 排量判断
            string pl = dt.Rows[0]["PL"].ToString();
            //pl = Math.Round(Convert.ToDecimal(Convert.ToDouble(pl) / 1000), 1, MidpointRounding.AwayFromZero).ToString();
            double d1 = Convert.ToDouble(pl);
            double d2 = d1 / 1000;
            vi.faDongJiPaiLiang = d2.ToString();
            #endregion

            //摩托车四冲程0  0:否，1：是
            vi.moTuoCheSiChongCheng = "0";
            //发动机型号0
            vi.faDongJiXingHao = dt.Rows[0]["EngineModel"].ToString() ?? "";
            //发动机号0
            vi.faDongJiHao = dt.Rows[0]["EngineNumber"].ToString() ?? "";


            //第6行
            //车辆厂牌0
            vi.cheLiangChangPai = dt.Rows[0]["KindName"].ToString() ?? "";
            //车辆型号0
            vi.cheLiangXingHao = dt.Rows[0]["CarType"].ToString() ?? "";
            //汽车制造厂0
            vi.qiCheZhiZaoChang = dt.Rows[0]["zzcmc"].ToString() ?? "";
            //驱动形式0
            #region 驱动形式判断
            string qd = "";
            if (dt.Rows[0]["DriverMode"].ToString().Contains("前"))
            {
                qd = "0";
            }
            else
            {
                qd = "1";
            }
            #endregion
            vi.quDongXingShi = qd;


            //第7行
            //变速箱类型0
            #region 变速箱类型判断
            string bsxlx = "";
            //if (dt.Rows[0]["BSXLX"].ToString() == "MT")
            //{
            //    bsxlx = "0";
            //}
            //else
            //{
            //    bsxlx = "1";
            //}
            #endregion
            vi.bianSuXiangLeiXing = bsxlx;
            //档位个数0
            vi.dangWeiGeShu = "5";
            //气缸数量0
            if (Convert.ToDouble(pl) >= 3 && dt.Rows[0]["FuelState"].ToString().Contains("汽油"))
            {
                vi.qiGangShuLiang = "6";
            }
            else
            {
                vi.qiGangShuLiang = dt.Rows[0]["QGSL"].ToString() ?? "";
            }
            //排气管数0
            vi.paiQiGuanShu = "1";


            //第8行
            //单车轴重0
            vi.danCheZhouZhong = "1000";
            //核定载质量0
            vi.heDingZaiZhi = dt.Rows[0]["LoadWeight"].ToString() ?? "";
            //总质量0
            vi.zongZhiLiang = dt.Rows[0]["FullQuality"].ToString() ?? "";
            //整备质量0
            vi.zhengBeiZhiLiang = dt.Rows[0]["CurbWeight"].ToString() ?? "";

            //第9行
            //载客人数0
            vi.zaiKeRenShu = dt.Rows[0]["Carry"].ToString() ?? "";
            //发动机制造厂0
            vi.faDongJiZhiZaoChang = dt.Rows[0]["zzcmc"].ToString() ?? "";
            //额定转速0
            vi.eDingZhuanSu = dt.Rows[0]["EDZS"].ToString() ?? "";
            //额定功率0
            vi.eDingGongLu = dt.Rows[0]["EnginePower"].ToString() ?? "";


            //第10行
            //是否OBD  0：否  1：是
            vi.isOBD = "1";
            //供油方式0
            #region 供油方式判断
            string gyfsid = "汽油";
            //if (dt.Rows[0]["RYXJG"].ToString() == "化油器")
            //{
            //    gyfsid = "0";
            //}
            //else if (dt.Rows[0]["RYXJG"].ToString() == "闭环电喷")
            //{
            //    gyfsid = "1";
            //}
            //else if (dt.Rows[0]["RYXJG"].ToString() == "开环电喷")
            //{
            //    gyfsid = "2";
            //}
            #endregion
            vi.gongYouFangShi = gyfsid;
            //车身颜色0
            #region 车身颜色判断
            string csys = "";
            if (dt.Rows[0]["Color"].ToString() == "白")
            {
                csys = "A";
            }
            else if (dt.Rows[0]["Color"].ToString() == "灰")
            {
                csys = "B";
            }
            else if (dt.Rows[0]["Color"].ToString() == "黄")
            {
                csys = "C";
            }
            else if (dt.Rows[0]["Color"].ToString() == "粉")
            {
                csys = "D";
            }
            else if (dt.Rows[0]["Color"].ToString() == "红")
            {
                csys = "E";
            }
            else if (dt.Rows[0]["Color"].ToString() == "紫")
            {
                csys = "F";
            }
            else if (dt.Rows[0]["Color"].ToString() == "绿")
            {
                csys = "G";
            }
            else if (dt.Rows[0]["Color"].ToString() == "蓝")
            {
                csys = "H";
            }
            else if (dt.Rows[0]["Color"].ToString() == "棕")
            {
                csys = "I";
            }
            else if (dt.Rows[0]["Color"].ToString() == "黑")
            {
                csys = "J";
            }
            else
            {
                csys = "Z";
            }
            #endregion
            vi.cheShenYanSe = csys;
            //进气方式0
            vi.jinQiFangShi = "0";



            //第11行
            //燃料种类0
            #region 燃料类型判断
            string rl = "";
            if (dt.Rows[0]["FuelState"].ToString() == "汽油")
            {
                rl = "A";
            }
            else if (dt.Rows[0]["FuelState"].ToString() == "柴油")
            {
                rl = "B";
            }
            else
            {
                rl = "AE";
            }
            #endregion
            vi.ranLiaoZhongLei = rl;
            //htmlWindow2.Document.GetElementById("minlmd").InnerText = vehicle.minlmd;
            //htmlWindow2.Document.GetElementById("maxlmd").InnerText = vehicle.maxlmd;
            //htmlWindow2.Document.GetElementById("egr").SetAttribute("value", vehicle.egr);


            //第12行
            //后处理种类
            string hclzl = "1";
            if (rl == "B")
            {
                hclzl = "3";
            }
            vi.houChuLiZhongLei = hclzl;
            //是否有燃油蒸发控制装置
            //htmlWindow2.Document.GetElementById("tg").SetAttribute("value", this.vehicle.tg);
            //DPF
            //htmlWindow2.Document.GetElementById("dpf").SetAttribute("value", this.vehicle.dpf);
            //DPF型号
            //htmlWindow2.Document.GetElementById("dpfxh").InnerText = this.vehicle.dpfxh;


            //第13行
            //SCR
            //htmlWindow2.Document.GetElementById("scr").SetAttribute("value", this.vehicle.scr);
            //SCR型号
            //htmlWindow2.Document.GetElementById("scrxh").InnerText = this.vehicle.scrxh;
            //底盘生产企业
            //htmlWindow2.Document.GetElementById("dpscqy").InnerText = this.vehicle.dpscqy;
            //车辆出厂合格证号
            //htmlWindow2.Document.GetElementById("cchgzh").InnerText = this.vehicle.cchgzh;


            //第14行
            //车辆分类
            string clfl = "M1";
            //if (cllbid == "K")//客车
            //{
            //    if (Convert.ToInt32(dt.Rows[0]["ZKRS"].ToString()) < 9 && Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 5000)
            //    {
            //        clfl = "M1";
            //    }
            //    else if (Convert.ToInt32(dt.Rows[0]["ZKRS"].ToString()) <= 9 && Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 5000)
            //    {
            //        clfl = "M2";
            //    }
            //    else if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) > 5000)
            //    {
            //        clfl = "M3";
            //    }
            //}
            //else//货车
            //{
            //    if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 5000)
            //    {
            //        clfl = "N1";
            //    }
            //    else if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) > 5000 && Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 12000)
            //    {
            //        clfl = "N2";
            //    }
            //    else if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) > 12000)
            //    {
            //        clfl = "N3";
            //    }
            //}
            vi.cheLiangFenLei = clfl;
            //是否电控
            //htmlWindow2.Document.GetElementById("sfdk").SetAttribute("value", this.vehicle.sfdk);
            //是否能关闭车身稳定
            //htmlWindow2.Document.GetElementById("sfgbcswd").SetAttribute("value", this.vehicle.sfgbcswd);
            //电动机型号
            //htmlWindow2.Document.GetElementById("ddjxh").InnerText = this.vehicle.ddjxh;


            //第15行
            //储能装置型号
            //htmlWindow2.Document.GetElementById("cnzzxh").InnerText = this.vehicle.cnzzxh;
            //电池容量(千瓦时)
            //htmlWindow2.Document.GetElementById("dcrl").InnerText = this.vehicle.dcrl;
            //催化转化器型号
            //htmlWindow2.Document.GetElementById("chzhqxh").InnerText = this.vehicle.dcrl;


            string jsondata = JsonConvert.SerializeObject(vi);

            return jsondata;
        }

        //分析数据HY
        public string fenXiData(DataTable dt)
        {
            vehicleInfo vi = new vehicleInfo();

            hphm = dt.Rows[0]["CPH"].ToString();

            vi.lcbdushu = dt.Rows[0]["KilomCount"].ToString();

            //第1行
            //车架号0
            vi.vin = dt.Rows[0]["DPH"].ToString();
            //车牌号0
            vi.hphm = dt.Rows[0]["CPH"].ToString();
            //号牌种类0
            vi.haoPaiZhongLei = dt.Rows[0]["PZLBID"].ToString();
            //车辆类别0
            #region 判断车辆类别
            string cllbid = "";
            if (dt.Rows[0]["CLLBID"].ToString() == "0" || dt.Rows[0]["CLLBID"].ToString() == "1" || dt.Rows[0]["CLLBID"].ToString() == "2")
            {
                cllbid = "K";
            }
            else
            {
                cllbid = "H";
            }
            #endregion
            vi.cheLiangLeiBie = cllbid;


            //第2行
            //车辆类型0
            vi.cheLiangLeiXing = dt.Rows[0]["CLLBXID"].ToString();
            //使用性质0
            vi.shiYongXingZhi = dt.Rows[0]["SYXZID"].ToString();
            //所属区县0
            #region 所属区县判断
            string xzqh = "";

            if (dt.Rows[0]["CZDZ"].ToString().Contains("香河县"))
            {
                xzqh = "131024";
            }
            else
            {
                xzqh = "XXXXXX";
            }
            #region 作废的


            //if (dt.Rows[0]["QXMC"].ToString() == "市辖区")
            //{
            //    xzqh = "131001";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "安次区")
            //{
            //    xzqh = "131002";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "广阳区")
            //{
            //    xzqh = "131003";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "固安县")
            //{
            //    xzqh = "131022";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "永清县")
            //{
            //    xzqh = "131023";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "香河县")
            //{
            //    xzqh = "131024";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "大城县")
            //{
            //    xzqh = "131025";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "文安县")
            //{
            //    xzqh = "131026";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "大厂回族自治县")
            //{
            //    xzqh = "131028";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "霸州市")
            //{
            //    xzqh = "131081";
            //}
            //else if (dt.Rows[0]["QXMC"].ToString() == "三河市")
            //{
            //    xzqh = "131082";
            //}
            //else
            //{
            //    xzqh = "XXXXXX";
            //}
            #endregion
            #endregion
            vi.suoShuQuXian = xzqh;
            //是否弄农用车0
            vi.jinRuChengZhenNongYongChe = "0";


            //第3行
            //车主电话0
            vi.cheZhuDianHua = dt.Rows[0]["CZDH"].ToString();
            // 登记日期0
            #region 登记日期判断
            DateTime dj = Convert.ToDateTime(dt.Rows[0]["DJDate"].ToString() ?? "");
            string djrq = dj.ToString("yyyy-MM-dd");
            #endregion
            vi.dengJiDate = djrq;
            // 出厂日期0
            #region 出厂日期判断
            DateTime cc = Convert.ToDateTime(dt.Rows[0]["MakeDate"].ToString() ?? "");
            string ccrq = cc.ToString("yyyy-MM-dd");
            #endregion
            vi.chuChangDate = ccrq;
            //使用情况0   2：行驶在用车
            vi.shiYongQingKuang = dt.Rows[0]["UseStatus"].ToString();


            //第4行
            //车主单位0
            vi.cheZhuDanWei = dt.Rows[0]["DW"].ToString();
            //车主地址0
            vi.cheZhuDiZhi = dt.Rows[0]["CZDZ"].ToString() ?? "";


            //第5行
            //发动机排量0
            #region 排量判断
            string pl = dt.Rows[0]["PaiLiang"].ToString();
            pl = Math.Round(Convert.ToDecimal(Convert.ToDouble(pl) / 1000), 1, MidpointRounding.AwayFromZero).ToString();
            #endregion
            vi.faDongJiPaiLiang = pl;
            //摩托车四冲程0  0:否，1：是
            vi.moTuoCheSiChongCheng = "0";
            //发动机型号0
            vi.faDongJiXingHao = dt.Rows[0]["FDJXH"].ToString() ?? "";
            //发动机号0
            vi.faDongJiHao = dt.Rows[0]["FDJH"].ToString() ?? "";


            //第6行
            //车辆厂牌0
            vi.cheLiangChangPai = dt.Rows[0]["ChangPH"].ToString() ?? "";
            //车辆型号0
            vi.cheLiangXingHao = dt.Rows[0]["XingHao"].ToString() ?? "";
            //汽车制造厂0
            vi.qiCheZhiZaoChang = dt.Rows[0]["ZZCJ"].ToString() ?? "";
            //驱动形式0
            #region 驱动形式判断
            string qd = "";
            if (dt.Rows[0]["QDXS"].ToString() == "4×2前驱后驻车")
            {
                qd = "0";
            }
            else if (dt.Rows[0]["QDXS"].ToString() == "4×2后驱后驻车")
            {
                qd = "1";
            }
            else if (dt.Rows[0]["QDXS"].ToString() == "4×4分时四驱后驻车")
            {
                qd = "4";
            }
            else if (dt.Rows[0]["QDXS"].ToString() == "4×4全时四驱后驻车")
            {
                qd = "2";
            }
            else if (dt.Rows[0]["QDXS"].ToString() == "4×4适时四驱后驻车")
            {
                qd = "3";
            }
            else
            {
                qd = "1";
            }
            #endregion
            vi.quDongXingShi = qd;


            //第7行
            //变速箱类型0
            #region 变速箱类型判断
            string bsxlx = "";
            if (dt.Rows[0]["BSXLX"].ToString() == "MT")
            {
                bsxlx = "0";
            }
            else
            {
                bsxlx = "1";
            }
            #endregion
            vi.bianSuXiangLeiXing = bsxlx;
            //档位个数0
            vi.dangWeiGeShu = "5";
            //气缸数量0
            if (Convert.ToDouble(pl) >= 3 && dt.Rows[0]["FuelStr"].ToString() == "汽油")
            {
                vi.qiGangShuLiang = "6";
            }
            else
            {
                vi.qiGangShuLiang = dt.Rows[0]["GS"].ToString() ?? "";
            }
            //排气管数0
            vi.paiQiGuanShu = "1";


            //第8行
            //单车轴重0
            vi.danCheZhouZhong = "1000";
            //核定载质量0
            vi.heDingZaiZhi = dt.Rows[0]["ZZL"].ToString() ?? "";
            //总质量0
            vi.zongZhiLiang = dt.Rows[0]["ZCZL"].ToString() ?? "";
            //整备质量0
            vi.zhengBeiZhiLiang = dt.Rows[0]["ZBZL"].ToString() ?? "";

            //第9行
            //载客人数0
            vi.zaiKeRenShu = dt.Rows[0]["ZKRS"].ToString() ?? "";
            //发动机制造厂0
            vi.faDongJiZhiZaoChang = dt.Rows[0]["ZZCJ"].ToString() ?? "";
            //额定转速0
            vi.eDingZhuanSu = dt.Rows[0]["EDGLRPM"].ToString() ?? "";
            //额定功率0
            vi.eDingGongLu = dt.Rows[0]["EDGL"].ToString() ?? "";


            //第10行
            //是否OBD  0：否  1：是
            vi.isOBD = "1";
            //供油方式0
            #region 供油方式判断
            string gyfsid = "";
            if (dt.Rows[0]["RYXJG"].ToString() == "化油器")
            {
                gyfsid = "0";
            }
            else if (dt.Rows[0]["RYXJG"].ToString() == "闭环电喷")
            {
                gyfsid = "1";
            }
            else if (dt.Rows[0]["RYXJG"].ToString() == "开环电喷")
            {
                gyfsid = "2";
            }
            #endregion
            vi.gongYouFangShi = gyfsid;
            //车身颜色0
            #region 车身颜色判断
            string csys = "";
            if (dt.Rows[0]["CarColor"].ToString() == "白")
            {
                csys = "A";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "灰")
            {
                csys = "B";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "黄")
            {
                csys = "C";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "粉")
            {
                csys = "D";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "红")
            {
                csys = "E";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "紫")
            {
                csys = "F";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "绿")
            {
                csys = "G";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "蓝")
            {
                csys = "H";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "棕")
            {
                csys = "I";
            }
            else if (dt.Rows[0]["CarColor"].ToString() == "黑")
            {
                csys = "J";
            }
            else
            {
                csys = "Z";
            }
            #endregion
            vi.cheShenYanSe = csys;
            //进气方式0
            vi.jinQiFangShi = "0";



            //第11行
            //燃料种类0
            #region 燃料类型判断
            string rl = "";
            if (dt.Rows[0]["FuelStr"].ToString() == "汽油")
            {
                rl = "A";
            }
            else if (dt.Rows[0]["FuelStr"].ToString() == "柴油")
            {
                rl = "B";
            }
            else
            {
                rl = "AE";
            }
            #endregion
            vi.ranLiaoZhongLei = rl;
            //htmlWindow2.Document.GetElementById("minlmd").InnerText = vehicle.minlmd;
            //htmlWindow2.Document.GetElementById("maxlmd").InnerText = vehicle.maxlmd;
            //htmlWindow2.Document.GetElementById("egr").SetAttribute("value", vehicle.egr);


            //第12行
            //后处理种类
            string hclzl = "1";
            if (rl == "B")
            {
                hclzl = "3";
            }
            vi.houChuLiZhongLei = hclzl;
            //是否有燃油蒸发控制装置
            //htmlWindow2.Document.GetElementById("tg").SetAttribute("value", this.vehicle.tg);
            //DPF
            //htmlWindow2.Document.GetElementById("dpf").SetAttribute("value", this.vehicle.dpf);
            //DPF型号
            //htmlWindow2.Document.GetElementById("dpfxh").InnerText = this.vehicle.dpfxh;


            //第13行
            //SCR
            //htmlWindow2.Document.GetElementById("scr").SetAttribute("value", this.vehicle.scr);
            //SCR型号
            //htmlWindow2.Document.GetElementById("scrxh").InnerText = this.vehicle.scrxh;
            //底盘生产企业
            //htmlWindow2.Document.GetElementById("dpscqy").InnerText = this.vehicle.dpscqy;
            //车辆出厂合格证号
            //htmlWindow2.Document.GetElementById("cchgzh").InnerText = this.vehicle.cchgzh;


            //第14行
            //车辆分类
            string clfl = "M1";
            if (cllbid == "K")//客车
            {
                if (Convert.ToInt32(dt.Rows[0]["ZKRS"].ToString()) < 9 && Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 5000)
                {
                    clfl = "M1";
                }
                else if (Convert.ToInt32(dt.Rows[0]["ZKRS"].ToString()) <= 9 && Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 5000)
                {
                    clfl = "M2";
                }
                else if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) > 5000)
                {
                    clfl = "M3";
                }
            }
            else//货车
            {
                if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 5000)
                {
                    clfl = "N1";
                }
                else if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) > 5000 && Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) < 12000)
                {
                    clfl = "N2";
                }
                else if (Convert.ToInt32(dt.Rows[0]["ZCZL"].ToString()) > 12000)
                {
                    clfl = "N3";
                }
            }
            vi.cheLiangFenLei = clfl;
            //是否电控
            //htmlWindow2.Document.GetElementById("sfdk").SetAttribute("value", this.vehicle.sfdk);
            //是否能关闭车身稳定
            //htmlWindow2.Document.GetElementById("sfgbcswd").SetAttribute("value", this.vehicle.sfgbcswd);
            //电动机型号
            //htmlWindow2.Document.GetElementById("ddjxh").InnerText = this.vehicle.ddjxh;


            //第15行
            //储能装置型号
            //htmlWindow2.Document.GetElementById("cnzzxh").InnerText = this.vehicle.cnzzxh;
            //电池容量(千瓦时)
            //htmlWindow2.Document.GetElementById("dcrl").InnerText = this.vehicle.dcrl;
            //催化转化器型号
            //htmlWindow2.Document.GetElementById("chzhqxh").InnerText = this.vehicle.dcrl;


            string jsondata = JsonConvert.SerializeObject(vi);

            return jsondata;
        }
        //字符串压缩
        public static string CompressString(string text)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();
            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);
            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }
        //生成二维码
        public string CreateErWeiMa(string data, string path)
        {
            QRCodeGenerator qrd = new QRCodeGenerator();
            QRCodeData qrdata = qrd.CreateQrCode(data, QRCoder.QRCodeGenerator.ECCLevel.L);
            QRCode qr = new QRCode(qrdata);

            Bitmap bp = qr.GetGraphic(15, Color.Black, Color.White, null, 0, 0, false);
            bp.Save(path);

            //pictureBox1.Image = bp;

            return path;
        }

        //获取二维码图片路径
        public string getPat()
        {
            string s1 = Application.StartupPath;
            string s2 = s1 + "\\erweima";

            // 判断文件路径是否存在，不存在则创建文件夹
            if (!Directory.Exists(s2))
            {
                // 不存在就创建目录
                Directory.CreateDirectory(s2);
            }

            return s2;

        }
        #endregion   =========================================


        //窗体加载
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = comboBox2.Items.Count - 2;


            try
            {
                string path = Application.StartupPath;

                List<string> ls = readtxt(path + "\\phone.txt");

                for (int i = 0; i < ls.Count; i++)
                {
                    //string[] s = ls[i].Split('-');

                    songjiandianhua.Items.Add(ls[i]);
                }
                songjiandianhua.SelectedIndex = 1;
            }
            catch
            {

            }


            //获取串口
            try
            {
                foreach (string com in SerialPort.GetPortNames())
                {
                    comboBox1.Items.Add(com);
                }
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
            catch
            {

            }


            #region 判断软件是否注册
            try
            {
                //判断软件是否注册
                RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("mySoftWare").CreateSubKey("Register.INI");
                foreach (string strRNum in retkey.GetSubKeyNames())
                {
                    if (strRNum == softReg.GetRNum())
                    {
                        //MessageBox.Show("此软件已注册！");
                        button4.Visible = false;
                        return;
                    }
                }
                //MessageBox.Show("此软件尚未注册！");
                //this.btnReg.Enabled = true;
                //MessageBox.Show("您现在使用的是试用版，可以免费试用30次！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Int32 tLong;    //已使用次数
                try
                {
                    tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", 0);
                    MessageBox.Show("未注册\r\n已经使用了" + tLong + "次！\r\n共35次！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("欢迎使用本软件！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", 0, RegistryValueKind.DWord);
                }

                //判断是否可以继续试用
                tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", 0);
                if (tLong < 35)
                {
                    int tTimes = tLong + 1;
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\mySoftWare", "UseTimes", tTimes);

                }
                else
                {
                    DialogResult result = MessageBox.Show("试用次数已到！您是否需要注册？", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        button2.Enabled = false;
                        button5.Enabled = false;
                        f2.Show();

                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch
            {
                MessageBox.Show("请尝试右键以管理员身份运行");
            }
            #endregion
        }

        //打开串口
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                if (button2.ForeColor == Color.Red)
                {
                    sp.PortName = comboBox1.Text;//串口号。
                    sp.BaudRate = 9600;//波特率。
                    sp.Parity = Parity.None;//校验位。
                    sp.StopBits = StopBits.One;//停止位。
                    sp.DataBits = 8;//数据位。
                    //sp.ReadBufferSize = 4096;
                    //sp.WriteBufferSize = 4096;
                    sp.Open();//打开串口。


                    sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    msg(comboBox1.Text + "已打开。。。");
                    button2.Text = "关闭";
                    comboBox1.Enabled = false;
                    button2.ForeColor = Color.Green;
                }
                else
                {
                    sp.Close();
                    msg(comboBox1.Text + "已关闭。。。");
                    button2.Text = "打开";
                    button2.ForeColor = Color.Red;
                    comboBox1.Enabled = true;
                    sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
            }
            else
            {
                msg("未检测到串口");
            }
        }

        //开始按钮
        private void button1_Click(object sender, EventArgs e)
        {
            ////关闭黑色cmd窗口
            //ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory.ToString());
            ////关闭黑色cmd窗口
            //driverService.HideCommandPromptWindow = true;

            ////创建配置信息 
            //ChromeOptions options = new ChromeOptions();
            //options.AddArgument(@"--user-data-dir=D:\ChromeUserData");  //缓存
            //options.AddExcludedArgument("enable-automation");//取消 chrome正受到自动测试软件的控制的信息栏

            ////创建webDriver实例并打开浏览器 跳转至制动url
            //webDriver = new ChromeDriver(driverService, options);
            //webDriver.Navigate().GoToUrl(zhuye);

            msg("正在启动，请稍后。。。");

            //关闭黑色cmd窗口
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory.ToString());
            driverService.HideCommandPromptWindow = true;  //关闭黑色cmd窗口

            // 创建 ChromeOptions 实例
            ChromeOptions options = new ChromeOptions();
            options.DebuggerAddress = "localhost:9222";// 添加已经存在的谷歌浏览器实例的标识符
            // 创建ChromeDriver实例
            webDriver = new ChromeDriver(driverService, options);

            button1.Enabled = false;
            msg("启动完毕");
        }


        //注册按钮
        private void button4_Click(object sender, EventArgs e)
        {

            f2.Show();

        }

        //窗体最前
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        //窗体大小改变
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (ControlsInfo.Count > 0)//如果字典中有数据，即窗体改变
            {
                ControlsChaneInit(this.Controls[0]);//表示pannel控件
                ControlsChange(this.Controls[0]);

            }
        }


        //下拉框触发事件
        private void songjiandianhua_SelectedValueChanged(object sender, EventArgs e)
        {


            //保定送检人
            baodingSongJianRen(songjiandianhua.SelectedItem.ToString());

            //HY送检人
            songjianTianBiao(songjiandianhua.SelectedItem.ToString());




            //天津送检人
            tianJinSongJianRen(songjiandianhua.SelectedItem.ToString());


            string[] ss = songjiandianhua.SelectedItem.ToString().Split(new char[] { '-' });

            phone = ss[1];


        }


        #region 复制和输入部分
        ////车辆类型
        //private void button17_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(copyInfo.words_result.车辆类型.words.ToString());
        //                break;
        //            }
        //        }
        //    }
        //}

        ////号牌号码
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    //string s1 = "{\"words_result\":{\"车辆识别代号\":{\"words\":\"LZWACAGAXA4224982\"},\"住址\":{\"words\":\"河北省路坊市香河县城西区新开大街土地月家属小区1034日\"},\"发证单位\":{\"words\":\"河北省廊坊市公安局交通警察支队\"},\"车辆类型\":{\"words\":\"小型面包车\"},\"品牌型号\":{\"words\":\"五菱牌LZW6390A3\"},\"发证日期\":{\"words\":\"20160721\"},\"所有人\":{\"words\":\"阴志成\"},\"号牌号码\":{\"words\":\"冀RP6B61\"},\"使用性质\":{\"words\":\"非营运\"},\"发动机号码\":{\"words\":\"8A60710526\"},\"注册日期\":{\"words\":\"20100722\"}},\"direction\":1,\"words_result_num\":11,\"log_id\":1681597010717386949}";

        //    //Root info = JsonConvert.DeserializeObject<Root>(s1);

        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(copyInfo.words_result.号牌号码.words.ToString());
        //                break;
        //            }
        //        }
        //    }


        //}

        ////所有人
        //private void button6_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(copyInfo.words_result.所有人.words.ToString());
        //                break;
        //            }
        //        }
        //    }
        //}

        ////车架号
        //private void button8_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(copyInfo.words_result.车辆识别代号.words.ToString());
        //                break;
        //            }
        //        }
        //    }
        //}

        ////证芯标号
        //private void button9_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(zxbh);
        //                break;
        //            }
        //        }
        //    }
        //}

        ////住址
        //private void button10_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(copyInfo.words_result.住址.words.ToString());
        //                break;
        //            }
        //        }
        //    }
        //}

        ////发动机号
        //private void button7_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(copyInfo.words_result.发动机号码.words.ToString());
        //                break;
        //            }
        //        }
        //    }

        //}

        ////注册日期
        //private void button11_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);

        //                string s1 = copyInfo.words_result.注册日期.words.ToString();

        //                char[] c = s1.ToCharArray();




        //                SendKeys.Send(c[0].ToString() + c[1].ToString() + c[2].ToString() + c[3].ToString() + "-" + c[4].ToString() + c[5].ToString() + "-" + c[6].ToString() + c[7].ToString());
        //                break;
        //            }
        //        }
        //    }
        //}

        ////发证日期
        //private void button12_Click(object sender, EventArgs e)
        //{

        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);

        //                string s1 = copyInfo.words_result.发证日期.words.ToString();

        //                char[] c = s1.ToCharArray();




        //                SendKeys.Send(c[0].ToString() + c[1].ToString() + c[2].ToString() + c[3].ToString() + "-" + c[4].ToString() + c[5].ToString() + "-" + c[6].ToString() + c[7].ToString());
        //                break;
        //            }
        //        }
        //    }

        //}

        ////电话
        //private void button16_Click(object sender, EventArgs e)
        //{
        //    if (copyInfo != null)
        //    {
        //        while (true)
        //        {
        //            if ((GetAsyncKeyState((int)System.Windows.Forms.Keys.LButton) & 0x8000) != 0)
        //            {
        //                Thread.Sleep(500);
        //                SendKeys.Send(phone);
        //                break;
        //            }
        //        }
        //    }
        //}



        #endregion



        //查询按钮
        private void button14_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "FST")
            {
                string sql = "SELECT * FROM TB_CarInfo WHERE NumberPlate = @cph";
                SqlParameter pms = new SqlParameter("@cph", SqlDbType.VarChar) { Value = textBox2.Text.Trim() };
                DataTable dt = sqlHelper.ExecuteTable(link, sql, pms);


                //查询到数据后执行
                if (dt.Rows.Count > 0)
                {
                    //分析数据
                    string sss = fstData(dt);

                    //压缩
                    string s1 = CompressString(sss);

                    //创建二维码 会返回二维码的径路
                    erweimaPath = CreateErWeiMa(s1, getPat() + "\\QR code.jpg");

                    new Form3(erweimaPath, hphm).ShowDialog();


                }
                else
                {
                    textBox1.Text = "未查询到数据";
                }
            }
            else if (comboBox2.Text == "HY")
            {

                //冀R0EY50
                string sql = "SELECT TOP 1 CPH, PZLBID, PZLBStr, CLLBID, CLLBStr, CLLBXID, CLLBXStr, Cllx,  DW, CZDH, CZDZ, FDJH, DPH, CarColor, SYXZID, SYXZStr, UseStatus, Vin, MakeDate, DJDate, QXMCID, QXMC,  CLJSDAH, ChangPH,  XingHao,  ZZCJ,FDJXH,QDXSID,QDXS, ZCZ, Axis, Fuel, FuelStr, RYXJGID, RYXJG, AshHaveYb, PQGNum, PaiLiang,ZCZL, ZBZL, ZZL,ZKRS, EDGL, EDGLRPM,GS,FDJPL, EDGY,QDLLTGG,BSXLX,GS,KilomCount FROM CarInfo WHERE CPH LIKE @cph";
                SqlParameter pms = new SqlParameter("@cph", SqlDbType.VarChar) { Value = "%" + textBox2.Text.Trim() + "%" };
                DataTable dt = sqlHelper.ExecuteTable(link, sql, pms);

                //查询到数据后执行
                if (dt.Rows.Count > 0)
                {
                    //分析数据
                    string sss = fenXiData(dt);

                    //压缩
                    string s1 = CompressString(sss);

                    //创建二维码 会返回二维码的径路
                    erweimaPath = CreateErWeiMa(s1, getPat() + "\\erweima.jpg");

                    new Form3(erweimaPath, hphm).ShowDialog();


                }
                else
                {
                    textBox1.Text = "未查询到数据";
                }
            }

        }

        //外检单正面
        private void button15_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            //创建行
            dt.Rows.Add();

            //创建列
            //车牌号
            dt.Columns.Add("CPH", typeof(string));
            dt.Rows[0]["CPH"] = "111";

            //里程表
            dt.Columns.Add("cllx", typeof(string));
            dt.Rows[0]["cllx"] = "111";

            //车辆类型
            dt.Columns.Add("lcb", typeof(string));
            dt.Rows[0]["lcb"] = "111";

            //使用性质
            dt.Columns.Add("syxz", typeof(string));
            dt.Rows[0]["syxz"] = "111";

            //出厂日期
            dt.Columns.Add("ccdate", typeof(string));
            dt.Rows[0]["ccdate"] = "111";

            //初次登记日期
            dt.Columns.Add("ccdjdate", typeof(string));
            dt.Rows[0]["ccdjdate"] = "111";

            //检测日期
            dt.Columns.Add("date", typeof(string));
            dt.Rows[0]["date"] = DateTime.Now.ToString("yyyy年MM月dd日");

            //是否四驱
            dt.Columns.Add("issq", typeof(string));
            dt.Rows[0]["issq"] = "111";

            //是否电子手刹
            dt.Columns.Add("isdzss", typeof(string));
            dt.Rows[0]["isdzss"] = "111";

            //是否空气
            dt.Columns.Add("iskqxj", typeof(string));
            dt.Rows[0]["iskqxj"] = "111";

            biaoGe.waijianzhengmian(dt);

        }

        //外检单反面
        private void button5_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            //创建行
            dt.Rows.Add();

            //创建列
            //车主姓名
            dt.Columns.Add("dw", typeof(string));
            dt.Rows[0]["dw"] = "111";
            //车主地址
            dt.Columns.Add("czdz", typeof(string));
            dt.Rows[0]["czdz"] = "111";
            //车主电话
            dt.Columns.Add("czdh", typeof(string));
            dt.Rows[0]["czdh"] = "111";


            biaoGe.waijianfangmian(dt, getPat() + "\\erweima.jpg");




            //string s1 = "{\r\n\t\"words_result\": {\r\n\t\t\"车辆识别代号\": {\r\n\t\t\t\"words\": \"11111122222211\"\r\n\t\t},\r\n\t\t\"住址\": {\r\n\t\t\t\"words\": \"11111111111111111\"\r\n\t\t},\r\n\t\t\"发证单位\": {\r\n\t\t\t\"words\": \"1111111\"\r\n\t\t},\r\n\t\t\"车辆类型\": {\r\n\t\t\t\"words\": \"小型轿车\"\r\n\t\t},\r\n\t\t\"品牌型号\": {\r\n\t\t\t\"words\": \"1111111\"\r\n\t\t},\r\n\t\t\"发证日期\": {\r\n\t\t\t\"words\": \"20230516\"\r\n\t\t},\r\n\t\t\"所有人\": {\r\n\t\t\t\"words\": \"22222222\"\r\n\t\t},\r\n\t\t\"号牌号码\": {\r\n\t\t\t\"words\": \"11111111\"\r\n\t\t},\r\n\t\t\"使用性质\": {\r\n\t\t\t\"words\": \"非营运\"\r\n\t\t},\r\n\t\t\"发动机号码\": {\r\n\t\t\t\"words\": \"111111\"\r\n\t\t},\r\n\t\t\"注册日期\": {\r\n\t\t\t\"words\": \"20040826\"\r\n\t\t}\r\n\t},\r\n\t\"words_result_num\": 11,\r\n\t\"direction\": 0,\r\n\t\"log_id\": 1682965372812672984\r\n}";

            //baodingTianBiao(JsonConvert.DeserializeObject<Root>(s1));
            //baodingZhengXinBiao("123123");

            //tianJinTianBiao(JsonConvert.DeserializeObject<Root>(s1));
            //tianJinZhengXinBiao("11111111111111");



        }

        //申请表
        private void button13_Click(object sender, EventArgs e)
        {


            #region 车辆信息生成二维码
            if (comboBox2.Text == "FST")
            {
                string sqlfst = "SELECT * FROM TB_CarInfo WHERE NumberPlate = @cph";
                SqlParameter pmsfst = new SqlParameter("@cph", SqlDbType.VarChar) { Value = textBox2.Text.Trim() };
                DataTable dtfst = sqlHelper.ExecuteTable(link, sqlfst, pmsfst);


                //查询到数据后执行
                if (dtfst.Rows.Count > 0)
                {
                    //分析数据
                    string sss = fstData(dtfst);

                    //压缩
                    string s1 = CompressString(sss);

                    //创建二维码 会返回二维码的径路
                    erweimaPath = CreateErWeiMa(s1, getPat() + "\\QR code.jpg");

                    new Form3(erweimaPath, hphm).ShowDialog();


                }
                else
                {
                    textBox1.Text = "未查询到数据";
                }
            }
            else if (comboBox2.Text == "HY")
            {

                //冀R0EY50
                string sqlhy = "SELECT TOP 1 CPH, PZLBID, PZLBStr, CLLBID, CLLBStr, CLLBXID, CLLBXStr, Cllx,  DW, CZDH, CZDZ, FDJH, DPH, CarColor, SYXZID, SYXZStr, UseStatus, Vin, MakeDate, DJDate, QXMCID, QXMC,  CLJSDAH, ChangPH,  XingHao,  ZZCJ,FDJXH,QDXSID,QDXS, ZCZ, Axis, Fuel, FuelStr, RYXJGID, RYXJG, AshHaveYb, PQGNum, PaiLiang,ZCZL, ZBZL, ZZL,ZKRS, EDGL, EDGLRPM,GS,FDJPL, EDGY,QDLLTGG,BSXLX,GS,KilomCount FROM CarInfo WHERE CPH LIKE @cph";
                SqlParameter pmshy = new SqlParameter("@cph", SqlDbType.VarChar) { Value = "%" + textBox2.Text.Trim() + "%" };
                DataTable dthy = sqlHelper.ExecuteTable(link, sqlhy, pmshy);

                //查询到数据后执行
                if (dthy.Rows.Count > 0)
                {
                    //分析数据
                    string sss = fenXiData(dthy);

                    //压缩
                    string s1 = CompressString(sss);

                    //创建二维码 会返回二维码的径路
                    erweimaPath = CreateErWeiMa(s1, getPat() + "\\erweima.jpg");




                }
                else
                {
                    textBox1.Text = "未查询到数据";
                }
            }
            #endregion











            //冀R0EY50
            string sql = "SELECT TOP 1 DW,CZDZ,PZLBStr,CPH,CZDH FROM CarInfo WHERE CPH LIKE @cph";
            SqlParameter pms = new SqlParameter("@cph", SqlDbType.VarChar) { Value = "%" + textBox2.Text.Trim() + "%" };
            DataTable dt = sqlHelper.ExecuteTable(link, sql, pms);

            if (dt.Rows.Count == 0)
            {
                return;
            }



            //委托人
            dt.Columns.Add("wtr", typeof(string));
            dt.Rows[0]["wtr"] = songjiandianhua.SelectedItem.ToString().Split(new char[] { '-' })[0];
            //委托人电话
            dt.Columns.Add("wtrdh", typeof(string));
            dt.Rows[0]["wtrdh"] = dt.Rows[0]["CZDH"].ToString();

            ////提示是否是本人
            //DialogResult result = MessageBox.Show("是否是本人", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //if (result == DialogResult.Yes)
            //{
            //    //车主电话
            //    dt.Columns.Add("czdh", typeof(string));
            //    dt.Rows[0]["czdh"] = textBox3.Text;

            //    //委托人
            //    dt.Columns.Add("wtr", typeof(string));
            //    dt.Rows[0]["wtr"] = "";
            //    //委托人电话
            //    dt.Columns.Add("wtrdh", typeof(string));
            //    dt.Rows[0]["wtrdh"] = "";
            //}
            //else
            //{
            //    string[] ss = songjiandianhua.SelectedItem.ToString().Split(new char[] { '-' });

            //    //车主电话
            //    dt.Columns.Add("czdh", typeof(string));
            //    dt.Rows[0]["czdh"] = "";

            //    //委托人
            //    dt.Columns.Add("wtr", typeof(string));
            //    dt.Rows[0]["wtr"] = ss[0];
            //    //委托人电话
            //    dt.Columns.Add("wtrdh", typeof(string));
            //    dt.Rows[0]["wtrdh"] = ss[1];
            //}



            //申请表打钩
            //给dt添加申请的打钩的数据
            string s = dt.Rows[0]["CPH"].ToString();
            //if (s == "冀R")
            //{
            //    dt.Columns.Add("bendi", typeof(string));
            //    dt.Rows[0]["bendi"] = "☑";
            //    dt.Columns.Add("waidi", typeof(string));
            //    dt.Rows[0]["waidi"] = "□";
            //}
            //else
            //{
            //    dt.Columns.Add("bendi", typeof(string));
            //    dt.Rows[0]["bendi"] = "□";
            //    dt.Columns.Add("waidi", typeof(string));
            //    dt.Rows[0]["waidi"] = "☑";
            //}


            if (s == jiancheng)
            {
                dt.Columns.Add("bendi", typeof(string));
                dt.Rows[0]["bendi"] = "√";
                dt.Columns.Add("waidi", typeof(string));
                dt.Rows[0]["waidi"] = "";
            }
            else
            {
                dt.Columns.Add("bendi", typeof(string));
                dt.Rows[0]["bendi"] = "";
                dt.Columns.Add("waidi", typeof(string));
                dt.Rows[0]["waidi"] = "√";
            }
            //年月日
            dt.Columns.Add("date", typeof(string));
            dt.Rows[0]["date"] = DateTime.Now.ToString("yyyy年MM月dd日");


            biaoGe.shenQingBiao(dt, getPat() + "\\erweima.jpg");


        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webDriver != null)
            {
                webDriver.Quit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (webDriver==null)
            {
                msg("没有浏览器");
                return;
            }
            ReadOnlyCollection<string> s1 = webDriver.WindowHandles;
            for (int i = 0; i < s1.Count; i++)
            {
                if (webDriver.Url != denglu)
                {
                    webDriver.SwitchTo().Window(s1[i]);
                    //HY送检人
                    songjianTianBiao(songjiandianhua.SelectedItem.ToString());
                }
                else
                {
                    //HY送检人
                    songjianTianBiao(songjiandianhua.SelectedItem.ToString());



                }
            }



        }
    }
}
