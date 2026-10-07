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
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string SelectedProduct { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool SubmittedSuccessfully { get; set; }
    }
}
