using System;
using System.Text.RegularExpressions;
using Rsk.Enforcer.Services.DataMasking;

namespace Masking
{
    public class EmailMaskingCategoryAttribute : MaskingCategoryAttribute
    {
        public EmailMaskingCategoryAttribute(string maskingCategory, params string[] additionalMaskingCategories) : base(maskingCategory, additionalMaskingCategories)
        {
            
        }

        public override object GetMaskedValue(object value)
        {
            if (value is string emailAddress)
            {
                string regex = @"[\w-\._\+%]{2}@[\w-\._\+%]{2}";
                return Regex.Replace(emailAddress, regex, "**@****");
            }
            else
            {
                throw new ArgumentException("Email masking can only be applied to strings");
            }
        }
    }
}