﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Collections;

namespace DBSupport
{
    public class Access : IDataAccess
    {
        #region 字段、属性

        //连接对象
        private OleDbConnection cnn;

        //命令对象
        private OleDbCommand cmd;

        //数据库连接字符串
        private string connectString = "";

        //错误信息
        private string errorMessage = "";

        /// <summary>
        /// 设置或获取连接字符串
        /// </summary>
        public string ConnectString
        {
            get
            {
                return connectString;
            }
            set
            {
                connectString = value;
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string LastError
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 无参构造
        /// </summary>
        public Access()
        {
            cnn = new OleDbConnection();
            cmd = new OleDbCommand();
            cmd.Connection = cnn;
        }

        /// <summary>
        /// 带参构造
        /// </summary>
        /// <param name="connectString">数据库连接字符传</param>
        public Access(string connectString)
        {
            try
            {
                this.connectString = connectString;
                cnn = new OleDbConnection(connectString);
                cmd = new OleDbCommand();
                cmd.Connection = cnn;
                cnn.Open();
            }
            catch (Exception ex)
            {

            }

        }

        #endregion

        #region 事务处理

        /// <summary>
        /// 开始事务,成功返回true,失败返回false
        /// </summary>
        /// <returns>true成功，false失败</returns>
        public bool BeginTransaction()
        {
            bool ret = false;
            try
            {
                Open();
                cmd.Transaction = cnn.BeginTransaction();
                ret = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 提交事务，成功返回true，失败返回false
        /// </summary>
        /// <returns>成功返回true,失败返回false</returns>
        public bool CommitTransacton()
        {
            bool ret = false;
            try
            {
                cmd.Transaction.Commit();
                Close();
                ret = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 回滚事务，成功返回true,失败返回false
        /// </summary>
        /// <returns>成功返回true,失败返回false</returns>
        public bool RollbackTransaction()
        {
            bool ret = false;
            try
            {
                cmd.Transaction.Rollback();
                Close();
                ret = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return ret;
        }

        #endregion

        #region 打开、关闭连接
        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            errorMessage = "";
            if (cnn.State != ConnectionState.Open)
            {
                cnn.ConnectionString = connectString;
                cnn.Open();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
        }

        #endregion

        #region 创建参数列表

        /// <summary>
        /// 创建参数列表
        /// </summary>
        /// <param name="paramCount">参数个数</param>
        /// <returns>参数列表</returns>
        public IDataParameter[] CreateParamters(int paramCount)
        {
            OleDbParameter[] Params = new OleDbParameter[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                Params[i] = new OleDbParameter();
            }
            return Params;
        }

        #endregion

        #region 创建SqlCommand对象并配置其参数列表(私有方法)

        /// <summary>
        /// 创建SqlCommand对象并配置其参数列表
        /// </summary>
        /// <param name="IsProceduceType">命令对象类型标志</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="Params">参数列表</param>
        /// <returns>配置后的SqlCommand对象</returns>
        private void SetParamters(bool IsProceduceType, string commandText, object[] Params)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = commandText;

            if (IsProceduceType)
            {
                cmd.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                cmd.CommandType = CommandType.Text;
            }

            if (Params != null)
            {
                OleDbParameter[] p = (OleDbParameter[])Params;

                if (p.Length > 0)
                {
                    for (int i = 0; i < Params.Length; i++)
                    {
                        cmd.Parameters.Add(p[i]);
                    }
                }
            }
        }

        #endregion

        #region 执行非查询方法
        /// <summary>
        /// 执行非查询SQL语句,成功返回影响的行数，失败返回-1
        /// </summary>
        /// <param name="commandText">非查询无参Sql</param>
        /// <returns>成功返回影响的行数，失败返回-1</returns>
        public int ExecuteNonQuery(string commandText)
        {
            int ret = -1;

            try
            {
                Open();
                cmd.Transaction = cnn.BeginTransaction();
                string[] strArr = commandText.Split('#');
                for (int i = 0; i < strArr.Length; i++)
                {
                    cmd.CommandText = strArr[i];
                    cmd.ExecuteNonQuery();
                }

                cmd.Transaction.Commit();
                ret = 1;
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行带参非查询SQL语句，成功返回影响的行数，失败返回-1
        /// </summary>
        /// <param name="commandText">非查询带参Sql语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>成功返回影响的行数，失败返回-1<returns>
        public int ExecuteNonQuery(string commandText, object[] Params)
        {
            int ret = -1;
            try
            {
                Open();
                SetParamters(false, commandText, Params);

                ret = cmd.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行无参非查询存储过程，成功返回影响的行数，失败返回-1
        /// </summary>
        /// <param name="strProcduceName">无参非查询存储过程名</param>
        /// <returns>成功返回影响的行数，失败返回-1</returns>
        public int ExecuteNonQueryForProcduce(string strProcduceName)
        {
            int ret = -1;

            try
            {
                Open();
                SetParamters(true, strProcduceName, null);

                ret = cmd.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行有参非查询存储过程，成功返回影响的行数，失败返回-1
        /// </summary>
        /// <param name="strProcduceName">有参非查询存储过程名</param>
        /// <param name="Params">参数列表</param>
        /// <returns>成功返回影响的行数，失败返回-1</returns>
        public int ExecuteNonQueryForProcduce(string strProcduceName, object[] Params)
        {
            int ret = -1;

            try
            {
                Open();
                SetParamters(true, strProcduceName, Params);

                ret = cmd.ExecuteNonQuery();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        #endregion

        #region 执行查询方法返回DataSet对象

        /// <summary>
        /// 执行无参SQL返回DataSet对象，失败返回null
        /// </summary>
        /// <param name="commandText">无参查询SQL语句</param>
        /// <returns>DataSet对象，失败返回null</returns>
        public System.Data.DataSet ExecuteDataSet(string commandText)
        {
            DataSet ret = null;

            try
            {
                cmd.CommandText = commandText;
                OleDbDataAdapter sda = new OleDbDataAdapter(cmd);

                ret = new DataSet();
                sda.Fill(ret);
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行带参查询SQL语句返回DataSet对象，失败返回null
        /// </summary>
        /// <param name="commandText">有参查询SQL语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>DataSet对象，失败返回null</returns>
        public DataSet ExecuteDataSet(string commandText, object[] Params)
        {
            DataSet ret = null;

            try
            {
                SetParamters(false, commandText, Params);

                OleDbDataAdapter sda = new OleDbDataAdapter(cmd);
                ret = new DataSet();
                sda.Fill(ret);
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行无参查询存储过程返回DataSet对象，失败返回null
        /// </summary>
        /// <param name="strProcduceName">有参查询存储过程名</param>
        /// <returns>DataSet对象，失败返回null</returns>
        public DataSet ExecuteDataSetForProcduce(string strProcduceName)
        {
            DataSet ret = null;

            try
            {
                SetParamters(true, strProcduceName, null);

                OleDbDataAdapter sda = new OleDbDataAdapter(cmd);
                ret = new DataSet();
                sda.Fill(ret);
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行有参查询存储过程返回DataSet对象，失败返回null
        /// </summary>
        /// <param name="strProcduceName">有参查询存储过程名</param>
        /// <param name="Params">参数列表</param>
        /// <returns>DataSet对象，失败返回null</returns>
        public DataSet ExecuteDataSetForProcduce(string strProcduceName, object[] Params)
        {
            DataSet ret = null;

            try
            {
                SetParamters(true, strProcduceName, Params);

                OleDbDataAdapter sda = new OleDbDataAdapter(cmd);
                ret = new DataSet();
                sda.Fill(ret);
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        #endregion

        #region 执行查询方法返回SqlDataReader对象

        /// <summary>
        /// 执行无参查询SQL返回SqlDataReader对象，失败返回null
        /// </summary>
        /// <param name="commandText">无参查询SQL语句</param>
        /// <returns>SqlDataReader对象，失败返回null</returns>
        public IDataReader ExecuteDataReader(string commandText)
        {
            IDataReader ret = null;

            try
            {
                Open();
                cmd.CommandText = commandText;

                ret = cmd.ExecuteReader();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                //Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行带参查询SQL语句返回SqlDataReader对象，失败返回null
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>SqlDataReader对象，失败返回null</returns>
        public IDataReader ExecuteDataReader(string commandText, object[] Params)
        {
            IDataReader ret = null;

            try
            {
                Open();
                SetParamters(false, commandText, Params);

                ret = cmd.ExecuteReader();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                //Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行无参查询存储过程返回SqlDataReader对象，失败返回null
        /// </summary>
        /// <param name="strProcduceName">存储过程名</param>
        /// <returns>SqlDataReader对象，失败返回null</returns>
        public IDataReader ExecuteDataReaderForProcduce(string strProcduceName)
        {
            IDataReader ret = null;

            try
            {
                Open();
                SetParamters(true, strProcduceName, null);

                ret = cmd.ExecuteReader();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                //Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行有参查询存储过程返回SqlDataReader对象，失败返回null
        /// </summary>
        /// <param name="strProcduceName">存储过程名</param>
        /// <param name="Params">参数列表</param>
        /// <returns>SqlDataReader对象，失败返回null</returns>
        public IDataReader ExecuteDataReaderForProcduce(string strProcduceName, object[] Params)
        {
            IDataReader ret = null;

            try
            {
                Open();
                SetParamters(true, strProcduceName, Params);

                ret = cmd.ExecuteReader();
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                //Close();
            }

            return ret;
        }

        #endregion

        #region 执行查询方法返回单一值

        /// <summary>
        /// 执行无参查询SQL返回单一值,失败返回null
        /// </summary>
        /// <param name="commandText">无参查询SQL语句</param>
        /// <returns>单一值,失败返回null</returns>
        public object ExecuteScalar(string commandText)
        {
            object ret = null;

            try
            {
                Open();
                cmd.CommandText = commandText;

                object obj = cmd.ExecuteScalar();
                ret = obj == null ? "" : obj;
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行带参查询SQL语句返回单一值，失败返回null
        /// </summary>
        /// <param name="commandText">有参查询SQL语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>返回单一值，失败返回null</returns>
        public object ExecuteScalar(string commandText, object[] Params)
        {
            object ret = null;

            try
            {
                Open();
                SetParamters(false, commandText, Params);

                object obj = cmd.ExecuteScalar();
                ret = obj == null ? "" : obj;
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行无参查询存储过程返回单一值，失败返回null
        /// </summary>
        /// <param name="strProcduceName">有参查询存储过程名</param>
        /// <returns>单一值，失败返回null</returns>
        public object ExecuteScalarForProcduce(string strProcduceName)
        {
            object ret = null;

            try
            {
                Open();
                SetParamters(true, strProcduceName, null);

                object obj = cmd.ExecuteScalar();
                ret = obj == null ? "" : obj;
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        /// <summary>
        /// 执行有参查询存储过程返回单一值，失败返回null
        /// </summary>
        /// <param name="strProcduceName">有参查询存储过程名</param>
        /// <param name="Params">参数列表</param>
        /// <returns>返回单一值，失败返回null</returns>
        public object ExecuteScalarForProcduce(string strProcduceName, object[] Params)
        {
            object ret = null;

            try
            {
                Open();
                SetParamters(true, strProcduceName, Params);

                object obj = cmd.ExecuteScalar();
                ret = obj == null ? "" : obj;
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                Close();
            }

            return ret;
        }

        #endregion

        #region 执行无参查询SQL返回IList对象

        /// <summary>
        /// 执行无参查询SQL返回IList对象,失败返回null
        /// </summary>
        /// <param name="commandText">无参查询SQL语句</param>
        /// <returns>IList对象,,失败返回null</returns>
        public IList List(string commandText)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                cmd.CommandText = commandText;
                OleDbDataReader sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    IList temp = new ArrayList();
                    for (int i = 0; i < sqr.FieldCount; i++)
                    {
                        temp.Add(sqr.GetValue(i));
                    }
                    ret.Add(temp);
                }
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行带参查询SQL语句返回IList对象,失败返回null
        /// </summary>
        /// <param name="commandText">有参查询SQL语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>IList对象,,失败返回null</returns>
        public IList List(string commandText, object[] Params)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                SetParamters(false, commandText, Params);

                OleDbDataReader sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    IList temp = new ArrayList();
                    for (int i = 0; i < sqr.FieldCount; i++)
                    {
                        temp.Add(sqr.GetValue(i));
                    }
                    ret.Add(temp);
                }
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行无参查询存储过程返回IList对象,,失败返回null
        /// </summary>
        /// <param name="strProcduceName">存储过程名</param>
        /// <returns>IList对象,,失败返回null</returns>
        public IList ListForProcduce(string strProcduceName)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                SetParamters(true, strProcduceName, null);

                OleDbDataReader sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    IList temp = new ArrayList();
                    for (int i = 0; i < sqr.FieldCount; i++)
                    {
                        temp.Add(sqr.GetValue(i));
                    }
                    ret.Add(temp);
                }
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        /// <summary>
        /// 执行有参查询存储过程返回IList对象,,失败返回null
        /// </summary>
        /// <param name="strProcduceName">存储过程名</param>
        /// <param name="Params">参数列表</param>
        /// <returns>IList对象,,失败返回null</returns>
        public IList ListForProcduce(string strProcduceName, object[] Params)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                SetParamters(true, strProcduceName, Params);

                OleDbDataReader sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    IList temp = new ArrayList();
                    for (int i = 0; i < sqr.FieldCount; i++)
                    {
                        temp.Add(sqr.GetValue(i));
                    }
                    ret.Add(temp);
                }
            }
            catch (OleDbException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            finally
            {
                Close();
            }
            return ret;
        }

        #endregion

        #region
        public void BulkToDB(DataTable dt, string tbname)
        { 

        }
        #endregion
    }
}
