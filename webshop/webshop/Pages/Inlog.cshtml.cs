using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;

namespace webshop.Pages
{
    public class InlogModel : PageModel
    {
        private readonly string connectionString = "Data Source=webshop.db";

        [BindProperty]
        public string CustomerName { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Gebruikersnaam en wachtwoord zijn verplicht!");
                return Page();
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM customers WHERE customerName = @customerName AND password = @password";
                selectCommand.Parameters.AddWithValue("@customerName", CustomerName);
                selectCommand.Parameters.AddWithValue("@password", Password);

                var reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    HttpContext.Session.SetString("CustomerName", CustomerName);
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Gebruikersnaam of wachtwoord is onjuist.");
                    return Page();
                }
            }
        }
    }
}
