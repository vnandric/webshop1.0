using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace webshop.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";

        public string Naam { get; set; }

        public async Task<IActionResult> OnGetAsync(string naam)
        {
            Naam = naam;

            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "DELETE FROM product_type WHERE naam = @Naam";
                deleteCommand.Parameters.AddWithValue("@Naam", Naam);
                deleteCommand.ExecuteNonQuery();
            }

            return RedirectToPage("/Upload");
        }
    }
}
