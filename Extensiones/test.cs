//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Linq.Expressions;

//namespace Devx.Extensiones
//{
//    public static class MVCLinq
//    {
//        /// <summary>
//        /// 扩展方法，IEnumerable<T>转换为IList<SelectListItem>
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="data">带转换的数据</param>
//        /// <param name="Text"></param>
//        /// <param name="Value"></param>
//        /// <param name="selectValue"></param>
//        /// <param name="NewItem">传递过来的SelectListItem，如请选择……</param>
//        /// <returns></returns>
//        public static IList<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> data, Expression<Func<T, object>> Text, Expression<Func<T, object>> Value, string selectValue = "", SelectListItem NewItem = null) where T : class,new()
//        {
//            var list = new List<SelectListItem>();
//            if (NewItem != null)
//            {
//                list.Add(NewItem);
//            }
//            string _text = "";
//            string _value = "";
//            if (Text.Body is MemberExpression)
//            {
//                MemberExpression TextMember = (MemberExpression)Text.Body;
//                _text = TextMember.Member.Name;
//            }
//            else if (Text.Body is UnaryExpression)
//            {
//                UnaryExpression TextMember = (UnaryExpression)Value.Body;
//                _text = (TextMember.Operand as MemberExpression).Member.Name;
//            }
//            if (Value.Body is MemberExpression)
//            {
//                MemberExpression ValueMember = (MemberExpression)Text.Body;
//                _value = ValueMember.Member.Name;
//            }
//            else if (Value.Body is UnaryExpression)
//            {
//                UnaryExpression ValueMember = (UnaryExpression)Value.Body;
//                _value = (ValueMember.Operand as MemberExpression).Member.Name;
//            }
//            var type = new T().GetType();
//            var TextPropertyInfo = type.GetProperty(_text);
//            var ValuePropertyInfo = type.GetProperty(_value);
//            foreach (var item in data)
//            {
//                var selectItem = new SelectListItem() { Text = TextPropertyInfo.GetValue(item).ToString(), Value = ValuePropertyInfo.GetValue(item).ToString() };
//                if (!string.IsNullOrWhiteSpace(selectValue) && selectValue == selectItem.Value)
//                {
//                    selectItem.Selected = true;
//                }
//                list.Add(selectItem);
//            }

//            return list;
//        }
//    }
//}
