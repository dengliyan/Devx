namespace Devx
{
    using System;
    using System.Xml;

    public static class ConvertObjectExtensiones
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string AsString(this XmlNode node, string defaultValue = "", bool isTrim = true)
        {
            if (node == null)
            {
                return defaultValue;
            }
            if (node.InnerText.Trim() == String.Empty)
            {
                return defaultValue;
            }
            return isTrim ? node.InnerText.Trim() : node.InnerText;
        }

        public static string AsString(this XmlNode node, string attributeName, string defaultValue = "", bool isTrim = true)
        {
            if (node == null)
            {
                return defaultValue;
            }
            XmlAttribute attribute = node.Attributes[attributeName];
            if (attribute == null)
            {
                return defaultValue;
            }
            if (attribute.Value.Trim() == String.Empty)
            {
                return defaultValue;
            }
            return isTrim ? attribute.Value.Trim() : attribute.Value;
        }

        public static string AsString(this System.Data.IDataReader reader, string name, string defaultValue = "")
        {
            var o = reader[name];

            return o != null && o != DBNull.Value ? o.ToString().Trim() : defaultValue;
        }
    }

}
