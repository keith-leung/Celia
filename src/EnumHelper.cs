using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SharpCC.UtilityFramework
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var attr = enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>();
            if (attr != null)
                return attr.Name;

            return enumValue.ToString();
        }
    }
}
