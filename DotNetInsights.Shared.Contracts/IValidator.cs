using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts
{
    public interface IValidator<TModel> : IValidator
    {
        ValidationResult Validate(TModel model);
        Task<ValidationResult> ValidateAsync(TModel model);
    }

    public interface IValidator
    {
        ValidationResult Validate(object model);
    }
}