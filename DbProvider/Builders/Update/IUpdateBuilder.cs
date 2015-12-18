namespace Devx.DbProvider
{
	public interface IUpdateBuilder
	{
		int Execute();
        IUpdateBuilder Column(string columnName, object value, bool isFunction);
        IUpdateBuilder Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);
        IUpdateBuilder Where(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);
	}

    
}