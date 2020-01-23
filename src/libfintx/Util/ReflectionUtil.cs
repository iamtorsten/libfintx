using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace libfintx.Util
{
    public static class ReflectionUtil
    {
        public static void ResetStaticFields(Type type)
        {
            var propList = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var prop in propList)
            {
                if (prop.CanWrite)
                {
                    if (prop.PropertyType.IsValueType)
                        prop.SetValue(null, Activator.CreateInstance(prop.PropertyType), null);
                    else
                        prop.SetValue(null, null, null);
                }
            }
        }

        public static string ToString(object obj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (System.Reflection.PropertyInfo property in obj.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");
                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    object value = property.GetValue(obj, null);
                    if (value is string str)
                    {
                        sb.Append(str);
                    }
                    else if (value is System.Collections.IEnumerable array)
                    {
                        var enumOfObjects = array as IList<object> ?? array.Cast<object>().ToList();
                        sb.Append($"[{string.Join(", ", enumOfObjects)}]");
                    }
                    else
                    {
                        sb.Append(value);
                    }
                }

                sb.Append(System.Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
