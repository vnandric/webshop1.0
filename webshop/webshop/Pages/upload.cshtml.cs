using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace webshop.Pages
{
    public class uploadModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";
        private readonly string _uploadFolderPath;

        [BindProperty]
        [Required(ErrorMessage = "Fill in the fields!")]
        public string PT_naam { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Fill in the fields!")]
        public int Prijs { get; set; }

        [BindProperty]
        public IFormFileCollection UploadedFiles { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Fill in the fields!")]
        public string Naam { get; set; }

        public string ErrorMessage { get; set; }

        public string[] PT_name;

        public uploadModel(IWebHostEnvironment webHostEnvironment)
        {
            _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        public IActionResult OnGet()
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
                this.PT_name = kaas.ToArray();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadedFiles != null && UploadedFiles.Count > 0)
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();

                        foreach (var file in UploadedFiles)
                        {
                            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                            string filePath = Path.Combine("uploads", fileName);
                            string physicalPath = Path.Combine(_uploadFolderPath, fileName);

                            using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            var insertCommand = connection.CreateCommand();
                            insertCommand.CommandText = "INSERT INTO product (PT_naam, prijs, img, naam, FilePath) VALUES ( @PT_naam, @Prijs, @Img, @Naam, @FilePath)";
                            insertCommand.Parameters.AddWithValue("@PT_naam", PT_naam);
                            insertCommand.Parameters.AddWithValue("@Prijs", Prijs);
                            insertCommand.Parameters.AddWithValue("@Img", file.FileName);
                            insertCommand.Parameters.AddWithValue("@Naam", Naam);
                            insertCommand.Parameters.AddWithValue("@FilePath", filePath);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"An error occurred while uploading files: {ex.Message}";
                    return Page();
                }
          }

            return RedirectToPage("Index");
        }
    }
}