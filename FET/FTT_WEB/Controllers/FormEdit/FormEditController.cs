using FTT_WEB.Models.Partial;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.FormEdit
{
    public class FormEditController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadFormMaintain(string formNo)
        {
            var vm = new FormEditVM
            {
                FormNo = formNo,
            };
            return PartialView("Form/_FormMaintain", vm);
        }
    }
}
