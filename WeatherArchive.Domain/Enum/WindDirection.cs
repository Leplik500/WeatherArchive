using System.ComponentModel.DataAnnotations;

namespace WeatherArchive.Domain.Enum;

public enum WindDirection
{
    [Display(Name = "Штиль")] Штиль = 0,
    [Display(Name = "С")] Север = 1,
    [Display(Name = "СВ")] СевероВосток = 2,
    [Display(Name = "В")] Восток = 3,
    [Display(Name = "ЮВ")] ЮгоВосток = 4,
    [Display(Name = "Ю")] Юг = 5,
    [Display(Name = "ЮЗ")] ЮгоЗапад = 6,
    [Display(Name = "З")] Запад = 7,
    [Display(Name = "СЗ")] СевероЗапад = 8,

    [Display(Name = "С, СВ")] СеверСевероВосток = 9,
    [Display(Name = "С, СЗ")] СеверСевероЗапад = 10,
    [Display(Name = "СВ, В")] СевероВостокВосток = 11,
    [Display(Name = "В, СВ")] ВостокСевероВосток = 12,
    [Display(Name = "В, ЮВ")] ВостокЮгоВосток = 13,
    [Display(Name = "ЮВ, В")] ЮгоВостокВосток = 14,
    [Display(Name = "ЮВ, Ю")] ЮгоВостокЮг = 15,
    [Display(Name = "Ю, ЮВ")] ЮгЮгоВосток = 16,
    [Display(Name = "Ю, ЮЗ")] ЮгЮгоЗапад = 17,
    [Display(Name = "ЮЗ, Ю")] ЮгоЗападЮг = 18,
    [Display(Name = "ЮЗ, З")] ЮгоЗападЗапад = 19,
    [Display(Name = "З, ЮЗ")] ЗападЮгоЗапад = 20,
    [Display(Name = "З, СЗ")] ЗападСевероЗапад = 21,
    [Display(Name = "СЗ, З")] СевероЗападЗапад = 22,
    [Display(Name = "СЗ, С")] СевероЗападСевер = 23,

    [Display(Name = "СВ, СЗ")] СевероВостокСевероЗапад = 24,
    [Display(Name = "ЮВ, ЮЗ")] ЮгоВостокЮгоЗапад = 25
}