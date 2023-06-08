using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.Sqlite;

namespace webshop.Pages
{
    public class CartModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";

        public List<CartItem> CartItems { get; set; }

        public class CartItem
        {
            public int ID { get; set; }
            
            public int Prijs { get; set; }

            public string Naam { get; set; }

            public int Quantity { get; set; }
        }

        public void OnGet()
        {
            Dictionary<string, int> shoppingCart = HttpContext.Session.Get <Dictionary<string, int>>("ShoppingCart") ?? new Dictionary<string, int>();

            CartItems = new List<CartItem>();

            foreach (var item in shoppingCart)
            {
                string productId = item.Key;
                int quantity = item.Value;

                using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                {
                    connection.Open();

                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = "SELECT * FROM product WHERE id = @productId";
                    insertCommand.Parameters.AddWithValue("@productId", productId);

                }
            }
        }
    }
}
