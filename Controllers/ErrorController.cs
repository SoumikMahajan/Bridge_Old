using System.Web.Mvc;

namespace POMS.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

    }
}
