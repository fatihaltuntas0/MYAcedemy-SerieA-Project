using FluentValidation;
using SerieA.API.DTOs.Users;

namespace SerieA.API.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad Soyad alanı zorunludur.")
                .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(150).WithMessage("E-posta en fazla 150 karakter olabilir.");
            // Şifre girilmişse en az 8 karakter olmalı, boşsa kontrol edilmez
            When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır.");
            });
        }
    }
}
