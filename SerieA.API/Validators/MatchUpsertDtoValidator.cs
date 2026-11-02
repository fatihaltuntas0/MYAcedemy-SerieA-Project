using FluentValidation;
using SerieA.API.DTOs.Matches;
using SerieA.API.Enums;

namespace SerieA.API.Validators
{
    public class MatchUpsertDtoValidator : AbstractValidator<MatchUpsertDto>
    {
        public MatchUpsertDtoValidator()
        {
            RuleFor(x => x.HomeTeamId)
                .NotEmpty().WithMessage("Ev sahibi takım seçilmelidir.");
            RuleFor(x => x.AwayTeamId)
                .NotEmpty().WithMessage("Deplasman takımı seçilmelidir.")
                .NotEqual(x => x.HomeTeamId).WithMessage("Ev sahibi takım ile deplasman takımı aynı olamaz.");
            RuleFor(x => x.Week)
                .InclusiveBetween(1, 38).WithMessage("Hafta bilgisi Serie A standardı olan 1 ile 38 arasında olmalıdır.");
            RuleFor(x => x.Stadium)
                .NotEmpty().WithMessage("Stadyum bilgisi boş bırakılamaz.")
                .MaximumLength(150).WithMessage("Stadyum adı 150 karakterden uzun olamaz.");
            RuleFor(x => x.MatchDate)
                .NotEmpty().WithMessage("Maç tarihi ve saati belirtilmelidir.");
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Geçersiz maç durumu seçildi.");
            // Scoring Rules
            RuleFor(x => x.HomeScore)
                .GreaterThanOrEqualTo(0).WithMessage("Ev sahibi skoru 0'dan küçük olamaz.")
                .When(x => x.HomeScore.HasValue);
            RuleFor(x => x.AwayScore)
                .GreaterThanOrEqualTo(0).WithMessage("Deplasman skoru 0'dan küçük olamaz.")
                .When(x => x.AwayScore.HasValue);
            RuleFor(x => x.HomeScore)
                .NotNull().WithMessage("Tamamlanan bir maç için ev sahibi skoru girilmelidir.")
                .When(x => x.Status == MatchStatus.Finished);
            RuleFor(x => x.AwayScore)
                .NotNull().WithMessage("Tamamlanan bir maç için deplasman skoru girilmelidir.")
                .When(x => x.Status == MatchStatus.Finished);
            RuleFor(x => x.HomeScore)
                .Null().WithMessage("Henüz başlamamış bir maça skor girilemez.")
                .When(x => x.Status == MatchStatus.NotStarted);
            RuleFor(x => x.AwayScore)
                .Null().WithMessage("Henüz başlamamış bir maça skor girilemez.")
                .When(x => x.Status == MatchStatus.NotStarted);
        }
    }
}
