﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace webshop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";
        private readonly string _uploadFolderPath;

        public List<ProductModel> Producten { get; set; }

        public IndexModel(IWebHostEnvironment webHostEnvironment)
        {
            _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        public IActionResult OnGet()
        {
            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT ID, PT_naam, prijs, img, naam FROM product";

                var producten = new List<ProductModel>();

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            ID = reader.GetString(0),
                            PT_naam = reader.GetString(1),
                            prijs = reader.GetString(2),
                            img = reader.GetString(3),
                            naam = reader.GetString(4)
                        };

                        producten.Add(product);
                    }
                }

                Producten = producten;
            }
            return Page();
        }
    }

    public class ProductModel 
    {
        public string ID { get; set; }
        public string PT_naam { get; set; }
        public string prijs { get; set; }
        public string img { get; set; }
        public string naam { get; set; }

    }
}