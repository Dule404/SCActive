using backend.Models;
using backend.Models.SearchModel;
using Microsoft.AspNetCore.Mvc;
namespace backend.ViewComponents
{
    public class SearchViewComponent :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new SearchModel();
            return View(model);
        }
    }
}