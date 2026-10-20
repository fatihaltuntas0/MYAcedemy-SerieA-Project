using FluentValidation;
using SerieA.API.DTOs;

namespace SerieA.API.Validators
{
    public class TeamUpsertDtoValidator : AbstractValidator<TeamUpsertDto>
    {
        public TeamUpsertDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Takım adı boş bırakılamaz.")
                .MinimumLength(2).WithMessage("Takım adı en az 2 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Takım adı 100 karakterden uzun olamaz.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Takım kodu boş bırakılamaz.")
                .Length(3).WithMessage("Takım kodu tam olarak 3 karakter olmalıdır (Örn: JUV, MIL).");
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir alanı boş bırakılamaz.");
            RuleFor(x => x.Stadium)
                .NotEmpty().WithMessage("Stadyum adı boş bırakılamaz.");
            RuleFor(x => x.StadiumCapacity)
                .GreaterThan(0).WithMessage("Stadyum kapasitesi 0'dan büyük olmalıdır.");
            RuleFor(x => x.LogoUrl)
                .NotEmpty().WithMessage("Logo linki boş bırakılamaz.");
        }
    }
}
