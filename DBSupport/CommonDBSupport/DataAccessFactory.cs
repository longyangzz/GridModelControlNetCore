


using System;

namespace DBSupport
{
    /// <summary>
    /// DataAccessFactory ͨ�����ݿ���ʶ��󹤳�
    /// </summary>
    public class DataAccessFactory
    {
        public DataAccessFactory()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
        }

        /// <summary>
        /// ���ݿ�����ö��
        /// </summary>
        public enum DBType { MsSql = 0, Oracle, Odbc, SqlServer, Access, SqLite }

        /// <summary>
        /// �������ݿ���ʶ���
        /// </summary>
        /// <param name="dbt">���ݿ�����ö��</param>
        /// <returns>ͨ�����ݿ���ʽӿ�</returns>
        public static IDataAccess CreateDataAccess(DBType dbt)
        {
            switch (dbt)
            {
                case DBType.MsSql:
                    return new MsSql();

                case DBType.Oracle:
                    return new Oracle();

                case DBType.Odbc:
                    return new Odbc();

                case DBType.SqlServer:
                    return new SqlServer();

                case DBType.Access:
                    return new Access();

                case DBType.SqLite:
                    return new SqLite();

                default: return null;
            }
        }

        /// <summary>
        /// �������ݿ���ʶ���
        /// </summary>
        /// <param name="dbt">���ݿ�����ö��</param>
        /// <param name="connectString">�����ַ���</param>
        /// <returns>ͨ�����ݿ���ʽӿ�</returns>
        public static IDataAccess CreateDataAccess(DBType dbt, string connectString)
        {
            switch (dbt)
            {
                case DBType.MsSql:
                    return new MsSql(connectString);

                case DBType.Oracle:
                    return new Oracle(connectString);

                case DBType.Odbc:
                    return new Odbc(connectString);

                case DBType.SqlServer:
                    return new SqlServer(connectString);

                case DBType.Access:
                    return new Access(connectString);

                case DBType.SqLite:
                    return new SqLite(connectString);

                default: return null;
            }
        }
    }
}
