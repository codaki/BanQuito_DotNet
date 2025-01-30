using CLIWEB_BANQUITO_SOAP_DOTNET.WSBANQUITO;
using System.Web.Mvc;

namespace CLIWEB_BANQUITO_SOAP_DOTNET.Controllers
{
    public class LoginController : Controller
    {
        private readonly WSBanquitoSoapClient _srvLogin = new WSBanquitoSoapClient();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(string username, string password)
        {

            var loginResult = _srvLogin.ValidarCredenciales(username, password);

            if (loginResult) // Adjust based on actual response
            {
                string redirectUrl = Url.Action("Menu", "Home"); // Define your desired URL here
                return Redirect(redirectUrl);
            }
            else
            {
                ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
                return View("Index");
            }
        }



    }
}
