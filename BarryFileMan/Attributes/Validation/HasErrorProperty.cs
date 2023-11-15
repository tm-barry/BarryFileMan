using System.ComponentModel.DataAnnotations;

namespace BarryFileMan.Attributes.Validation
{
    public class HasErrorProperty : ValidationAttribute
    {
        public string PropertyName { get; }

        public HasErrorProperty(string propertyName)
        {
            PropertyName = propertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var errorValue = instance.GetType().GetProperty(PropertyName)?.GetValue(instance);

            if (errorValue != null && errorValue is string errorValueStr && !string.IsNullOrWhiteSpace(errorValueStr)) 
            {
                return new ValidationResult(errorValueStr);
            }

            return ValidationResult.Success;
        }
    }
}
