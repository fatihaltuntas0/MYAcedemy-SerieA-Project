using FluentValidation;
using SerieA.API.DTOs.Users;

namespace SerieA.API.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad Soyad alanı zorunludur.")
                .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(150).WithMessage("E-posta en fazla 150 karakter olabilir.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı zorunludur.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.");
        }
    }
}
