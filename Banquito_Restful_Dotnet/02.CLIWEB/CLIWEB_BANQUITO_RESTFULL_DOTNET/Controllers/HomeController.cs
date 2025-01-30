using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLIWEB_BANQUITO_RESTFULL_DOTNET.Controllers
{
    public class HomeController : Controller
    {
        private static List<Telefono> CarritoItems = new List<Telefono>();
        private readonly HttpClient _httpClient;
        private const decimal DESCUENTO_PORCENTAJE = 42m;
        private const string BASE_URL = "http://localhost:888/api/";

        public HomeController()
        {
            _httpClient = new HttpClient();
        }

        // Acción para la página principal
        public System.Web.Mvc.ActionResult Index()
        {
            return View();
        }

        // Acción para el menú
        public System.Web.Mvc.ActionResult Menu()
        {
            return View();
        }

        // Acción para la vista de Catálogos de Teléfonos
        public System.Web.Mvc.ActionResult CatalogosTelefonos()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> Agregar(string nombre, string precio, string fotoBase64)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(precio) || string.IsNullOrWhiteSpace(fotoBase64))
            {
                return Json(new { success = false, message = "Los datos no son válidos" });
            }

            // Create a new telefono object with the photo base64 data
            var telefono = new
            {
                Nombre = nombre,
                Precio = decimal.Parse(precio),
                Foto = fotoBase64
            };

            // Serialize the object to JSON
            var jsonContent = new StringContent(JsonConvert.SerializeObject(telefono), Encoding.UTF8, "application/json");

            // API URL for adding a phone
            string url = $"{BASE_URL}telefono/insertar";

            var response = await _httpClient.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Teléfono agregado exitosamente" });
            }
            else
            {
                return Json(new { success = false, message = "Error al agregar el teléfono" });
            }
        }

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> Eliminar(string nombre, string COD_TEL)
        {
            string url = $"{BASE_URL}telefono/eliminar/{COD_TEL}";

            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Error al eliminar el teléfono" });
            }
        }

        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> Actualizar(int codTel, string nombre, string precio, string fotoBase64)
        {
            // Create an object with updated phone data
            var telefonoUpdateRequest = new
            {
                CodTel = codTel,
                Nombre = nombre,
                Precio = decimal.Parse(precio),
                Foto = fotoBase64  // Include base64 image if present
            };

            var json = JsonConvert.SerializeObject(telefonoUpdateRequest);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string url = $"{BASE_URL}telefono/actualizar";

            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Teléfono actualizado exitosamente." });
            }
            else
            {
                return Json(new { success = false, message = "Error al actualizar el teléfono" });
            }
        }

        [System.Web.Mvc.HttpGet]
        public async Task<JsonResult> Listar()
        {
            string url = $"{BASE_URL}telefono/obtener";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var telefonos = JsonConvert.DeserializeObject<List<Telefono>>(content);
                return Json(telefonos, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Error al listar los teléfonos" }, JsonRequestBehavior.AllowGet);
            }
        }


        // Acción para la vista de Facturación
        public async Task<System.Web.Mvc.ActionResult> Facturacion()
        {
            try
            {
                string url = $"{BASE_URL}telefono/obtener";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var telefonos = JsonConvert.DeserializeObject<List<Telefono>>(content);
                    return View(telefonos);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<Telefono>());
            }
        }

        // Método para agregar al carrito
        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> AgregarAlCarrito(int id)
        {
            try
            {
                // Obtener el teléfono de la API
                string url = $"{BASE_URL}telefono/obtener";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var telefonos = JsonConvert.DeserializeObject<List<Telefono>>(content);
                    var telefono = telefonos.FirstOrDefault(t => t.Id == id);

                    if (telefono != null)
                    {
                        CarritoItems.Add(telefono);
                        return Json(new { success = true });
                    }
                }
                return Json(new { success = false, message = "No se pudo agregar el teléfono" });
            }
            catch
            {
                return Json(new { success = false, message = "Error al agregar al carrito" });
            }
        }

        // Método para eliminar del carrito
        [System.Web.Mvc.HttpPost]
        public JsonResult EliminarDelCarrito(int id)
        {
            try
            {
                //var carrito = (List<Telefono>)Session["CarritoItems"];
                var item = CarritoItems?.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    CarritoItems.Remove(item);
                    Session["CarritoItems"] = CarritoItems; // Actualiza la sesión
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Producto no encontrado" });
            }
            catch
            {
                return Json(new { success = false, message = "Error al eliminar del carrito" });
            }
        }


        // Acción para ver el carrito
        public System.Web.Mvc.ActionResult Carrito()
        {
            return View(CarritoItems);
        }



        // Acción para mostrar las facturas
        public async Task<System.Web.Mvc.ActionResult> VerFacturas()
        {
            string url = $"{BASE_URL}factura/obtener-facturas"; // URL de la API
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var facturas = JsonConvert.DeserializeObject<List<Factura>>(content);

                // Pasamos las facturas a la vista
                return View(facturas);
            }
            else
            {
                return View("Error"); // En caso de error, mostramos una vista de error
            }
        }


        public System.Web.Mvc.ActionResult MontoMaximo()
        {
            return View();
        }
        [System.Web.Mvc.HttpGet]
        public async Task<JsonResult> ObtenerCodigoCliente(string cedula)
        {
            string url = $"{BASE_URL}cliente/codigo/{cedula}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ApiResponse>(content);

                return Json(new { success = true, codigoCliente = data?.CodigoCliente }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No se pudo obtener el código del cliente" }, JsonRequestBehavior.AllowGet);
            }
        }

        [System.Web.Mvc.HttpGet]
        public async Task<JsonResult> ObtenerMontoMaximoCredito(int codigoCliente)
        {
            string url = $"{BASE_URL}credito/monto-maximo/{codigoCliente}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ApiMontoMaximoResponse>(content);

                return Json(new { success = true, montoMaximo = data?.MontoMaximoCredito, mensaje = data?.Mensaje }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No se pudo obtener el monto máximo de crédito" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Clases para deserializar la respuesta de la API
        public class ApiResponse
        {
            public int? CodigoCliente { get; set; }
        }

        public class ApiMontoMaximoResponse
        {
            public double? MontoMaximoCredito { get; set; }
            public string Mensaje { get; set; }
        }


        public System.Web.Mvc.ActionResult TablaAmortizacion()
        {
            return View();
        }
        // Acción para obtener la tabla de amortización
        [System.Web.Mvc.HttpPost]
        public async Task<JsonResult> ObtenerTablaAmortizacion(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                return Json(new { success = false, message = "Por favor ingrese un número de cédula" });
            }

            try
            {
                string clienteUrl = $"{BASE_URL}cliente/codigo/{cedula}";
                var clienteResponse = await _httpClient.GetAsync(clienteUrl);

                if (clienteResponse.IsSuccessStatusCode)
                {
                    var clienteJson = await clienteResponse.Content.ReadAsStringAsync();
                    var clienteData = JsonConvert.DeserializeObject<ApiResponse>(clienteJson);

                    if (clienteData?.CodigoCliente != null)
                    {
                        string tablaUrl = $"{BASE_URL}credito/obtener-tabla-amortizacion/{clienteData.CodigoCliente}";
                        var tablaResponse = await _httpClient.GetAsync(tablaUrl);

                        if (tablaResponse.IsSuccessStatusCode)
                        {
                            var tablaJson = await tablaResponse.Content.ReadAsStringAsync();
                            var tablaAmortizacion = JsonConvert.DeserializeObject<List<AmortizacionDetalle>>(tablaJson);

                            return Json(new { success = true, data = tablaAmortizacion });
                        }
                        else
                        {
                            return Json(new { success = false, message = "No se encontró tabla de amortización para esta cédula" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "No se encontró el código de cliente para esta cédula" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo obtener el código de cliente" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        public System.Web.Mvc.ActionResult PagarCredito()
        {
            // Puedes pasar un modelo vacío o con datos iniciales
            return View(new PagarCreditoViewModel { Total = 0 });
        }
        /// <summary>
                // Acción para manejar la verificación de crédito
        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> VerificarCredito(string cedula, string total)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                TempData["Error"] = "Por favor, ingrese una cédula.";
                return RedirectToAction("PagarCredito");
            }

            try
            {
                var client = new HttpClient();

                // Verificar si es sujeto a crédito
                string urlVerificarCredito = $"{BASE_URL}credito/verificar/{cedula}";
                var responseVerificar = await client.GetAsync(urlVerificarCredito);

                if (!responseVerificar.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo verificar el crédito.";
                    return RedirectToAction("PagarCredito");
                }

                var verificarJson = await responseVerificar.Content.ReadAsStringAsync();
                var verificarData = JsonConvert.DeserializeObject<CreditoResponse>(verificarJson);

                if (!verificarData.EsSujetoDCredito)
                {
                    TempData["Error"] = verificarData.Mensaje;
                    return RedirectToAction("PagarCredito");
                }

                // Obtener código del cliente
                string urlCliente = $"{BASE_URL}cliente/codigo/{cedula}";
                var responseCliente = await client.GetAsync(urlCliente);

                if (!responseCliente.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo obtener el código del cliente.";
                    return RedirectToAction("PagarCredito");
                }

                var clienteJson = await responseCliente.Content.ReadAsStringAsync();
                var clienteData = JsonConvert.DeserializeObject<ClienteResponse>(clienteJson);

                // Obtener monto máximo de crédito
                string urlMontoMaximo = $"{BASE_URL}credito/monto-maximo/{clienteData.CodigoCliente}";
                var responseMonto = await client.GetAsync(urlMontoMaximo);

                if (!responseMonto.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo obtener el monto máximo de crédito.";
                    return RedirectToAction("PagarCredito");
                }

                var montoJson = await responseMonto.Content.ReadAsStringAsync();
                var montoData = JsonConvert.DeserializeObject<MontoMaximoResponse>(montoJson);

                if (double.TryParse(total, out double totalDouble) && totalDouble <= (double)montoData.MontoMaximoCredito)
                {
                    TempData["Success"] = "El cliente puede realizar el crédito.";
                    // Redirigir a la página GenerarTabla
                    return RedirectToAction("GenerarTabla", new { codigoCliente = clienteData.CodigoCliente, total });
                }
                else if (montoData.Mensaje == "Monto máximo de crédito calculado exitosamente.")
                {
                    TempData["Error"] = "El monto de la factura supera el monto máximo aprobado.";
                }
                else
                {
                    TempData["Error"] = montoData.Mensaje;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error: {ex.Message}";
            }

            return RedirectToAction("PagarCredito");
        }
        //
        // Acción para generar la tabla de amortización{


        public System.Web.Mvc.ActionResult GenerarTabla(string codigoCliente, string total)
        {
            // Verifica si los parámetros son válidos
            if (string.IsNullOrWhiteSpace(codigoCliente) || string.IsNullOrWhiteSpace(total))
            {
                TempData["Error"] = "Faltan parámetros necesarios.";
                return RedirectToAction("PagarCredito");
            }

            // Convierte el valor total a un tipo numérico
            if (!double.TryParse(total, out double totalDouble))
            {
                TempData["Error"] = "El total no es un valor válido.";
                return RedirectToAction("PagarCredito");
            }

            // Asignar los valores a ViewBag para que estén disponibles en la vista
            ViewBag.CodigoCliente = codigoCliente;
            ViewBag.Total = total;

            // Aquí, puedes realizar cualquier cálculo adicional para la amortización o validaciones
            // Como ejemplo, supongamos que generas un modelo de amortización vacío o con valores iniciales.

            return View();
        }



        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> GenerarTabla(int codigoCliente, string total, int cuota)
        {
            total = total.Replace(",", ".");
            decimal totalPrestamo;
            if (!decimal.TryParse(total, out totalPrestamo))
            {
                // Manejar el error de análisis de valores, tal vez devolver un mensaje de validación
                return View("Error", new { message = "Total de préstamo inválido" });
            }
           // codigoCliente = 1;
            //totalPrestamo = 25;
           // cuota = 3;
            // URL para crear la tabla de amortización
            string createTableUrl = $"{BASE_URL}credito/crear-tabla-amortizacion?codCliente={codigoCliente}&valorPrestamo={total}&numCuotas={cuota}";
            string generarFactura = $"{BASE_URL}factura/crear?formaPago=CR&codCliente={codigoCliente}&total={total}&Cantidad={CarritoItems.Count}";

            try
            {
                // Hacer las peticiones POST
                var createResponse = await _httpClient.PostAsync(createTableUrl, null);
                var createResponseFactura = await _httpClient.PostAsync(generarFactura, null);

                // Obtener la tabla de amortización
                string getTableUrl = $"{BASE_URL}credito/obtener-tabla-amortizacion/{codigoCliente}";
                var getResponse = await _httpClient.GetAsync(getTableUrl);

                if (getResponse.IsSuccessStatusCode)
                {
                    var json = await getResponse.Content.ReadAsStringAsync();
                    var GenerarTabla = JsonConvert.DeserializeObject<List<Models.AmortizacionDetalle>>(json);
                    return View(GenerarTabla); // Asegúrate de que la vista espera un modelo de tipo List<AmortizacionDetalle>
                }
                else
                {
                    // Manejar el error al obtener la tabla
                    return View("Error", new { message = "No se pudo obtener la tabla de amortización" });
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier otro error
                return View("Error", new { message = ex.Message });
            }
        }

        ///

        public System.Web.Mvc.ActionResult PagarEfectivo()
        {

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> GuardarFactura(string cedula, decimal total)
        {
            try
            {
                // Verificar si la cédula está vacía
                if (string.IsNullOrWhiteSpace(cedula))
                {
                    ViewBag.ErrorMessage = "Por favor, ingrese una cédula.";
                    return View("PagarEfectivo");
                }

                // Obtener el código del cliente
                string urlCliente = $"{BASE_URL}cliente/codigo/{cedula}";
                var responseCliente = await _httpClient.GetAsync(urlCliente);

                if (!responseCliente.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "No se pudo obtener el código del cliente.";
                    return View("PagarEfectivo");
                }

                var clienteJson = await responseCliente.Content.ReadAsStringAsync();
                var clienteData = JsonConvert.DeserializeObject<ClienteResponse>(clienteJson);

                // Calcular total con descuento
                decimal totalConDescuento = total - (total * (DESCUENTO_PORCENTAJE / 100m));
                // Convertir el total (string) para aceptar tanto punto como coma
                string totalConPunto = total.ToString().Replace(',', '.'); // Reemplazar coma por punto
                decimal totalDecimal = decimal.Parse(totalConPunto, CultureInfo.InvariantCulture); // Convertir a decimal

                // Crear la factura
                string urlFactura = $"{BASE_URL}factura/crear?codCliente={clienteData.CodigoCliente}&total={totalConPunto}&formaPago=EF&Cantidad={CarritoItems.Count}";
                var responseFactura = await _httpClient.PostAsync(urlFactura, null);

                if (!responseFactura.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "No se pudo generar la factura.";

                    return View("PagarEfectivo");
                }

                // Mostrar mensaje de éxito
                ViewBag.Message = $"Factura generada exitosamente.\n\nCédula: {cedula}\nTotal Original: ${total:N2}\nDescuento: {DESCUENTO_PORCENTAJE}%\nTotal con Descuento: ${totalConDescuento:N2}";
                return View("PagarEfectivo");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View("PagarEfectivo");
            }
        }

        public class Telefono
        {
            [JsonProperty("COD_TEL")]
            public int Id { get; set; }
            [JsonProperty("NOMBRE")]
            public string Nombre { get; set; }
            [JsonProperty("PRECIO")]
            public decimal Precio { get; set; }
            [JsonProperty("FOTO")]
            public string Foto { get; set; }  // Add the Foto property
        }
        public class AmortizacionDetalle
        {
            public int NumeroCuota { get; set; }
            public decimal ValorCuota { get; set; }
            public decimal InteresPagado { get; set; }
            public decimal CapitalPagado { get; set; }
            public decimal Saldo { get; set; }
        }
        // Clase para deserializar la respuesta del cliente
        private class ClienteResponse
        {
            public int CodigoCliente { get; set; }
            public string Mensaje { get; set; }
        }

        // Clase para representar la factura
        public class Factura
        {
            public int IdFactura { get; set; }
            public string NombreCliente { get; set; }
            public string Cedula { get; set; }
            public DateTime Fecha { get; set; }
            public decimal Total { get; set; }
            public string FormaPago { get; set; }
            public int Cantidad { get; set; }
        }
        // Clases para deserializar las respuestas de las API
        private class CreditoResponse
        {
            public bool EsSujetoDCredito { get; set; }
            public string Mensaje { get; set; }
        }
        private class MontoMaximoResponse
        {
            public decimal MontoMaximoCredito { get; set; }
            public string Mensaje { get; set; }
        }
        public class PagarCreditoViewModel
        {
            public string Cedula { get; set; }
            public double Total { get; set; }
            public string Mensaje { get; set; }
        }
    }
    // ApiResponse for client data
    public class ApiResponse
    {
        public int? CodigoCliente { get; set; }
        public string Mensaje { get; set; }
    }

    // Amortization Details for the response data
    public class AmortizacionDetalle
    {
        public int NumeroCuota { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal InteresPagado { get; set; }
        public decimal CapitalPagado { get; set; }
        public decimal Saldo { get; set; }
    }

}
