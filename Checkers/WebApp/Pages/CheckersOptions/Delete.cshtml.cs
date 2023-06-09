using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Db;
using Domain;

namespace WebApp.Pages_CheckersOptions
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public DeleteModel(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public CheckersOptions CheckersOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.CheckersOptions == null)
            {
                return NotFound();
            }

            var checkersoptions = await _context.CheckersOptions.FirstOrDefaultAsync(m => m.Id == id);

            if (checkersoptions == null)
            {
                return NotFound();
            }
            else 
            {
                CheckersOptions = checkersoptions;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.CheckersOptions == null)
            {
                return NotFound();
            }
            var checkersoptions = await _context.CheckersOptions.FindAsync(id);

            if (checkersoptions != null)
            {
                CheckersOptions = checkersoptions;
                _context.CheckersOptions.Remove(CheckersOptions);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
