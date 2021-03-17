using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Doppelkopf.Core.App.Enums
{
    public static class EnumExtension
    {
        public static string GetText(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? ((DescriptionAttribute)attributes[0]).Description : enumVal.ToString();
        }
    }
}
