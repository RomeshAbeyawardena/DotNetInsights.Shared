using DotNetInsights.Shared.Library.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetInsights.Shared.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OptionalRequiredAttribute : ValidationAttribute
    {
        private readonly string[] _optionalMembers;
        private readonly int _minimumNumber;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(validationContext == null)
                throw new ArgumentNullException(nameof(validationContext));

            var valueType = validationContext.ObjectType ?? value?.GetType();

            var nullMembersList = new List<string>();
            
            foreach (var optionalMember in _optionalMembers)
            {
                var valueProperty = valueType.GetProperty(optionalMember);

                if(valueProperty == null)
                    return new ValidationResult($"Unable to validate { optionalMember }");

                var valuePropertyValue = valueProperty
                    .GetValue(validationContext.ObjectInstance);
                
                if(valuePropertyValue.IsNullOrDefault())
                    nullMembersList.Add(optionalMember);
            }

            var totalInvalid = nullMembersList.Count;

            if(totalInvalid > _optionalMembers.Length - _minimumNumber)
                return new ValidationResult("Parameter must not be null", nullMembersList.ToArray());

            return ValidationResult.Success;
        }

        public OptionalRequiredAttribute(int minumumNumberOfRequiredMembers = 1, params string[] optionalMembers)
        {
            _minimumNumber = minumumNumberOfRequiredMembers;
            _optionalMembers = optionalMembers;
        }
    }
}
