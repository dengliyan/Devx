using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx
{
    public class HttpParameter
    {
        private Dictionary<string, string> _m = null;

        public HttpParameter(string parameterName)
        {
            string parameter = WebHelper.Form(parameterName);
            parameter = Uri.UnescapeDataString(parameter);
            this._m = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(parameter))
            {
                string[] joins = parameter.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var c in joins)
                {
                    string[] equality = c.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (equality.Length == 2)
                    {
                        this._m[equality[0]] = equality[1];
                    }
                }
            }
        }

        public string Parse(string name)
        {
            string value = string.Empty;

            return this._m.TryGetValue(name, out value) ? value : "";
        }
    }
}
