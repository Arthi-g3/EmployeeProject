using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Model.Validation
{
    public class WordCountAttribute :ValidationAttribute
    {
        private readonly int _maxWords;
        public WordCountAttribute(int maxWords)
        {
            _maxWords = maxWords;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string text = value.ToString()!.Trim();

            int wordCount = string.IsNullOrWhiteSpace(text)
                ? 0
                : text.Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries).Length;

            if (wordCount > _maxWords)
            {
                return new ValidationResult(
                    $"Reason cannot exceed {_maxWords} words.");
            }

            return ValidationResult.Success;
        }
    }
}

