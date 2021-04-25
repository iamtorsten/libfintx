/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 Abid Hussain
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 	
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace libfintx.FinTS.Util
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
            var sb = new StringBuilder();
            foreach (var property in obj.GetType().GetProperties())
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

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
