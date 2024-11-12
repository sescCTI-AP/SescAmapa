using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ExcludeFromValidationAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ConditionalValidationAttribute : ValidationAttribute
{
    private readonly Type _type;

    public ConditionalValidationAttribute(Type type)
    {
        _type = type;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var modelState = validationContext.GetService(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary)) as Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;
        if (modelState != null)
        {
            var propertiesToExclude = _type.GetProperties()
                                            .Where(p => p.GetCustomAttribute<ExcludeFromValidationAttribute>() != null)
                                            .Select(p => p.Name);

            foreach (var propertyName in propertiesToExclude)
            {
                modelState.Remove(propertyName);
            }
        }

        return ValidationResult.Success;
    }
}