using System.Web.Http;
using System.Web.Http.Cors;

namespace WS_BANQUITO_RESTFULL_DOTNET
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de Web API
            // Habilitar CORS para cualquier origen, encabezado y método
            var cors = new EnableCorsAttribute("*", "*", "*"); // Permitir todo (modifica esto según tus necesidades)
            config.EnableCors(cors);

            // Rutas de Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
