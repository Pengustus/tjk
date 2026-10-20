using System.ComponentModel.DataAnnotations;

namespace tjk.Models
{
    public class CategoryItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    public class ProductItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string FullDescription { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Badge { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> Features { get; set; } = new();
        public Dictionary<string, string> Specifications { get; set; } = new();
    }

    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Моля, въведете вашето име.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, въведете имейл адрес.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, въведете телефон за връзка.")]
        public string Phone { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;
        public string SelectedProduct { get; set; } = string.Empty;

        [Required(ErrorMessage = "Моля, напишете вашето съобщение.")]
        public string Message { get; set; } = string.Empty;

        public bool SubmittedSuccessfully { get; set; }
    }

    public class MachineFormViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Името на машината е задължително.")]
        [Display(Name = "Име на модела")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "URL идентификатор (Slug)")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Моля, изберете категория.")]
        [Display(Name = "Категория")]
        public string CategoryId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Краткото описание е задължително.")]
        [Display(Name = "Кратко описание")]
        public string ShortDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пълното описание е задължително.")]
        [Display(Name = "Пълно описание")]
        public string FullDescription { get; set; } = string.Empty;

        [Display(Name = "URL адрес на изображение")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Или качете снимка от компютъра")]
        public IFormFile? ImageUpload { get; set; }

        [Display(Name = "Етикет / Бадж (напр. 'ОКАЗИОН', 'НА СКЛАД', 'ТОП ОФЕРТА')")]
        public string? Badge { get; set; }

        [Display(Name = "Показвай като избран модел на началната страница")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Ключови характеристики (по една на ред)")]
        public string FeaturesText { get; set; } = string.Empty;

        [Display(Name = "Технически спецификации (формат 'Параметър: Стойност', по един на ред)")]
        public string SpecificationsText { get; set; } = string.Empty;
    }

    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Въведете потребителско име.")]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Въведете парола.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
