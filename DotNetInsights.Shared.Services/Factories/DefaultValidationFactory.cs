using Microsoft.Extensions.DependencyInjection;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.Factories
{
    public class DefaultValidationFactory : IValidationFactory
    {
        public IValidator<TModel> GetValidator<TModel>()
        {
            return _serviceProvider.GetRequiredService<IValidator<TModel>>();
        }

        public ValidationResult Validate<TModel>(TModel model)
        {
            var modelValidator = GetValidator<TModel>();
            return modelValidator.Validate(model);
        }

        public async Task<ValidationResult> ValidateAsync<TModel>(TModel model)
        {
            var modelValidator = GetValidator<TModel>();
            return await modelValidator.ValidateAsync(model)
                .ConfigureAwait(false);
        }

        public DefaultValidationFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;
    }
}