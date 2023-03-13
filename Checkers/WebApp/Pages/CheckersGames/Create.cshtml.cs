using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using Domain;

namespace WebApp.Pages_CheckersGames
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IGameRepository _repo;


        public CreateModel(AppDbContext context, IGameRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        public IActionResult OnGet()
        {
        OptionsSelectList = new SelectList(_context.CheckersOptions, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public CheckersGame CheckersGame { get; set; } = default!;
        
        public SelectList OptionsSelectList { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.CheckersGames == null || CheckersGame == null)
            {
                return Page();
            }

          CheckersGame =_repo.SaveGame(CheckersGame.GameName, CheckersGame);

          return RedirectToPage("./Play", new {id = CheckersGame.Id});
        }
    }
}
