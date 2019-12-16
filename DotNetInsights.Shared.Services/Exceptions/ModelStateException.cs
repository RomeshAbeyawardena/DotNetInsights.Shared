using DotNetInsights.Shared.Contracts.Builders;
using DotNetInsights.Shared.Services.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetInsights.Shared.Services.Exceptions
{
    public sealed class ModelStateException : Exception
    {
        public ModelStateException(Action<IDictionaryBuilder<string, string>> memberExceptions)
        {
            ModelStateBuilder = DictionaryBuilder.Create<string, string>();
            memberExceptions?.Invoke(ModelStateBuilder);
        }

        public ModelStateException(ValidationResult validationResult)
        {
            if(validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            ModelStateBuilder = DictionaryBuilder.Create<string, string>();
            foreach(var validationResultMember in validationResult.MemberNames) 
                ModelStateBuilder.Add(validationResultMember, validationResult.ErrorMessage);
        }

        private ModelStateException()
        {

        }

        private ModelStateException(string message)
            : base(message)
        {

        }

        private ModelStateException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public IDictionaryBuilder<string, string> ModelStateBuilder { get; private set; }
    }
}
