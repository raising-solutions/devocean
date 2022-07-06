using System.ComponentModel;
using System.Reflection;

namespace Devocean.Core.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum enumeration)
        where TEnum : Enum
    {
        FieldInfo? fieldInfo = enumeration.GetType().GetField(enumeration.ToString());
        if (fieldInfo == null)
            return enumeration.ToString();

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo
            .GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length > 0
            ? attributes[0].Description
            : enumeration.ToString();
    }
}