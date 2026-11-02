using FluentValidation;
using SerieA.API.DTOs.Matches.Events;

namespace SerieA.API.Validators
{
    public class CreateGoalDtoValidator : AbstractValidator<CreateGoalDto>
    {
        public CreateGoalDtoValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Lütfen golü atan takımı seçin.");
            RuleFor(x => x.PlayerName).NotEmpty().WithMessage("Golü atan oyuncunun adı zorunludur.");
            RuleFor(x => x.Minute)
                .NotEmpty().WithMessage("Dakika bilgisi zorunludur.")
                .Matches(@"^\d{1,3}(\+\d{1,2})?$").WithMessage("Geçerli bir dakika giriniz (Örn: 45, 90+3).");
        }
    }
}
