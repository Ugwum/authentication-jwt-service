using FluentValidation;

namespace AuthService.Core.Model.Validators
{
    public class RefreshTokenReqModelValidator : AbstractValidator<RefreshTokenReq>
    {
        public RefreshTokenReqModelValidator()
        {
            RuleFor(model => model.expiredToken).NotNull().NotEmpty().WithMessage("expired token field is required");

        }
    }


}
