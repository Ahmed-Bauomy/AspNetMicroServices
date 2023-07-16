using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Exceptions
{
    public class CustomValidationException : ApplicationException
    {
        private Dictionary<string, string[]> Errors { get; }
        public CustomValidationException()
            :base("one or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public CustomValidationException(IEnumerable<ValidationFailure> failures):this()
        {
            Errors = failures.GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                             .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
