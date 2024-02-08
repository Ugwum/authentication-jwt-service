using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Reflection;

namespace Prospa.AuthService.Core.Model.Validators
{

    public interface ICustomValidator
    {
        Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(T model) where T : class;
        ValidationResult GetErrorResult(FluentValidation.Results.ValidationResult validation);
    }
    public class CustomValidator : ICustomValidator
    {
        public ValidationResult GetErrorResult(FluentValidation.Results.ValidationResult validation)
        {
            return new ValidationResult
            {
                code = "VALIDATION_ERROR",
                data = new Data
                {
                    error = validation.Errors?.Select(e => new Error
                    {
                        attribute = e.PropertyName,
                        error = new List<string> { e.ErrorMessage }

                    }).ToList()
                },
                message = "Validation error"
            };
        }
        public async Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(T model) where T : class
        {
            var validator = CreateValidator<T>();

            var context = new ValidationContext<T>(model);

            return await validator.ValidateAsync(context);
        }


        private IValidator CreateValidator<T>() where T : class
        {
            Type targetType = typeof(T);

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            var validatorType = currentAssembly.GetTypes()
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                               type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>) &&
                               targetType.IsAssignableFrom(type.BaseType.GetGenericArguments()[0]))
                .FirstOrDefault();

            if (validatorType == null)
            {
                throw new InvalidOperationException($"Validator for type '{typeof(T).Name}' not found.");
            }
            return (IValidator)Activator.CreateInstance(validatorType);
        }
    }

}
