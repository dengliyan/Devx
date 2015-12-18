using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    public class DbCommandData
    {
        public DbContext Context { get; private set; }
        public System.Data.IDbCommand InnerCommand { get; private set; }    
        public System.Data.IDataReader Reader { get; set; }

        internal DbCommandExecute Execute { get; set; }

        public DbCommandData(DbContext context, System.Data.IDbCommand innerCommand)
        {
            Context = context;
            InnerCommand = innerCommand;
            InnerCommand.CommandType = System.Data.CommandType.Text; 
        }



    }
}
