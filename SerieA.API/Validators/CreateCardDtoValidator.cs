using FluentValidation;
using SerieA.API.DTOs.Matches.Events;

namespace SerieA.API.Validators
{
    public class CreateCardDtoValidator : AbstractValidator<CreateCardDto>
    {
        public CreateCardDtoValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Lütfen kart gören takımı seçin.");
            RuleFor(x => x.PlayerName).NotEmpty().WithMessage("Kart gören oyuncunun adı zorunludur.");
            RuleFor(x => x.CardType).IsInEnum().WithMessage("Geçerli bir kart türü seçiniz (Sarı / Kırmızı).");
            RuleFor(x => x.Minute)
                .NotEmpty().WithMessage("Dakika bilgisi zorunludur.")
                .Matches(@"^\d{1,3}(\+\d{1,2})?$").WithMessage("Geçerli bir dakika giriniz (Örn: 45, 90+3).");
        }
    }
}
