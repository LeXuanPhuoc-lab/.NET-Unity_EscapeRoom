using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EscapeRoomAPI.Extensions;

public static class ValidationExtension
{
    public static ValidationProblemDetails ToProblemDetails(this ValidationResult result)
    {
        // Init ValidationProblemDetails 
        var error = new ValidationProblemDetails() { Status = StatusCodes.Status400BadRequest };

        // Each error in ValidationResult.Errors is ValidationFailure -> (Property, ErrorMessage)
        foreach (var validationFailure in result.Errors)
        {
            // If error property already exist
            if (error.Errors.ContainsKey(validationFailure.PropertyName))
            {
                // From key -> get value and concat with new error
                error.Errors[validationFailure.PropertyName] =
                    error.Errors[validationFailure.PropertyName]
                        .Concat(new[] { validationFailure.ErrorMessage }).ToArray();
            }
            else // not exist property
            {
                error.Errors.Add(new KeyValuePair<string, string[]>(
                    validationFailure.PropertyName,
                    new[] { validationFailure.ErrorMessage }));
            }
        }

        return error;
    }

    public static async Task<ValidationProblemDetails?> ValidateAsync<TRequest> (this TRequest request, IServiceProvider serviceProvider)
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();

        if (validator is null) throw new InvalidOperationException($"No validator found for type {typeof(TRequest).Name}");

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return result.ToProblemDetails();
        }

        return null!;
    }
}