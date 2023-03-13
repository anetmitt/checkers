using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using Domain;

namespace WebApp.Pages_CheckersOptions
{
    public class CreateModel : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public CreateModel(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CheckersOptions CheckersOptions { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.CheckersOptions == null || CheckersOptions == null
              || CheckersOptions.Width < 4 || CheckersOptions.Height < 4
              || CheckersOptions.Height % 2 != 0 || CheckersOptions.Width % 2 != 0)
            {
                return Page();
            }
          
          _context.CheckersOptions.Add(CheckersOptions);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
