using System.Text.RegularExpressions;
using FluentValidation;

namespace Devocean.Core.Extensions;

public static class FluentValidatorExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithMessageEnum<T, TProperty, TEnum>(this IRuleBuilderOptions<T,
        TProperty> ruleBuilderOptions, TEnum enumeration)
        where TEnum : Enum
    {
        var enumValueCode = Convert.ToInt32(enumeration).ToString().PadLeft(3, '0');
        var enumErrorsName = Regex
            .Match(typeof(TEnum).FullName!, @"[^.]+$").Value.Replace("+","");
        
        Console.WriteLine($"{typeof(TEnum).FullName!} - {enumErrorsName}");
        
        var errorCode = $"{enumErrorsName}-{enumValueCode}";
        
        return ruleBuilderOptions
            .WithErrorCode(errorCode)
            .WithMessage(enumeration.GetDescription());
    }
}