using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HYAnJianDengLu
{
    
    public class 车辆识别代号
    {
        /// <summary>
        /// 
        /// </summary>
        public string words { get; set; }
    }

    public class 住址
    {
        /// <summary>
        /// 河北省廊坊市香河县淑阳镇郭辛庄村71号
        /// </summary>
        public string words { get; set; }
    }

    public class 发证单位
    {
        /// <summary>
        /// 河北省廊坊市公安局交通警察支队
        /// </summary>
        public string words { get; set; }
    }

    public class 车辆类型
    {
        /// <summary>
        /// 小型轿车
        /// </summary>
        public string words { get; set; }
    }

    public class 品牌型号
    {
        /// <summary>
        /// 三菱牌DN7162H5
        /// </summary>
        public string words { get; set; }
    }

    public class 发证日期
    {
        /// <summary>
        /// 
        /// </summary>
        public string words { get; set; }
    }

    public class 所有人
    {
        /// <summary>
        /// 郭瑞龙
        /// </summary>
        public string words { get; set; }
    }

    public class 号牌号码
    {
        /// <summary>
        /// 冀R1622X
        /// </summary>
        public string words { get; set; }
    }

    public class 使用性质
    {
        /// <summary>
        /// 非营运
        /// </summary>
        public string words { get; set; }
    }

    public class 发动机号码
    {
        /// <summary>
        /// 
        /// </summary>
        public string words { get; set; }
    }

    public class 注册日期
    {
        /// <summary>
        /// 
        /// </summary>
        public string words { get; set; }
    }

    public class Words_result
    {
        /// <summary>
        /// 
        /// </summary>
        public 车辆识别代号 车辆识别代号 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 住址 住址 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 发证单位 发证单位 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 车辆类型 车辆类型 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 品牌型号 品牌型号 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 发证日期 发证日期 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 所有人 所有人 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 号牌号码 号牌号码 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 使用性质 使用性质 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 发动机号码 发动机号码 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public 注册日期 注册日期 { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Words_result words_result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string direction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string words_result_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string log_id { get; set; }
    }
}
