using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain;

namespace WebApp.Pages.CheckersGames
{
    public class IndexModel : PageModel
    {
        private readonly IGameRepository _repo;
        
        public IndexModel(IGameRepository repo)
        {
            _repo = repo;
        }

        public IList<CheckersGame> CheckersGame { get;set; } = default!;

        public async Task OnGetAsync()
        {
            CheckersGame = _repo.GetGamesList();
        }
    }
}
