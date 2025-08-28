using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public HeaderViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            return View();
        }

    }
}
