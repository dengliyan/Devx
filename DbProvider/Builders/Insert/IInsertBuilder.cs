namespace Devx.DbProvider
{
    public interface IInsertBuilder
    {
        int Execute();

        T ExecuteReturnLastId<T>();

        IInsertBuilder Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);
        IInsertBuilder Column(string columnName, object value, bool isFunction);
    }
}
