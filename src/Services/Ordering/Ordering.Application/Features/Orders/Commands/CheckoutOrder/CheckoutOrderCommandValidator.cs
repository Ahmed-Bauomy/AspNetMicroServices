using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(t => t.UserName)
                .NotEmpty().WithMessage("{UserName} is required")
                .NotNull()
                .MaximumLength(50).WithMessage("{UserName} length must not exceed 50 characters.");

            RuleFor(t => t.EmailAddress)
                .NotEmpty().WithMessage("{EmailAddress} is required")
                .NotNull()
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible);

            RuleFor(t => t.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required")
                .GreaterThan(0).WithMessage("{TotalPrice} must be greater than Zero");
        }
    }
}
