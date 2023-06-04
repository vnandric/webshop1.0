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
        public IFormFileCollection Img { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The Evenement field is required.")]

        public string PT_naam { get; set; }

        public string ErrorMessage { get; set; }

    }
}
