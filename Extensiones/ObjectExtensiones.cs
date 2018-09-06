using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Devx
{

    public static class Cloner
    {
        static Dictionary<Type, Delegate> _cachedIL = new Dictionary<Type, Delegate>();

        public static T Clone<T>(T myObject)
        {
            Delegate myExec = null;

            if (!_cachedIL.TryGetValue(typeof(T), out myExec))
            {
                var dymMethod = new DynamicMethod("DoClone", typeof(T), new Type[] { typeof(T) }, true);
                var cInfo = myObject.GetType().GetConstructor(new Type[] { });

                var generator = dymMethod.GetILGenerator();

                var lbf = generator.DeclareLocal(typeof(T));

                generator.Emit(OpCodes.Newobj, cInfo);
                generator.Emit(OpCodes.Stloc_0);

                foreach (var field in myObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    // Load the new object on the eval stack... (currently 1 item on eval stack)
                    generator.Emit(OpCodes.Ldloc_0);
                    // Load initial object (parameter)          (currently 2 items on eval stack)
                    generator.Emit(OpCodes.Ldarg_0);
                    // Replace value by field value             (still currently 2 items on eval stack)
                    generator.Emit(OpCodes.Ldfld, field);
                    // Store the value of the top on the eval stack into the object underneath that value on the value stack.
                    //  (0 items on eval stack)
                    generator.Emit(OpCodes.Stfld, field);
                }

                // Load new constructed obj on eval stack -> 1 item on stack
                generator.Emit(OpCodes.Ldloc_0);
                // Return constructed object.   --> 0 items on stack
                generator.Emit(OpCodes.Ret);

                myExec = dymMethod.CreateDelegate(typeof(Func<T, T>));

                _cachedIL.Add(typeof(T), myExec);
            }

            return ((Func<T, T>)myExec)(myObject);
        }
    }


    public static class ObjectExtensiones
    {
        private delegate void MemberwiseCopyDelegate(object source, object target);

        private delegate object MemberwiseCloneDelegate(object source);

        private static Dictionary<Type, Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate>> memberwiseCopyDelegates_true = new Dictionary<Type, Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate>>();

        private static Dictionary<Type, Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate>> memberwiseCopyDelegates_false = new Dictionary<Type, Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate>>();

        private static Dictionary<Type, ObjectExtensiones.MemberwiseCloneDelegate> memberwiseCloneDelegates = new Dictionary<Type, ObjectExtensiones.MemberwiseCloneDelegate>();


        /// <summary>
        /// 浅表复制，将源对象中与目标对象同名的公共非静态字段或属性的值复制到目标对象。
        /// 如果字段是值类型的，则对该字段执行逐位复制。 如果字段是引用类型，则复制引用但不复制引用的对象；因此，源对象及当前对象引用同一对象。
        /// 此方法要求 source 类型必须为 TSource 或从其继承，但仅复制源对象中由 TSource 限定的部分字段或属性。
        /// </summary>
        public static void MemberwiseCopy<TSource>(this object source, object target, bool ignoreCase = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (!typeof(TSource).IsAssignableFrom(source.GetType()))
            {
                throw new ArgumentException("源对象的类型不是类型参数指定的类型或其子类。");
            }
            ObjectExtensiones.MemberwiseCopyDelegate copyDelegate = ObjectExtensiones.GetCopyDelegate(typeof(TSource), target.GetType(), ignoreCase);
            if (copyDelegate != null)
            {
                copyDelegate(source, target);
            }
        }

        /// <summary>
        /// 浅表复制，将源对象中与目标对象同名的公共非静态字段或属性的值复制到目标对象。
        /// 如果字段是值类型的，则对该字段执行逐位复制。 如果字段是引用类型，则复制引用但不复制引用的对象；因此，源对象及当前对象引用同一对象。
        /// </summary>
        public static void MemberwiseCopy(this object source, object target, bool ignoreCase = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            ObjectExtensiones.MemberwiseCopyDelegate copyDelegate = ObjectExtensiones.GetCopyDelegate(source.GetType(), target.GetType(), ignoreCase);
            if (copyDelegate != null)
            {
                copyDelegate(source, target);
            }
        }

        private static ObjectExtensiones.MemberwiseCopyDelegate GetCopyDelegate(Type sourceType, Type targetType, bool ignoreCase)
        {
            ObjectExtensiones.MemberwiseCopyDelegate copyDelegate = null;
            Dictionary<Type, Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate>> memberwiseCopyDelegates = ignoreCase ? ObjectExtensiones.memberwiseCopyDelegates_true : ObjectExtensiones.memberwiseCopyDelegates_false;
            lock (memberwiseCopyDelegates)
            {
                if (memberwiseCopyDelegates.ContainsKey(sourceType))
                {
                    Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate> dict = memberwiseCopyDelegates[sourceType];
                    if (dict.ContainsKey(targetType))
                    {
                        copyDelegate = dict[targetType];
                    }
                    else
                    {
                        copyDelegate = ObjectExtensiones.CreateMemberwiseCopyDelegate(sourceType, targetType, ignoreCase);
                        dict[targetType] = copyDelegate;
                    }
                }
                else
                {
                    Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate> dict = new Dictionary<Type, ObjectExtensiones.MemberwiseCopyDelegate>();
                    memberwiseCopyDelegates.Add(sourceType, dict);
                    copyDelegate = ObjectExtensiones.CreateMemberwiseCopyDelegate(sourceType, targetType, ignoreCase);
                    dict[targetType] = copyDelegate;
                }
            }
            return copyDelegate;
        }

        private static string CreateMemberwiseCopyMethodName(Type targetType, bool ignoreCase)
        {
            return targetType.FullName.Replace(".", "_") + string.Format("_{0}", ignoreCase ? 1 : 0);
        }

        private static bool CheckMemberName(MemberInfo sourceMember, MemberInfo targetMember, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return sourceMember.Name == targetMember.Name;
            }
            return sourceMember.Name.Equals(targetMember.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        private static void EnsureMemberType(MemberInfo sourceMember, MemberInfo targetMember, Type sourceMemberType, Type targetMemberType)
        {
            bool throwException = false;
            if (sourceMemberType != targetMemberType)
            {
                if (targetMemberType.IsInterface)
                {
                    if (!targetMemberType.IsAssignableFrom(sourceMemberType))
                    {
                        throwException = true;
                    }
                }
                else if (!sourceMemberType.IsSubclassOf(targetMemberType))
                {
                    throwException = true;
                }
            }
            if (throwException)
            {
                throw new InvalidProgramException(string.Format("在执行 MemberwiseCopy 的过程中，从源对象的 {0} 成员复制数据到目标对象的 {1} 成员时发现错误：源对象和目标对象的同名成员的类型不匹配。", sourceMember.Name, targetMember.Name));
            }
        }

        /// <summary>
        /// 动态创建一个委托并返回
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static ObjectExtensiones.MemberwiseCopyDelegate CreateMemberwiseCopyDelegate(Type sourceType, Type targetType, bool ignoreCase)
        {
            DynamicMethod dynCopyMethod = new DynamicMethod(ObjectExtensiones.CreateMemberwiseCopyMethodName(targetType, ignoreCase), null, new Type[]
			{
				TypeHelper.Object,
				TypeHelper.Object
			}, sourceType);
            MemberInfo[] sourceMembers = sourceType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
            MemberInfo[] targetMembers = targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
            ILGenerator il = dynCopyMethod.GetILGenerator();
            for (int i = 0; i < sourceMembers.Length; i++)
            {
                MemberTypes memberType = sourceMembers[i].MemberType;
                if (memberType != MemberTypes.Field)
                {
                    if (memberType == MemberTypes.Property)
                    {
                        int j = 0;
                        while (j < targetMembers.Length)
                        {
                            if (ObjectExtensiones.CheckMemberName(sourceMembers[i], targetMembers[j], ignoreCase))
                            {
                                MemberTypes memberType2 = targetMembers[j].MemberType;
                                if (memberType2 != MemberTypes.Field)
                                {
                                    if (memberType2 == MemberTypes.Property && ((PropertyInfo)targetMembers[j]).CanWrite)
                                    {
                                        ObjectExtensiones.EnsureMemberType(sourceMembers[i], targetMembers[j], ((PropertyInfo)sourceMembers[i]).PropertyType, ((PropertyInfo)targetMembers[j]).PropertyType);
                                        il.Emit(OpCodes.Ldarg_1);
                                        il.Emit(OpCodes.Ldarg_0);
                                        il.Emit(OpCodes.Callvirt, ((PropertyInfo)sourceMembers[i]).GetGetMethod());
                                        il.Emit(OpCodes.Callvirt, ((PropertyInfo)targetMembers[j]).GetSetMethod());
                                        break;
                                    }
                                    break;
                                }
                                else
                                {
                                    if (!((FieldInfo)targetMembers[j]).IsInitOnly)
                                    {
                                        ObjectExtensiones.EnsureMemberType(sourceMembers[i], targetMembers[j], ((PropertyInfo)sourceMembers[i]).PropertyType, ((FieldInfo)targetMembers[j]).FieldType);
                                        il.Emit(OpCodes.Ldarg_1);
                                        il.Emit(OpCodes.Ldarg_0);
                                        il.Emit(OpCodes.Callvirt, ((PropertyInfo)sourceMembers[i]).GetGetMethod());
                                        il.Emit(OpCodes.Stfld, (FieldInfo)targetMembers[j]);
                                        break;
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                j++;
                            }
                        }
                    }
                }
                else
                {
                    int k = 0;
                    while (k < targetMembers.Length)
                    {
                        if (ObjectExtensiones.CheckMemberName(sourceMembers[i], targetMembers[k], ignoreCase))
                        {
                            MemberTypes memberType3 = targetMembers[k].MemberType;
                            if (memberType3 != MemberTypes.Field)
                            {
                                if (memberType3 == MemberTypes.Property && ((PropertyInfo)targetMembers[k]).CanWrite)
                                {
                                    ObjectExtensiones.EnsureMemberType(sourceMembers[i], targetMembers[k], ((FieldInfo)sourceMembers[i]).FieldType, ((PropertyInfo)targetMembers[k]).PropertyType);
                                    il.Emit(OpCodes.Ldarg_1);
                                    il.Emit(OpCodes.Ldarg_0);
                                    il.Emit(OpCodes.Ldfld, (FieldInfo)sourceMembers[i]);
                                    il.Emit(OpCodes.Callvirt, ((PropertyInfo)targetMembers[k]).GetSetMethod());
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                if (!((FieldInfo)targetMembers[k]).IsInitOnly)
                                {
                                    ObjectExtensiones.EnsureMemberType(sourceMembers[i], targetMembers[k], ((FieldInfo)sourceMembers[i]).FieldType, ((FieldInfo)targetMembers[k]).FieldType);
                                    il.Emit(OpCodes.Ldarg_1);
                                    il.Emit(OpCodes.Ldarg_0);
                                    il.Emit(OpCodes.Ldfld, (FieldInfo)sourceMembers[i]);
                                    il.Emit(OpCodes.Stfld, (FieldInfo)targetMembers[k]);
                                    break;
                                }
                                break;
                            }
                        }
                        else
                        {
                            k++;
                        }
                    }
                }
            }
            il.Emit(OpCodes.Ret);
            return (ObjectExtensiones.MemberwiseCopyDelegate)dynCopyMethod.CreateDelegate(typeof(ObjectExtensiones.MemberwiseCopyDelegate));
        }

        /// <summary>
        /// 浅表复制，将源对象中与目标对象同名的公共非静态字段或属性的值复制到目标对象。
        /// 如果字段是值类型的，则对该字段执行逐位复制。 如果字段是引用类型，则复制引用但不复制引用的对象；因此，源对象及当前对象引用同一对象。
        /// </summary>
        public static T MemberwiseClone<T>(this T source) where T : class
        {
            if (source != null)
            {
                Type sourceType = source.GetType();
                ObjectExtensiones.MemberwiseCloneDelegate cloneDelegate = null;
                lock (ObjectExtensiones.memberwiseCloneDelegates)
                {
                    if (ObjectExtensiones.memberwiseCloneDelegates.ContainsKey(sourceType))
                    {
                        cloneDelegate = ObjectExtensiones.memberwiseCloneDelegates[sourceType];
                    }
                    else
                    {
                        cloneDelegate = ObjectExtensiones.CreateMemberwiseCloneDelegate(sourceType);
                        ObjectExtensiones.memberwiseCloneDelegates[sourceType] = cloneDelegate;
                    }
                }
                if (cloneDelegate != null)
                {
                    return (T)((object)cloneDelegate(source));
                }
            }
            return default(T);
        }

        /// <summary>
        /// 动态创建一个委托并返回
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private static ObjectExtensiones.MemberwiseCloneDelegate CreateMemberwiseCloneDelegate(Type sourceType)
        {
            DynamicMethod dynCopyMethod = new DynamicMethod("_MemberwiseClone", TypeHelper.Object, new Type[]
			{
				TypeHelper.Object
			}, sourceType);
            MemberInfo[] sourceMembers = sourceType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
            ILGenerator il = dynCopyMethod.GetILGenerator();
            ConstructorInfo ci = sourceType.GetConstructor(Type.EmptyTypes);
            il.DeclareLocal(sourceType);
            il.Emit(OpCodes.Newobj, ci);
            il.Emit(OpCodes.Stloc_0);
            for (int i = 0; i < sourceMembers.Length; i++)
            {
                MemberTypes memberType = sourceMembers[i].MemberType;
                if (memberType != MemberTypes.Field)
                {
                    if (memberType == MemberTypes.Property && ((PropertyInfo)sourceMembers[i]).CanWrite)
                    {
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Callvirt, ((PropertyInfo)sourceMembers[i]).GetGetMethod());
                        il.Emit(OpCodes.Callvirt, ((PropertyInfo)sourceMembers[i]).GetSetMethod());
                    }
                }
                else if (!((FieldInfo)sourceMembers[i]).IsInitOnly)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, (FieldInfo)sourceMembers[i]);
                    il.Emit(OpCodes.Stfld, (FieldInfo)sourceMembers[i]);
                }
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return (ObjectExtensiones.MemberwiseCloneDelegate)dynCopyMethod.CreateDelegate(typeof(ObjectExtensiones.MemberwiseCloneDelegate));
        }

        /// <summary>
        /// 调用 Convert.ToXXX(object) 方法将指定对象转换为具有等效值的公共语言运行时类型, 如：Boolean、 SByte、 Byte、 Int16、 UInt16、 Int32、 UInt32、 Int64、 UInt64、 Single、 Double、 Decimal、 DateTime、 Char 和 String等，
        /// 如果对象为 null 或转换过程中发生异常，则返回 defaultValue 参数指定的默认值。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="value">指定的对象。</param>
        /// <param name="defaultValue">默认值，如果不指定，则为目标类型的默认值。</param>
        /// <returns>对象转换后的值。</returns>
        public static T ConvertTo<T>(this object value, T defaultValue) where T : IConvertible
        {
            if (value == null)
            {
                return defaultValue;
            }
            object retValue = ObjectExtensiones.ConvertInternal(typeof(T), value);
            if (retValue == null)
            {
                return defaultValue;
            }
            return (T)((object)retValue);
        }

        private static object ConvertInternal(Type type, object value)
        {
            if (value == null)
            {
                return null;
            }
            Type valueType = value.GetType();
            if (type == valueType)
            {
                return value;
            }
            try
            {
                if (type == TypeHelper.String)
                {
                    object result = ((IConvertible)value).ToString(null);
                    return result;
                }
                if (type.IsPrimitive)
                {
                    if (type == TypeHelper.Int32)
                    {
                        object result = ((IConvertible)value).ToInt32(null);
                        return result;
                    }
                    if (type == TypeHelper.Boolean)
                    {
                        object result = ((IConvertible)value).ToBoolean(null);
                        return result;
                    }
                    if (type == TypeHelper.Char)
                    {
                        object result = ((IConvertible)value).ToChar(null);
                        return result;
                    }
                    if (type == TypeHelper.Int16)
                    {
                        object result = ((IConvertible)value).ToInt16(null);
                        return result;
                    }
                    if (type == TypeHelper.Int64)
                    {
                        object result = ((IConvertible)value).ToInt64(null);
                        return result;
                    }
                    if (type == TypeHelper.SByte)
                    {
                        object result = ((IConvertible)value).ToSByte(null);
                        return result;
                    }
                    if (type == TypeHelper.Single)
                    {
                        object result = ((IConvertible)value).ToSByte(null);
                        return result;
                    }
                    if (type == TypeHelper.Double)
                    {
                        object result = ((IConvertible)value).ToDouble(null);
                        return result;
                    }
                    if (type == TypeHelper.Byte)
                    {
                        object result = ((IConvertible)value).ToByte(null);
                        return result;
                    }
                    if (type == TypeHelper.UInt16)
                    {
                        object result = ((IConvertible)value).ToUInt16(null);
                        return result;
                    }
                    if (type == TypeHelper.UInt32)
                    {
                        object result = ((IConvertible)value).ToUInt32(null);
                        return result;
                    }
                    if (type == TypeHelper.UInt64)
                    {
                        object result = ((IConvertible)value).ToUInt64(null);
                        return result;
                    }
                }
                else
                {
                    if (type == TypeHelper.DateTime)
                    {
                        object result = ((IConvertible)value).ToDateTime(null);
                        return result;
                    }
                    if (type == TypeHelper.Decimal)
                    {
                        object result = ((IConvertible)value).ToDecimal(null);
                        return result;
                    }
                    if (type == TypeHelper.TimeSpan)
                    {
                        object result;
                        if (valueType == TypeHelper.String)
                        {
                            result = ((string)value).ToTimeSpan(default(TimeSpan));
                            return result;
                        }
                        if (valueType == type)
                        {
                            result = value;
                            return result;
                        }
                        result = null;
                        return result;
                    }
                    else if (type.IsEnum)
                    {
                        object result;
                        if (valueType == TypeHelper.String)
                        {
                            result = Enum.Parse(type, (string)value, true);
                            return result;
                        }
                        if (valueType == type)
                        {
                            result = value;
                            return result;
                        }
                        if (valueType.IsPrimitive)
                        {
                            result = Enum.Parse(type, value.ToString(), true);
                            return result;
                        }
                        result = null;
                        return result;
                    }
                    else if (type.IsInterface)
                    {
                        if (type.IsAssignableFrom(valueType))
                        {
                            object result = valueType;
                            return result;
                        }
                    }
                    else if (valueType.IsSubclassOf(type))
                    {
                        object result = value;
                        return result;
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }

    internal class TypeHelper
    {
        public static readonly Type Object = typeof(object);

        public static readonly Type ByteArray = typeof(byte[]);

        public static readonly Type String = typeof(string);

        public static readonly Type Char = typeof(char);

        public static readonly Type Boolean = typeof(bool);

        public static readonly Type SByte = typeof(sbyte);

        public static readonly Type Int16 = typeof(short);

        public static readonly Type Int32 = typeof(int);

        public static readonly Type Int64 = typeof(long);

        public static readonly Type Byte = typeof(byte);

        public static readonly Type UInt16 = typeof(ushort);

        public static readonly Type UInt32 = typeof(uint);

        public static readonly Type UInt64 = typeof(ulong);

        public static readonly Type Decimal = typeof(decimal);

        public static readonly Type Single = typeof(float);

        public static readonly Type Double = typeof(double);

        public static readonly Type DateTime = typeof(DateTime);

        public static readonly Type TimeSpan = typeof(TimeSpan);

        public static readonly Type IList = typeof(IList);

        public static readonly Type IDictionary = typeof(IDictionary);

        public static readonly Type IEnumerable = typeof(IEnumerable);
    }
}
