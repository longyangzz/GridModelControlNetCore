using System;
using System.Data;
using System.Data.OracleClient;
using System.Collections;

namespace DBSupport
{
	/// <summary>
	/// Oracle ���ݿ���ʶ���
	/// </summary>
	internal class Oracle : IDataAccess
	{
        #region �ֶΡ�����

		//���Ӷ���
		private OracleConnection cnn;

        //�������
        private OracleCommand cmd;

		//���ݿ������ַ���
		private string connectString = "";

		//������Ϣ
		private string errorMessage = "";

		/// <summary>
		/// ���û��ȡ�����ַ���
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
		/// ��ȡ������Ϣ
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

		#region ���캯��

		/// <summary>
        /// �޲ι���
		/// </summary>
		public Oracle()
		{
			cnn = new OracleConnection();
            cmd = new OracleCommand();
            cmd.Connection = cnn;
		}

		/// <summary>
        /// ���ι���
		/// </summary>
		/// <param name="connectString">���ݿ������ַ���</param>
		public Oracle(string connectString)
		{
            try
            { 
                this.connectString = connectString;
			cnn = new OracleConnection(connectString);
            cmd = new OracleCommand();
            cmd.Connection = cnn;
			cnn.Open();
            }
            catch(Exception e)
            {
                string aa = e.ToString();
            }
			
		}

		#endregion

        #region ������

        /// <summary>
        /// ��ʼ����,�ɹ�����true,ʧ�ܷ���false
        /// </summary>
        /// <returns>true�ɹ���falseʧ��</returns>
        public bool BeginTransaction()
        {
            bool ret = false;
            try
            {
                Open();
                cmd.Transaction = cnn.BeginTransaction();
                ret = true;
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// �ύ���񣬳ɹ�����true��ʧ�ܷ���false
        /// </summary>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
        /// �ع����񣬳ɹ�����true,ʧ�ܷ���false
        /// </summary>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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

		#region �򿪡��ر�����
		/// <summary>
		/// ������
		/// </summary>
		public void Open()
		{
            errorMessage = "";
			if(cnn.State != ConnectionState.Open)
			{
                //if (cnn.ConnectionString != connectString) {
                //    cnn.ConnectionString = connectString;
                //}
				if(cnn.State == ConnectionState.Closed)
				cnn.Open();
			}
		}

		/// <summary>
		/// �ر�����
		/// </summary>
		public void Close()
		{
			if(cnn.State == ConnectionState.Open)
			{
				cnn.Close();
			}
		}

		#endregion

		#region ���������б�

        /// <summary>
        /// ���������б�
        /// </summary>
        /// <param name="paramCount">��������</param>
        /// <returns>�����б�</returns>
		public IDataParameter[] CreateParamters(int paramCount)
		{
			OracleParameter[] Params = new OracleParameter[paramCount];
			for(int i = 0; i < paramCount; i++)
			{
				Params[i] = new OracleParameter();
			}
			return Params;
		}
 
		#endregion

		#region ����OracleCommand��������������б�(˽�з���)

		/// <summary>
		/// ����OracleCommand��������������б�
		/// </summary>
		/// <param name="IsProceduceType">����������ͱ�־</param>
		/// <param name="commandText">�����ı�</param>
		/// <param name="Params">�����б�</param>
		/// <returns>���ú��OracleCommand����</returns>
		private void SetParamters(bool IsProceduceType, string commandText, object[] Params)
		{
            cmd.Parameters.Clear();
			cmd.CommandText = commandText;
            
			if(IsProceduceType)
			{
				cmd.CommandType = CommandType.StoredProcedure;
			}
			else
			{
				cmd.CommandType = CommandType.Text;
			}

			if(Params != null)
			{
				OracleParameter[] p = (OracleParameter[])Params;
                
				if(p.Length > 0)
				{
					for(int i = 0; i < Params.Length; i++)
					{
						cmd.Parameters.Add(p[i]);
					}
				}
			}
		}

		#endregion

		#region ִ�зǲ�ѯ����
		/// <summary>
        /// ִ�зǲ�ѯSQL���,�ɹ�����Ӱ���������ʧ�ܷ���-1
		/// </summary>
		/// <param name="commandText">�ǲ�ѯ�޲�Sql</param>
        /// <returns>�ɹ�����Ӱ���������ʧ�ܷ���-1</returns>
		public int ExecuteNonQuery(string commandText)
		{
			int ret = -1;

			try
			{
				Open();
                cmd.CommandText = commandText;

				ret = cmd.ExecuteNonQuery();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}
			return ret;
		}

		/// <summary>
        /// ִ�д��ηǲ�ѯSQL��䣬�ɹ�����Ӱ���������ʧ�ܷ���-1
		/// </summary>
		/// <param name="commandText">�ǲ�ѯ����Sql���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>�ɹ�����Ӱ���������ʧ�ܷ���-1<returns>
		public int ExecuteNonQuery(string commandText, object[] Params)
		{
			int ret = - 1;

			try
			{
				Open();
                SetParamters(false, commandText, Params);

				ret = cmd.ExecuteNonQuery();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		/// <summary>
        /// ִ���޲ηǲ�ѯ�洢���̣��ɹ�����Ӱ���������ʧ�ܷ���-1
		/// </summary>
		/// <param name="strProcduceName">�޲ηǲ�ѯ�洢������</param>
        /// <returns>�ɹ�����Ӱ���������ʧ�ܷ���-1</returns>
		public int ExecuteNonQueryForProcduce(string strProcduceName)
		{
			int ret = -1;

			try
			{
				Open();
                SetParamters(true, strProcduceName, null);

				ret = cmd.ExecuteNonQuery();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		/// <summary>
        /// ִ���вηǲ�ѯ�洢���̣��ɹ�����Ӱ���������ʧ�ܷ���-1
		/// </summary>
		/// <param name="strProcduceName">�вηǲ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>�ɹ�����Ӱ���������ʧ�ܷ���-1</returns>
		public int ExecuteNonQueryForProcduce(string strProcduceName, object[] Params)
		{
			int ret = -1;

			try
			{
				Open();
                SetParamters(true, strProcduceName, Params);

				ret = cmd.ExecuteNonQuery();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}
			return ret;
		}

		#endregion

		#region ִ�в�ѯ��������DataSet����

		/// <summary>
		/// ִ���޲�SQL����DataSet����ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>DataSet����ʧ�ܷ���null</returns>
		public System.Data.DataSet ExecuteDataSet(string commandText)
		{
			DataSet ret = null;

			try
			{
                cmd.CommandText = commandText;
                OracleDataAdapter sda = new OracleDataAdapter(cmd);

				ret = new DataSet();
				sda.Fill(ret);
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				ret = null;
			}
            //finally
            //{
            //    Close();
            //}
			return ret;
		}

		/// <summary>
        /// ִ�д��β�ѯSQL��䷵��DataSet����ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�вβ�ѯSQL���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>DataSet����ʧ�ܷ���null</returns>
		public DataSet ExecuteDataSet(string commandText, object[] Params)
		{
			DataSet ret = null;

			try
			{
                SetParamters(false, commandText, Params);

				OracleDataAdapter sda = new OracleDataAdapter(cmd);
				ret = new DataSet();
				sda.Fill(ret);
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				ret = null;
			}
            //finally
            //{
            //    Close();
            //}
			return ret;
		}

		/// <summary>
        /// ִ���޲β�ѯ�洢���̷���DataSet����ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <returns>DataSet����ʧ�ܷ���null</returns>
		public DataSet ExecuteDataSetForProcduce(string strProcduceName)
		{
			DataSet ret = null;
 
			try
			{
                SetParamters(true, strProcduceName, null);

				OracleDataAdapter sda = new OracleDataAdapter(cmd);
				ret = new DataSet();
				sda.Fill(ret);
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				ret = null;
			}
            //finally
            //{
            //    Close();
            //}
			return ret;
		}

		/// <summary>
        /// ִ���вβ�ѯ�洢���̷���DataSet����ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>DataSet����ʧ�ܷ���null</returns>
		public DataSet ExecuteDataSetForProcduce(string strProcduceName, object[] Params)
		{
			DataSet ret = null;

			try
			{
                SetParamters(true, strProcduceName, Params);

				OracleDataAdapter sda = new OracleDataAdapter(cmd);
				ret = new DataSet();
				sda.Fill(ret);
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				ret = null;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		#endregion	

        #region ִ�в�ѯ��������OracleDataReader����

        /// <summary>
        /// ִ���޲β�ѯSQL����OracleDataReader����ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>OracleDataReader����ʧ�ܷ���null</returns>
		public IDataReader ExecuteDataReader(string commandText)
		{
			IDataReader ret = null;

			try
			{
                Open();
                cmd.CommandText = commandText;

				ret = cmd.ExecuteReader();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				//Close();
			}

			return ret;
		}

		/// <summary>
        /// ִ�д��β�ѯSQL��䷵��OracleDataReader����ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">SQL���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>OracleDataReader����ʧ�ܷ���null</returns>
		public IDataReader ExecuteDataReader(string commandText, object[] Params)
		{
			IDataReader ret = null;

			try
			{
				Open();
                SetParamters(false, commandText, Params);

				ret = cmd.ExecuteReader();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				//Close();
			}

			return ret;
		}

		/// <summary>
        /// ִ���޲β�ѯ�洢���̷���OracleDataReader����ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�洢������</param>
        /// <returns>OracleDataReader����ʧ�ܷ���null</returns>
		public IDataReader ExecuteDataReaderForProcduce(string strProcduceName)
		{
			IDataReader ret = null;
 
			try
			{
				Open();
                SetParamters(true, strProcduceName, null);
            
				ret = cmd.ExecuteReader();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				//Close();
			}

			return ret;
		}

		/// <summary>
        /// ִ���вβ�ѯ�洢���̷���OracleDataReader����ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>OracleDataReader����ʧ�ܷ���null</returns>
		public IDataReader ExecuteDataReaderForProcduce(string strProcduceName, object[] Params)
		{
			IDataReader ret = null;

			try
			{
				Open();
                SetParamters(true, strProcduceName, Params);

				ret = cmd.ExecuteReader();
			}
			catch(OracleException e)
			{
				errorMessage = e.Message;
				//Close();
			}

			return ret;
		}

		#endregion	

		#region ִ�в�ѯ�������ص�һֵ

		/// <summary>
        /// ִ���޲β�ѯSQL���ص�һֵ,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�޲β�ѯSQL���</param>
		/// <returns>��һֵ,ʧ�ܷ���null</returns>
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
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		/// <summary>
        /// ִ�д��β�ѯSQL��䷵�ص�һֵ��ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�вβ�ѯSQL���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>���ص�һֵ��ʧ�ܷ���null</returns>
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
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		/// <summary>
        /// ִ���޲β�ѯ�洢���̷��ص�һֵ��ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <returns>��һֵ��ʧ�ܷ���null</returns>
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
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		/// <summary>
        /// ִ���вβ�ѯ�洢���̷��ص�һֵ��ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>���ص�һֵ��ʧ�ܷ���null</returns>
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
			catch(OracleException e)
			{
				errorMessage = e.Message;
			}
            //finally
            //{
            //    Close();
            //}

			return ret;
		}

		#endregion	
	
        #region ִ���޲β�ѯSQL����IList����

        /// <summary>
        /// ִ���޲β�ѯSQL����IList����,ʧ�ܷ���null
        /// </summary>
        /// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>IList����,,ʧ�ܷ���null</returns>
        public IList List(string commandText)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                cmd.CommandText = commandText;
                OracleDataReader sqr = cmd.ExecuteReader();
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
            catch (OracleException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            //finally 
            //{
            //    Close();
            //}
            return ret;
        }

        /// <summary>
        /// ִ�д��β�ѯSQL��䷵��IList����,ʧ�ܷ���null
        /// </summary>
        /// <param name="commandText">�вβ�ѯSQL���</param>
        /// <param name="Params">�����б�</param>
        /// <returns>IList����,,ʧ�ܷ���null</returns>
        public IList List(string commandText, object[] Params)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                SetParamters(false, commandText, Params);

                OracleDataReader sqr = cmd.ExecuteReader();
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
            catch (OracleException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            //finally
            //{
            //    Close();
            //}
            return ret;
        }

        /// <summary>
        /// ִ���޲β�ѯ�洢���̷���IList����,,ʧ�ܷ���null
        /// </summary>
        /// <param name="strProcduceName">�洢������</param>
        /// <returns>IList����,,ʧ�ܷ���null</returns>
        public IList ListForProcduce(string strProcduceName)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                SetParamters(true, strProcduceName, null);

                OracleDataReader sqr = cmd.ExecuteReader();
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
            catch (OracleException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            //finally
            //{
            //    Close();
            //}
            return ret;
        }

        /// <summary>
        /// ִ���вβ�ѯ�洢���̷���IList����,,ʧ�ܷ���null
        /// </summary>
        /// <param name="strProcduceName">�洢������</param>
        /// <param name="Params">�����б�</param>
        /// <returns>IList����,,ʧ�ܷ���null</returns>
        public IList ListForProcduce(string strProcduceName, object[] Params)
        {
            IList ret = new ArrayList();

            try
            {
                Open();
                SetParamters(true, strProcduceName, Params);

                OracleDataReader sqr = cmd.ExecuteReader();
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
            catch (OracleException e)
            {
                errorMessage = e.Message;
                ret = null;
            }
            //finally
            //{
            //    Close();
            //}
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
