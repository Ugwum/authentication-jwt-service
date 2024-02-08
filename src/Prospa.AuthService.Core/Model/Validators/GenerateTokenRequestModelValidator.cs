using FluentValidation;

namespace Prospa.AuthService.Core.Model.Validators
{
    public class GenerateTokenRequestModelValidator : AbstractValidator<GenerateTokenRequest>
    {
        public GenerateTokenRequestModelValidator()
        {
            RuleFor(model => model.username).NotNull().NotEmpty().WithMessage("username field is required");
            RuleFor(model => model.password).NotNull().NotEmpty().WithMessage("password field is required");
        }
    }


}
