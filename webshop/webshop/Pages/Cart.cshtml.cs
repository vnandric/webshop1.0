using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http;

namespace webshop.Pages
{
    public class CartModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";

        public List<CartItem> CartItems { get; set; }

        public class CartItem
        {
            public string ID { get; set; }
            
            public int Prijs { get; set; }

            public string Naam { get; set; }

            public int Quantity { get; set; }
        }

        public void OnGet()
        {
            //haalt data uit de shoppingcart session
            Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart") ?? new Dictionary<string, int>();

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
                    
                    var reader = insertCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        CartItems.Add(new CartItem
                        {
                            ID = reader.GetInt32(0).ToString(),
                            Naam = reader.GetString(1),
                            Prijs = reader.GetInt32(2),
                            Quantity = quantity
                        });

                    }

                    connection.Close();

                }
            }            
            
        }
        public IActionResult OnPostEmptyCart()
        {
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToPage("/Cart");
        }
    }
}
