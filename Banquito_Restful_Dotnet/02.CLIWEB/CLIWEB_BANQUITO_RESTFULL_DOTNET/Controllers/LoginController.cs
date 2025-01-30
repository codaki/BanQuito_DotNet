using CLIWEB_BANQUITO_RESTFULL_DOTNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CLIWEB_BANQUITO_RESTFULL_DOTNET.Controllers
{
    public class LoginController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BASE_URL = "http://localhost:888/api/";

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> IniciarSesion(string username, string password)
        {
            
            string apiUrl = $"{BASE_URL}telefono/ValidarCredenciales?usuario={username}&contrasena={password}";

            // Hacer la solicitud HTTP GET a la API
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

            if (response.IsSuccessStatusCode)
            {
                // Si la respuesta es exitosa, asumir que la validación fue correcta
                // Aquí puedes modificar la lógica según lo que devuelva la API
                string responseContent = await response.Content.ReadAsStringAsync();

                if (responseContent.Equals("true")) // Ajusta esto dependiendo de la respuesta real de la API
                {
                    string redirectUrl = Url.Action("Menu", "Home"); // Definir la URL de redirección
                    return Redirect(redirectUrl);
                }
            }

            // Si la validación falla o la respuesta no es exitosa
            ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
            return View("Index");
        }
    }
}
