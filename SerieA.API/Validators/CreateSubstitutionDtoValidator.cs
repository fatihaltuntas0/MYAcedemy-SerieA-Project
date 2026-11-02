using FluentValidation;
using SerieA.API.DTOs.Matches.Events;

namespace SerieA.API.Validators
{
    public class CreateSubstitutionDtoValidator : AbstractValidator<CreateSubstitutionDto>
    {
        public CreateSubstitutionDtoValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Lütfen değişiklik yapan takımı seçin.");
            RuleFor(x => x.PlayerIn).NotEmpty().WithMessage("Oyuna giren futbolcunun adı zorunludur.");
            RuleFor(x => x.PlayerOut).NotEmpty().WithMessage("Oyundan çıkan futbolcunun adı zorunludur.");
            RuleFor(x => x.PlayerIn)
                .NotEqual(x => x.PlayerOut)
                .WithMessage("Oyuna giren oyuncu ile oyundan çıkan oyuncu aynı olamaz.");
            RuleFor(x => x.Minute)
                .NotEmpty().WithMessage("Dakika bilgisi zorunludur.")
                .Matches(@"^\d{1,3}(\+\d{1,2})?$").WithMessage("Geçerli bir dakika giriniz (Örn: 45, 90+3).");
        }
    }
}
