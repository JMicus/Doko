using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App.Helper
{
    
    public static class ListSerializerExtension
    {
        [Obsolete("User json-serializer")]
        public static List<(T, U)> DokoCodeToList<T, U>(this string code)
        {
            var list = new List<(T, U)>();

            foreach (var entry in code.Split(new[] { "###" }, StringSplitOptions.None))
            {
                var split = entry.Split(new[] { "---" }, StringSplitOptions.None);
                list.Add(((T)Convert.ChangeType(split[0], typeof(T)), (U)Convert.ChangeType(split[1], typeof(U))));
            }
            return list;
        }

        [Obsolete("User json-serializer")]
        public static string ToDokoCode<T, U>(this List<(T, U)> list)
        {
            return string.Join("###", list.Select(x => x.Item1 + "---" + x.Item2));
        }
    }
}
