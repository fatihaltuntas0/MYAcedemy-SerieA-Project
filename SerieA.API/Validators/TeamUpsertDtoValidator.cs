using FluentValidation;
using SerieA.API.DTOs.Teams;

namespace SerieA.API.Validators
{
    public class TeamUpsertDtoValidator : AbstractValidator<TeamUpsertDto>
    {
        public TeamUpsertDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Takım adı boş bırakılamaz.")
                .Length(2, 100).WithMessage("Takım adı 2 ile 100 karakter arasında olmalıdır.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Takım kodu boş bırakılamaz.")
                .Length(3).WithMessage("Takım kodu tam olarak 3 karakter olmalıdır (Örn: JUV, MIL).");
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir alanı boş bırakılamaz.")
                .MaximumLength(60).WithMessage("Şehir adı 60 karakterden uzun olamaz.");
            RuleFor(x => x.Region)
                .NotEmpty().WithMessage("Bölge alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Bölge adı 50 karakterden uzun olamaz.");
            RuleFor(x => x.Stadium)
                .NotEmpty().WithMessage("Stadyum adı boş bırakılamaz.")
                .MaximumLength(150).WithMessage("Stadyum adı 150 karakterden uzun olamaz.");
            RuleFor(x => x.StadiumCapacity)
                .GreaterThan(0).WithMessage("Stadyum kapasitesi 0'dan büyük olmalıdır.")
                .LessThan(150000).WithMessage("Lütfen gerçekçi bir stadyum kapasitesi giriniz.");
            RuleFor(x => x.FoundedYear)
                .InclusiveBetween(new DateTime(1850, 1, 1), DateTime.Now)
                .WithMessage($"Kuruluş tarihi 1850 yılından eski olamaz.");
            RuleFor(x => x.ColorOne)
                .NotEmpty().WithMessage("Birinci kulüp rengi seçilmelidir.")
                .Matches(@"^#([A-Fa-f0-9]{6})$").WithMessage("Birinci renk geçerli bir Hex formatında olmalıdır (Örn: #000000).");
            RuleFor(x => x.ColorTwo)
                .NotEmpty().WithMessage("İkinci kulüp rengi seçilmelidir.")
                .Matches(@"^#([A-Fa-f0-9]{6})$").WithMessage("İkinci renk geçerli bir Hex formatında olmalıdır (Örn: #FFFFFF).");
            RuleFor(x => x.LogoUrl)
                .NotEmpty().WithMessage("Logo bağlantısı boş bırakılamaz.");
        }
    }
}
