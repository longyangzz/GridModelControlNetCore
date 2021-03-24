
#region ����
/*---------------------------------------------------------------------------
 * ����:���ݿ�ͨ�÷��ʽӿ�IDataAccess
 * ����:�Գ������ݷ�������з�װ�����δ������ݿ����Ӷ������ݲ�ѯ��
 *      ��Ȳ������ṩ�Բ�ͬ���ݷ�������ִ����ɾ�Ĳ�����ݿ�Ĳ�����
 *      ���㿪�������ж����ݿ���в���
 * ��д��Ա:zydong
 * ��дʱ��:2006-5
 * �޸�ʱ��:2007-5
 * -------------------------------------------------------------------------*/
#endregion

using System;
using System.Data;
using System.Collections;

namespace DBSupport
{
	/// <summary>
	/// IDataAceess ͨ�����ݿ���ʽӿ�
	/// </summary>
	public interface IDataAccess
	{
		#region ����

		/// <summary>
		/// ���û��ȡ�����ַ���
		/// </summary>
		string ConnectString{get;set;}

		/// <summary>
		/// ��ȡ������Ϣ
		/// </summary>
        string LastError { get; set; }

		#endregion

		#region �򿪡��ر�����
		/// <summary>
		/// ������
		/// </summary>
		void Open();

		/// <summary>
		/// �ر�����
		/// </summary>
		void Close();

		#endregion

        #region ������

        /// <summary>
        /// ��ʼ����,ʧ�ܷ���false;
        /// </summary>
        bool BeginTransaction();

        /// <summary>
        /// �ύ����,ʧ�ܷ���false;
        /// </summary>
        bool CommitTransacton();

        /// <summary>
        /// �ع�����,ʧ�ܷ���false;
        /// </summary>
        bool RollbackTransaction();

        #endregion 

        #region ��ȡ�����б�

        /// <summary>
		/// ���������б�
		/// </summary>
		/// <param name="paramsCount">��������</param>
		/// <returns>�����б�</returns>
		IDataParameter[] CreateParamters(int paramCount);
		#endregion

		#region ִ�зǲ�ѯ����
		/// <summary>
        /// ִ�зǲ�ѯSQL���,ʧ�ܷ���-1;
		/// </summary>
		/// <param name="commandText">�ǲ�ѯ�޲�Sql</param>
        /// <returns>��Ӱ�������,ʧ�ܷ���-1</returns>
		int ExecuteNonQuery(string commandText);

		/// <summary>
        /// ִ�д��ηǲ�ѯSQL���,ʧ�ܷ���-1
		/// </summary>
		/// <param name="commandText">�ǲ�ѯ����Sql���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>��Ӱ�������,ʧ�ܷ���-1</returns>
		int ExecuteNonQuery(string commandText, object[] Params);

		/// <summary>
        /// ִ���޲ηǲ�ѯ�洢����,ʧ�ܷ���-1
		/// </summary>
		/// <param name="strProcduceName">�޲ηǲ�ѯ�洢������</param>
        /// <returns>��Ӱ�������,ʧ�ܷ���-1</returns>
		int ExecuteNonQueryForProcduce(string strProcduceName);
		
		/// <summary>
        /// ִ���вηǲ�ѯ�洢����,ʧ�ܷ���-1
		/// </summary>
		/// <param name="strProcduceName">�вηǲ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>��Ӱ�������,ʧ�ܷ���-1</returns>
		int ExecuteNonQueryForProcduce(string strProcduceName, object[] Params);
       
		#endregion

		#region ִ�в�ѯ��������DataSet����

		/// <summary>
        /// ִ���޲�SQL����DataSet����,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>DataSet����,ʧ�ܷ���null</returns>
		DataSet ExecuteDataSet(string commandText);
		
		/// <summary>
        /// ִ�д��ηǲ�ѯSQL���,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�вβ�ѯSQL���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>DataSet����,ʧ�ܷ���null</returns>
		DataSet ExecuteDataSet(string commandText, object[] Params);

		/// <summary>
        /// ִ���޲β�ѯ�洢����,ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <returns>DataSet����,ʧ�ܷ���null</returns>
		DataSet ExecuteDataSetForProcduce(string strProcduceName);

		/// <summary>
        /// ִ���вβ�ѯ�洢����,ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>DataSet����,ʧ�ܷ���null</returns>
		DataSet ExecuteDataSetForProcduce(string strProcduceName, object[] Params);
		#endregion	

		#region ִ�в�ѯ��������SqlDataReader����

		/// <summary>
        /// ִ���޲β�ѯSQL����SqlDataReader����,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>SqlDataReader����,ʧ�ܷ���null</returns>
		IDataReader ExecuteDataReader(string commandText);
	
		/// <summary>
        /// ִ�д��β�ѯSQL��䷵��SqlDataReader����,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�вβ�ѯSQL���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>SqlDataReader����,ʧ�ܷ���null</returns>
		IDataReader ExecuteDataReader(string commandText, object[] Params);
		
		/// <summary>
        /// ִ���޲β�ѯ�洢���̷���SqlDataReader����,ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <returns>SqlDataReader����,ʧ�ܷ���null</returns>
		IDataReader ExecuteDataReaderForProcduce(string strProcduceName);
		

		/// <summary>
        /// ִ���вβ�ѯ�洢���̷���SqlDataReader����,ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>SqlDataReader����,ʧ�ܷ���null</returns>
		IDataReader ExecuteDataReaderForProcduce(string strProcduceName, object[] Params);

		#endregion	

		#region ִ�в�ѯ�������ص�һֵ

		/// <summary>
        /// ִ���޲β�ѯSQL���ص�һֵ,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>��һֵ,ʧ�ܷ���null</returns>
		object ExecuteScalar(string commandText);

		/// <summary>
        /// ִ�д��β�ѯSQL��䷵�ص�һֵ,ʧ�ܷ���null
		/// </summary>
		/// <param name="commandText">�вβ�ѯSQL���</param>
		/// <param name="Params">�����б�</param>
        /// <returns>��һֵ,ʧ�ܷ���null</returns>
		object ExecuteScalar(string commandText, object[] Params);

		/// <summary>
        /// ִ���޲β�ѯ�洢���̷��ص�һֵ,ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <returns>��һֵ,ʧ�ܷ���null</returns>
		object ExecuteScalarForProcduce(string strProcduceName);

		/// <summary>
        /// ִ���вβ�ѯ�洢���̷��ص�һֵ,ʧ�ܷ���null
		/// </summary>
		/// <param name="strProcduceName">�вβ�ѯ�洢������</param>
		/// <param name="Params">�����б�</param>
        /// <returns>��һֵ,ʧ�ܷ���null</returns>
		object ExecuteScalarForProcduce(string strProcduceName, object[] Params);

		#endregion	

        #region ִ�з���IList

        /// <summary>
        /// ִ���޲β�ѯSQL����IList����,ʧ�ܷ���null
        /// </summary>
        /// <param name="commandText">�޲β�ѯSQL���</param>
        /// <returns>IList����,ʧ�ܷ���null</returns>
        IList List(string commandText);

        /// <summary>
        /// ִ�д��β�ѯSQL��䷵��IList����,ʧ�ܷ���null
        /// </summary>
        /// <param name="commandText">�вβ�ѯSQL���</param>
        /// <param name="Params">�����б�</param>
        /// <returns>IList����,ʧ�ܷ���null</returns>
        IList List(string commandText, object[] Params);

        /// <summary>
        /// ִ���޲β�ѯ�洢���̷���IList����,ʧ�ܷ���null
        /// </summary>
        /// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <returns>IList����,ʧ�ܷ���null</returns>
        IList ListForProcduce(string strProcduceName);


        /// <summary>
        /// ִ���вβ�ѯ�洢���̷���IList����,ʧ�ܷ���null
        /// </summary>
        /// <param name="strProcduceName">�вβ�ѯ�洢������</param>
        /// <param name="Params">�����б�</param>
        /// <returns>IList����,ʧ�ܷ���null</returns>
        IList ListForProcduce(string strProcduceName, object[] Params);


        #endregion

        #region ִ��SQLSERVERBULK����
        /// <summary>
        /// ִ��sqlserverBulk����
        /// </summary>
        /// <param name="dt">����������б�</param>
        /// <param name="tbname">��������ݿ�����</param>
        void BulkToDB(DataTable dt,string tbname);
        #endregion

    }
}
