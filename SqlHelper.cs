using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace HYAnJianDengLu
{
    public static class sqlHelper
    {
        //执行增删改
        public static int ExecuteNonQuery(string link, string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(link))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }


        //执行查询，返回单个值的方法ExecuteScalar()
        public static object ExecuteScalar(string link, string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(link))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        //执行查询，返回多行 多列方法	ExecuteReader()


        //用 SqlDataAdapter返回datatable方法
        public static DataTable ExecuteTable(string link, string sql, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter ap = new SqlDataAdapter(sql, link))
            {
                if (pms != null)
                {
                    ap.SelectCommand.Parameters.AddRange(pms);
                }
                ap.Fill(dt);
            }
            return dt;
        }

    }
}
