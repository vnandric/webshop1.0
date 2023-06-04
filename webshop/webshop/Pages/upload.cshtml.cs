using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;

namespace webshop.Pages
{
    public class uploadModel : PageModel
    {
        private readonly string _dbPath = "webshop.db";
        private readonly string _uploadFolderPath;

        [BindProperty]
        public IFormFileCollection UploadedFiles { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Fill in the fields!")]
        public string PT_naam { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Fill in the fields!")]
        public string Prijs { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Fill in the fields!")]
        public string Naam { get; set; }

        public string ErrorMessage { get; set; }

        public uploadModel(IWebHostEnvironment webHostEnvironment)
        {
            _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
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
                            string image = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                            string filePath = Path.Combine("uploads", image);
                            string physicalPath = Path.Combine(_uploadFolderPath, image);

                            using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            var insertCommand = connection.CreateCommand();
                            insertCommand.CommandText = "INSERT INTO product (ID, PT_naam, prijs, img, naam, FilePath) VALUES (@ID, @PT_naam, @prijs, @img, @naam, @FilePath)";
                            insertCommand.Parameters.AddWithValue("@ID", null);
                            insertCommand.Parameters.AddWithValue("@PT_naam", PT_naam);
                            insertCommand.Parameters.AddWithValue("@prijs", Prijs);
                            insertCommand.Parameters.AddWithValue("@img", file.FileName);
                            insertCommand.Parameters.AddWithValue("@naam", Naam);
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