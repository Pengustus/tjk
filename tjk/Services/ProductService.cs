using tjk.Models;

namespace tjk.Services
{
    public interface IProductService
    {
        List<CategoryItem> GetCategories();
        List<ProductItem> GetProducts(string? categorySlug = null, bool? featuredOnly = null);
        ProductItem? GetProductBySlug(string slug);
    }

    public class ProductService : IProductService
    {
        private readonly List<CategoryItem> _categories;
        private readonly List<ProductItem> _products;

        public ProductService()
        {
            _categories = new List<CategoryItem>
            {
                new CategoryItem
                {
                    Id = "stirrup-benders",
                    Slug = "estribadoras",
                    Name = "Автоматични стремежови машини",
                    Description = "Автоматизирано производство на стремена и огънати елементи от арматура на рулон с висока прецизност.",
                    IconClass = "bi-arrow-repeat",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153719/Estribos-Categoria-600x400.jpg",
                    ProductCount = 2
                },
                new CategoryItem
                {
                    Id = "shear-cutting",
                    Slug = "ryazane-razmer",
                    Name = "Линии за рязане по размер",
                    Description = "Високопроизводителни линии за измерване, подаване и рязане на прътова арматура с максимална прецизност.",
                    IconClass = "bi-scissors",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153833/Corte-a-medida-Categoria-600x400.jpg",
                    ProductCount = 1
                },
                new CategoryItem
                {
                    Id = "corte-doblado",
                    Slug = "ryazane-ogyvane",
                    Name = "Рязане и огъване",
                    Description = "Комбинирани системи за непрекъсната ефективност при взискателни производствени линии.",
                    IconClass = "bi-bezier2",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153834/Corte-y-doblado-Categoria-600x400.jpg",
                    ProductCount = 2
                },
                new CategoryItem
                {
                    Id = "straightening",
                    Slug = "izpravyane",
                    Name = "Изправяне и рязане от рулон",
                    Description = "Автоматично роторно изправяне с непрекъснато подаване на тел за чисти и прави заготовки.",
                    IconClass = "bi-dash-lg",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153833/Enderezadoras-Categoria-600x400.jpg",
                    ProductCount = 2
                },
                new CategoryItem
                {
                    Id = "pile-cages",
                    Slug = "armaturni-kosheve",
                    Name = "Арматурни кошове и пилоти",
                    Description = "Специализирани решения за автоматизирано заваряване на пилоти, колони и префaбрикати.",
                    IconClass = "bi-grid-3x3-gap",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153834/Jaulas-y-armaduras-Categoria-600x400.jpg",
                    ProductCount = 2
                },
                new CategoryItem
                {
                    Id = "doblado",
                    Slug = "roboti-ogyvane",
                    Name = "Огъване и формоване",
                    Description = "Прецизни криви, постоянни радиуси и огъване без деформации на тежки арматурни профили.",
                    IconClass = "bi-cpu",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153832/Doblado-y-conformado-Categoria-600x400.jpg",
                    ProductCount = 1
                }
            };

            _products = new List<ProductItem>
            {
                new ProductItem
                {
                    Id = "wg-12",
                    Slug = "estribadora-wg12",
                    CategoryId = "stirrup-benders",
                    CategoryName = "Автоматични стремежови машини",
                    Name = "TJK WG12 Автоматична стремежова машина",
                    ShortDescription = "CNC огъване на стремена и полигонални фигури от арматура на рулон до Ø12 mm.",
                    FullDescription = "TJK WG12 е автоматизирано решение за производство на стремена и огънати детайли от арматурен тел на рулон. Оборудвана с прецизни сервомотори, WG12 гарантира повторяемост на геометрията, висока скорост на изтегляне и минимален отпадък.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153800/Estribadora-WG12-1A-660x440.jpg",
                    Badge = "Популярен модел",
                    IsFeatured = true,
                    Features = new List<string>
                    {
                        "Обработка на единична нишка (Ø5 - 12 mm) или двойна нишка (Ø5 - 10 mm)",
                        "Скорост на изтегляне до 110 м/мин",
                        "Двупосочно огъване на главата (±180°)",
                        "Интуитивен сензорен CNC дисплей",
                        "Сервоуправление с висока енергийна ефективност"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Единична нишка", "Ø5 - 12 mm" },
                        { "Двойна нишка", "Ø5 - 10 mm" },
                        { "Макс. скорост на изтегляне", "110 м/мин" },
                        { "Макс. ъгъл на огъване", "±180°" },
                        { "Точност на дължина", "±1 mm" },
                        { "Мощност", "26 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "wg-16",
                    Slug = "estribadora-wg16",
                    CategoryId = "stirrup-benders",
                    CategoryName = "Автоматични стремежови машини",
                    Name = "TJK WG16 Автоматична стремежова машина",
                    ShortDescription = "Мощна стремежова машина за тежка арматура от рулон до Ø16 mm единично.",
                    FullDescription = "TJK WG16 съчетава голяма сила на огъване с надеждна конструкция за работа с по-големи диаметри арматурно желязо от рулон.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153759/Estribadora-WG16-1-660x440.jpg",
                    Badge = "Тежка серия",
                    IsFeatured = true,
                    Features = new List<string>
                    {
                        "Обработка на единична нишка (до Ø16 mm) и двойна нишка (до Ø12 mm)",
                        "Подсилени тракционни и изправящи ролки",
                        "Мощна хидравлична режеща система",
                        "Библиотека с предварително зададени фигури"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Единична нишка", "Ø6 - 16 mm" },
                        { "Двойна нишка", "Ø6 - 12 mm" },
                        { "Макс. скорост на изтегляне", "100 м/мин" },
                        { "Точност на ъгъла", "±0.5°" },
                        { "Мощност", "34 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "shear-line-150b",
                    Slug = "shear-line-150b",
                    CategoryId = "shear-cutting",
                    CategoryName = "Линии за рязане по размер",
                    Name = "TJK Shear Line 150B Линия за рязане",
                    ShortDescription = "Високопроизводителна автоматизирана линия за измерване и рязане на прътова арматура.",
                    FullDescription = "TJK Shear Line 150B е индустриален комплекс за автоматизирано подаване, измерване с висока точност и хидравлично срязване на арматурни снопове.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153701/SHARE-LINE-150B-1-660x440.jpg",
                    Badge = "Висока производителност",
                    IsFeatured = true,
                    Features = new List<string>
                    {
                        "Ширина на режещото острие 150 mm",
                        "Автоматизирана измервателна каретка",
                        "Сортировъчни джобове за готови дължини",
                        "Хидравличен притискач за прецизен срез без разкривяване"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Ширина на ножа", "150 mm" },
                        { "Дължина на подаване", "12 m / 14 m" },
                        { "Скорост на каретката", "120 м/мин" },
                        { "Точност на позиция", "±1.5 mm" },
                        { "Мощност", "22 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "wg-12-f1",
                    Slug = "wg-12-f1",
                    CategoryId = "corte-doblado",
                    CategoryName = "Рязане и огъване",
                    Name = "TJK WG 12 F1 Комбинирана линия",
                    ShortDescription = "Компактна автоматична система за измерване, подрязване и двойно огъване.",
                    FullDescription = "TJK WG 12 F1 оптимизира работния процес чрез комбиниране на рязане и огъване в една единствена технологична линия.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153749/WG12-F1-660x440.jpg",
                    Badge = "Комбиниран модел",
                    IsFeatured = false,
                    Features = new List<string>
                    {
                        "Хидравлично рязане с интегрирано двойно огъване",
                        "Компактна конструкция, спестяваща място в цеха",
                        "Бърза пренастройка за различни размери"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Работен обхват", "Ø12 - 32 mm" },
                        { "Дължина на масата", "12 m" },
                        { "Мощност", "18 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "wg-16-f2",
                    Slug = "wg-16-f2",
                    CategoryId = "corte-doblado",
                    CategoryName = "Рязане и огъване",
                    Name = "TJK WG 16 F2 Линия за рязане и огъване",
                    ShortDescription = "Двуглава автоматизирана линия за рязане и двустранно огъване на арматура.",
                    FullDescription = "Високоскоростна линия за паралелна обработка и огъване на двата края на арматурните пръти едновременно.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153751/WG16-F2-1-660x440.jpg",
                    Badge = "Двуглав",
                    IsFeatured = true,
                    Features = new List<string>
                    {
                        "Две независими огъващи глави",
                        "Синхронизирано серво задвижване",
                        "Автоматично изхвърляне на готовите елементи"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Работен диаметър", "до Ø36 mm" },
                        { "Дължина", "до 14 m" },
                        { "Мощност", "25 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "gt-5",
                    Slug = "gt5-izpravyashta-mashina",
                    CategoryId = "straightening",
                    CategoryName = "Изправяне и рязане от рулон",
                    Name = "TJK GT5 Изправяща и режеща машина",
                    ShortDescription = "Компактна роторна машина за високоскоростно изправяне на тел от рулон.",
                    FullDescription = "TJK GT5 предлага бързо и прецизно изправяне на гладък и оребрен тел с минимално увреждане на повърхността.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153731/GT5-1-660x440.jpg",
                    Badge = "Компактна",
                    IsFeatured = false,
                    Features = new List<string>
                    {
                        "Роторно изправяне с висока гладкост",
                        "Хидравличен нож за чисто отрязване",
                        "Лесно управление и бърза подмяна на ролките"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Диаметър", "Ø3 - 8 mm" },
                        { "Скорост", "до 120 м/мин" },
                        { "Точност", "±1 mm" },
                        { "Мощност", "15 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "gt-8",
                    Slug = "gt8-izpravyashta-mashina",
                    CategoryId = "straightening",
                    CategoryName = "Изправяне и рязане от рулон",
                    Name = "TJK GT8 Индустриална изправяща машина",
                    ShortDescription = "Високоскоростна роторна машина за изправяне и рязане на арматура до Ø14 mm.",
                    FullDescription = "TJK GT8 използва здрава роторна система за постигане на прецизна праволинейност на арматурната стомана с висока производителност.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153733/GT8-1-660x440.jpg",
                    Badge = "Висока скорост",
                    IsFeatured = false,
                    Features = new List<string>
                    {
                        "Роторна система с висока износоустойчивост",
                        "Летящ хидравличен нож за рязане в движение",
                        "Шумоизолиран защитен корпус"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Диаметър", "Ø5 - 14 mm" },
                        { "Скорост на изправяне", "до 140 м/мин" },
                        { "Точност на рязане", "±1.5 mm" },
                        { "Мощност", "30 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "hlz-2000",
                    Slug = "hlz-2000-mashina-za-kosheve",
                    CategoryId = "pile-cages",
                    CategoryName = "Арматурни кошове и пилоти",
                    Name = "TJK HLZ 2000 Машина за арматурни кошове",
                    ShortDescription = "Автоматизирана заваръчна машина за производство на пилотни кошове до Ø2000 mm.",
                    FullDescription = "TJK HLZ 2000 е проектирана за изработване на цилиндрични арматурни кошове за сондажни пилоти, колони и фундаменти с автоматично навиване и заваряване на спиралата.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153708/HLZ2000-1-660x440.jpg",
                    Badge = "За големи диаметри",
                    IsFeatured = true,
                    Features = new List<string>
                    {
                        "Диаметър на коша до 2000 mm",
                        "Дължина на коша до 12 m (опция 18 m)",
                        "Дигитално регулиране на стъпката на спиралата",
                        "Автоматична система за заваряване"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Диаметър на коша", "300 - 2000 mm" },
                        { "Макс. дължина", "12,000 mm" },
                        { "Макс. тегло на кош", "5,000 kg" },
                        { "Мощност", "22 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "hl-1500",
                    Slug = "hl-1500-mashina-za-kosheve",
                    CategoryId = "pile-cages",
                    CategoryName = "Арматурни кошове и пилоти",
                    Name = "TJK HL 1500 Машина за арматурни кошове",
                    ShortDescription = "Надеждна машина за навиване и заваряване на арматурни кошове до Ø1500 mm.",
                    FullDescription = "TJK HL 1500 позволява бързо и стабилно сглобяване на арматурни кошове за основи и конструкции.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153712/HLZ1500-1-660x440.jpg",
                    Badge = "Стандартен модел",
                    IsFeatured = false,
                    Features = new List<string>
                    {
                        "Диаметър на коша от 300 до 1500 mm",
                        "Хидравлични опори против провисване",
                        "Плавно регулиране на скоростта на въртене"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Диаметър", "300 - 1500 mm" },
                        { "Дължина", "до 12 m" },
                        { "Мощност", "18 kW" }
                    }
                },
                new ProductItem
                {
                    Id = "doblado-robot",
                    Slug = "doblado-robot",
                    CategoryId = "doblado",
                    CategoryName = "Огъване и формоване",
                    Name = "TJK Автоматичен център за огъване",
                    ShortDescription = "Прецизно огъване на арматурни профили с постоянни радиуси.",
                    FullDescription = "Автоматизиран комплекс за формоване и огъване на големи обеми арматурни заготовки с висока скорост и повторяемост.",
                    ImageUrl = "https://tjkserecon.s3.eu-central-1.amazonaws.com/wp-content/uploads/2025/07/26153832/Doblado-y-conformado-Categoria-600x400.jpg",
                    Badge = "Формоване",
                    IsFeatured = false,
                    Features = new List<string>
                    {
                        "Прецизен контрол на радиуса",
                        "Без деформации по оребрението",
                        "Бърза смяна на огъващите дорници"
                    },
                    Specifications = new Dictionary<string, string>
                    {
                        { "Диаметър", "до Ø40 mm" },
                        { "Ъгъл на огъване", "0° - 180°" },
                        { "Мощност", "20 kW" }
                    }
                }
            };
        }

        public List<CategoryItem> GetCategories() => _categories;

        public List<ProductItem> GetProducts(string? categorySlug = null, bool? featuredOnly = null)
        {
            var query = _products.AsQueryable();

            if (!string.IsNullOrEmpty(categorySlug))
            {
                var category = _categories.FirstOrDefault(c => c.Slug.Equals(categorySlug, StringComparison.OrdinalIgnoreCase));
                if (category != null)
                {
                    query = query.Where(p => p.CategoryId == category.Id);
                }
            }

            if (featuredOnly.HasValue && featuredOnly.Value)
            {
                query = query.Where(p => p.IsFeatured);
            }

            return query.ToList();
        }

        public ProductItem? GetProductBySlug(string slug)
        {
            return _products.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        }
    }
}
