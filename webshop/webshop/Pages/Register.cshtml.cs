using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace webshop.Pages.Shared
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Customer Customer { get; set; }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                string connectionString = "Data Source=webshop.db";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO customers (customerName, password) VALUES (@CustomerName, @Password)";

                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerName", Customer.CustomerName);
                        command.Parameters.AddWithValue("@Password", Customer.Password);

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                return RedirectToPage("Inlog");
            }

            return Page();
        }
    }

    public class Customer
    {
        public string CustomerName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
