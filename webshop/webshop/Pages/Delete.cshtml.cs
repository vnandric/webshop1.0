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
        private readonly string _uploadFolderPath;

        public DeleteModel(IWebHostEnvironment webHostEnvironment)
        {
            _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        public string Product { get; set; }

        public string ID { get; set; }

        public async Task<IActionResult> OnGetAsync(string product, string id)
        {
            Product = product;
            ID = id;

            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Redirect to login page if the user is not logged in
                return RedirectToPage("/Index");
            }

            // Delete the file from the uploads folder
            var fullPath = Path.Combine(_uploadFolderPath, Product);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "DELETE FROM product WHERE ID = @ID";
                deleteCommand.Parameters.AddWithValue("@ID", ID);
                deleteCommand.ExecuteNonQuery();
            }

            return RedirectToPage("Index");
        }
    }
}
