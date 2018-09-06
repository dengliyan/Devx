using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx
{
    public static class DataReaderExtensiones
    {
        public static string AsString(this System.Data.IDataReader reader, string name, string defaultValue = "")
        {
            var o = reader[name];

            return o != null && o != DBNull.Value ? o.ToString().Trim() : defaultValue;
        }
        public static int AsInt(this System.Data.IDataReader reader, string name, int defaultValue = 0)
        {
            var o = reader[name];

            return o != null && o != DBNull.Value ? reader.GetInt32(reader.GetOrdinal(name)) : defaultValue;
        }
        public static Int64 AsInt64(this System.Data.IDataReader reader, string name, long defaultValue=0)
        {
            var o = reader[name];

            return o != null && o != DBNull.Value ? reader.GetInt64(reader.GetOrdinal(name)) : defaultValue;
        }
        public static float AsFloat(this System.Data.IDataReader reader, string name, float defaultValue = 0f)
        {
            var o = reader[name];

            return o != null && o != DBNull.Value ? reader.GetFloat(reader.GetOrdinal(name)) : defaultValue;
        }

        public static double AsDouble(this System.Data.IDataReader reader, string name, double defaultValue = 0d)
        {
            var o = reader[name];
            
            return o != null && o != DBNull.Value ? reader.GetDouble(reader.GetOrdinal(name)) : defaultValue;
        }

        public static bool AsBoolean(this System.Data.IDataReader reader, string name, bool defaultValue = false)
        {
            var o = reader[name];
            
            return o != null && o != DBNull.Value ? reader.GetBoolean(reader.GetOrdinal(name)) : defaultValue;
        }

        public static DateTime AsDateTime(this System.Data.IDataReader reader, string name, DateTime defaultValue)
        {
            var o = reader[name];

            return o != null && o != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal(name)) : defaultValue;
        }

    }
}
