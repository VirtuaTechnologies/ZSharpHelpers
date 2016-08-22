using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSharpVault16lib.Global
{
    internal static class ExtensionMethods
    {
        internal static T[] ToSingleArray<T>(this T obj)
        {
            return new T[] { obj };
        }

        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Count() == 0;
        }

        internal static List<T> ShallowCopy<T>(this List<T> origList)
        {
            List<T> newList = new List<T>();
            newList.AddRange(origList);
            return newList;
        }
    }
}
