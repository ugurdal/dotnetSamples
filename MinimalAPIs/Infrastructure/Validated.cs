using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace MinimalAPIs.Infrastructure;

public class Validated<T>
{
    private ValidationResult Validation { get; }

    private Validated(T value, ValidationResult validation)
    {
        Value = value;
        Validation = validation;
    }
    
    public T Value { get; }
    public bool IsValid => Validation.IsValid;

    public IDictionary<string, string[]> Errors =>
        Validation.Errors.GroupBy(g => g.PropertyName)
            .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());

    public void Deconstruct(out bool isValid, out T value)
    {
        isValid = IsValid;
        value = Value;
    }

    public static async ValueTask<Validated<T>> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        // this is just a prototype
        // only JSON supported
        // TODO: check if the body is empty and json formatted
        var value = await context.Request.ReadFromJsonAsync<T>();
        var validator = context.RequestServices.GetRequiredService<IValidator<T>>();

        if (value is null)
        {
            throw new ArgumentException(parameter.Name);
        }

        var results = await validator.ValidateAsync(value);

        return new Validated<T>(value, results);
    }
}