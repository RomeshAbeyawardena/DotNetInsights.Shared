using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Library;
using DotNetInsights.Shared.Library.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public abstract class DefaultBaseValidator<TModel> : IValidator<TModel>
    {
        protected abstract void OnValidate(TModel model);

        protected abstract Task OnValidateAsync(TModel model);

        public ValidationResult Validate(TModel model)
        {
            try
            {
                OnValidate(model);
                return Success;
            }
            catch(ValidateException validateException)
            {
                return Fail(validateException.ErrorMessage, validateException.MemberName);
            }
        }

        public async Task<ValidationResult> ValidateAsync(TModel model)
        {
            try
            {
                OnValidate(model);
                await OnValidateAsync(model)
                    .ConfigureAwait(false);
                return Success;
            }
            catch(ValidateException validateException)
            {
                return Fail(validateException.ErrorMessage, validateException.MemberName);
            }
        }

        ValidationResult IValidator.Validate(object model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(!(model is TModel tModel))
                throw new NotSupportedException();

            return Validate(tModel);
        }

        protected IValidate<TModel> ValidateModel(TModel model) => new Validate<TModel>(model, _serviceProvider);

        protected ValidationResult Success => ValidationResult.Success;
        
        protected ValidationResult Fail(string errorMessage, params string[] memberNames)
        {
            return new ValidationResult(errorMessage, memberNames);
        }

        protected DefaultBaseValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;
    }
}