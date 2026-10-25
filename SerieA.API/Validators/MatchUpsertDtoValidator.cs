using FluentValidation;
using SerieA.API.DTOs.Matches;

namespace SerieA.API.Validators
{
    public class MatchUpsertDtoValidator : AbstractValidator<MatchUpsertDto>
    {
        public MatchUpsertDtoValidator()
        {
            // Home Team cannot be empty
            RuleFor(x => x.HomeTeamId)
                .NotEmpty().WithMessage("Ev sahibi takım seçilmelidir.");
            // Away Team cannot be empty
            RuleFor(x => x.AwayTeamId)
                .NotEmpty().WithMessage("Deplasman takımı seçilmelidir.");
            // Home Team and Away Team cannot be the same team
            RuleFor(x => x.AwayTeamId)
                .NotEqual(x => x.HomeTeamId).WithMessage("Ev sahibi takım ile deplasman takımı aynı olamaz.");
            // It must be between weeks 1 and 38 (Serie A league standard)
            RuleFor(x => x.Week)
                .InclusiveBetween(1, 38).WithMessage("Hafta bilgisi 1 ile 38 arasında olmalıdır.");
            // Stadium cannot be empty
            RuleFor(x => x.Stadium)
                .NotEmpty().WithMessage("Stadyum bilgisi boş bırakılamaz.");
            // Match date cannot be empty
            RuleFor(x => x.MatchDate)
                .NotEmpty().WithMessage("Maç tarihi ve saati belirtilmelidir.");
        }
    }
}
