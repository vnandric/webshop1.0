using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace webshop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";
        private readonly string _uploadFolderPath;

        public List<ProductModel> Producten { get; set; }

        public List<TypeModel> Type { get; set; }

        [BindProperty]
        public string Filter { get; set; }

        public string[] PT_naam;

        public IndexModel(IWebHostEnvironment webHostEnvironment)
        {
            _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        [Authorize]
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        public IActionResult OnGet( string filterOption)
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT * FROM product_type";

                var kaas = new List<string>();
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kaas.Add(reader.GetString(0));
                    }
                }
                this.PT_naam = kaas.ToArray();

                var types = new List<TypeModel>();

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product_type = new TypeModel()
                        {
                            naam = reader.GetString(0)
                        };

                        types.Add(product_type);
                    }

                }

                Type = types;
            }

            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT * FROM product";

                var producten = new List<ProductModel>();

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel()
                        {
                            ID = reader.GetString(0),
                            PT_naam = reader.GetString(1),
                            prijs = reader.GetString(2),
                            img = reader.GetString(3),                            
                            naam = reader.GetString(4),
                            FilePath = reader.GetString(5)
                        };

                        producten.Add(product);
                    }
                }

                Producten = producten;
            }

            

            return Page();
        }

        public RedirectToPageResult OnPostAddToCart(string id)
        {
            //Haalt de data uit de shopping cart session
            Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart") ?? new Dictionary<string, int>();
            if (shoppingCart.ContainsKey(id))
            {
                shoppingCart[id] += 1;
            }
            else
            {
                shoppingCart[id] = 1;
            }

            // Slaat de dingetje op :)
            HttpContext.Session.Set("ShoppingCart", shoppingCart);

            return RedirectToPage("Index");
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }


    public class ProductModel 
    {
        public string ID { get; set; }
        public string PT_naam { get; set; }
        public string prijs { get; set; }
        public string img { get; set; }        
        public string naam { get; set; }
        public string FilePath { get; set; }
    }

    public class TypeModel
    {
        public string naam { get; set; }
    }
}