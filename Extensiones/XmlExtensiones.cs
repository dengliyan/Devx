using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Devx
{
    public static  class XmlExtensiones
    {


        public static string Value(this XElement e, string name)
        {
            if (e == null) return null;

            var x = e.Element(name);

            if (x == null || x.Value == null) return null;

            return x.Value.Trim();
        }

        public static string Attr(this XElement e, string name, string attr)
        {
            if (e == null) return null;

            var x = e.Element(name);

            if (x == null) return null;

            var a = x.Attribute(attr);

            if (a == null) return null;

            return a.Value.Trim();
        }

        public static string Attr(this XElement e, string attr)
        {
            if (e == null) return null;

            var a = e.Attribute(attr);

            if (a == null) return null;

            return a.Value.Trim();
        }
    }
}
