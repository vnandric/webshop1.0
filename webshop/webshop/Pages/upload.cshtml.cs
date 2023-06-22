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

        [BindProperty]
        [Required(ErrorMessage = "Fill in a poduct type!")]
        public string ProductType { get; set; }
        
        public List<TypeModel> Types { get; set; }

        [BindProperty]
        public string Type { get; set; }

        [BindProperty]
        public string TypeHidden { get; set; }

        public uploadModel(IWebHostEnvironment webHostEnvironment)
        {
            _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        }

        public IActionResult OnPostUpdate()
        {

            using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                connection.Open();

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE product_type SET naam = @NaamT WHERE naam = @Naam";
                updateCommand.Parameters.AddWithValue("@NaamT", Type);
                updateCommand.Parameters.AddWithValue("@Naam", TypeHidden);
                updateCommand.ExecuteNonQuery();

            }
            return RedirectToPage("/Upload");
        }

        public IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Redirect to login page if the user is not logged in
                return RedirectToPage("/Index");
            }

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

                var types = new List<TypeModel>();

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var type = new TypeModel
                        {
                            ProductT = reader.GetString(0)
                        };
                        types.Add(type);
                    }
                }

                Types = types;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostForm1()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Redirect to login page if the user is not logged in
                return RedirectToPage("/Index");
            }

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

        public IActionResult OnPostForm2()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Redirect to login page if the user is not logged in
                return RedirectToPage("/Index");
            }

            if (!string.IsNullOrEmpty(ProductType))
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
                    {
                        connection.Open();

                        var insertCommand = connection.CreateCommand();
                        insertCommand.CommandText = "INSERT INTO product_type (naam) VALUES (@Naam)";
                        insertCommand.Parameters.AddWithValue("@Naam", ProductType);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"An error occurred while submitting form: {ex.Message}";
                    return Page();
                }
            }

            return RedirectToPage("/Upload");
        }

        public class Form1Model : PageModel
        {
            [BindProperty]
            public string PT_naam { get; set; }

            [BindProperty]
            public int Prijs { get; set; }

            [BindProperty]
            public IFormFileCollection UploadedFiles { get; set; }

            [BindProperty]
            public string Naam { get; set; }

            public string ErrorMessage { get; set; }

            public string[] PT_name;
        }

        public class Form2Model : PageModel
        {
            [BindProperty]
            public string ProductType { get; set;}
        }

        public class TypeModel
        {
            public string ProductT { get; set;}
        }
    }
}