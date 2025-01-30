using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CLIWEB_BANQUITO_SOAP_DOTNET.Models; // Añadido para usar TelefonoConImagen
using System.ServiceModel; // Añadido para EndpointAddress
using System.Linq; // Añadido para Select
using System.Collections.Generic;
using CLIWEB_BANQUITO_SOAP_DOTNET.Services;
using CLIWEB_BANQUITO_SOAP_DOTNET.WSBANQUITO; // Añadido para List<>
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;

namespace CLIWEB_BANQUITO_SOAP_DOTNET.Controllers
{
    public class HomeController : Controller
    {
        private const decimal DESCUENTO_PORCENTAJE = 42m;
        private readonly WSBANQUITO.WSBanquitoSoapClient _soapClient;

        public HomeController()
        {
            // Usar el constructor por defecto o especificar el endpoint
            _soapClient = new WSBANQUITO.WSBanquitoSoapClient();

        }

        // Acción para la página principal
        public ActionResult Index()
        {
            return View();
        }

        // Acción para el menú
        public ActionResult Menu()
        {
            return View();
        }

        // Acción para la vista de Catálogos de Teléfonos
        public ActionResult CatalogosTelefonos()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AgregarTelefono(string nombre, decimal precio, string base64Image)
        {
            try
            {
                var response = await _soapClient.InsertarTelefonoAsync(nombre, precio, base64Image);
                var result = response.Body.InsertarTelefonoResult; // Acceder al cuerpo de la respuesta

                return Json(new
                {
                    success = result == "Teléfono insertado exitosamente.",
                    message = result
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EliminarTelefono(int codTel)
        {
            try
            {
                var response = await _soapClient.EliminarTelefonoAsync(codTel);
                var result = response.Body.EliminarTelefonoResult; // Acceder al cuerpo de la respuesta

                return Json(new
                {
                    success = result == "Teléfono eliminado exitosamente.",
                    message = result
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarTelefono(int codTel, string nombre, decimal precio, string base64Image)
        {
            try
            {
                var response = await _soapClient.ActualizarTelefonoAsync(codTel, nombre, precio, base64Image);
                var result = response.Body.ActualizarTelefonoResult; // Acceder al cuerpo de la respuesta

                return Json(new
                {
                    success = result == "Teléfono actualizado exitosamente.",
                    message = result
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerTelefonos()
        {
            try
            {
                var response = await _soapClient.ObtenerTodosTelefonosAsync();
                var telefonos = response.Body.ObtenerTodosTelefonosResult;

                if (telefonos == null || !telefonos.Any())
                {
                    return Json(new { success = false, message = "No se encontraron teléfonos." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    telefonos = telefonos
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Acción para la vista de Facturación
        public async Task<ActionResult> Facturacion(string marcaSeleccionada = "")
        {
            try
            {
                var response = await _soapClient.ObtenerTodosTelefonosAsync();
                var telefonos = response.Body.ObtenerTodosTelefonosResult;

                // Convertir todos los teléfonos primero
                var telefonosConImagen = telefonos
                    .Select(t => new TelefonoConImagen(t))
                    .ToList();

                // Obtener las marcas únicas
                var marcasUnicas = telefonosConImagen
                    .Select(t => t.NOMBRE.Split(' ')[0])
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                // Aplicar el filtro solo si se seleccionó una marca específica
                if (!string.IsNullOrEmpty(marcaSeleccionada) && marcaSeleccionada != "Todos")
                {
                    telefonosConImagen = telefonosConImagen
                        .Where(t => t.NOMBRE.StartsWith(marcaSeleccionada, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                ViewBag.Marcas = marcasUnicas;
                ViewBag.MarcaSeleccionada = marcaSeleccionada;

                return View(telefonosConImagen);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error: {ex.Message}");
                return View(new List<TelefonoConImagen>());
            }
        }

        // Acción para agregar al carrito
        [HttpPost]
        public async Task<JsonResult> AgregarAlCarrito(int codTel)
        {
            try
            {
                // Buscar en el carrito si ya existe el teléfono
                var telefonoEnCarrito = CarritoService.CarritoItems
                    .FirstOrDefault(t => t.COD_TEL == codTel);


                // Si no está en el carrito, buscarlo entre los teléfonos disponibles
                var response = await _soapClient.ObtenerTodosTelefonosAsync();
                var telefonos = response.Body.ObtenerTodosTelefonosResult;
                var telefono = telefonos.FirstOrDefault(t => t.COD_TEL == codTel);

                if (telefono != null)
                {
                    // Convertir Telefono a TelefonoConImagen y agregarlo al carrito
                    var telefonoConImagen = new TelefonoConImagen(telefono);
                    CarritoService.AgregarAlCarrito(telefonoConImagen);

                    return Json(new { success = true, message = "Producto agregado al carrito" });
                }
                else
                {
                    return Json(new { success = false, message = "Teléfono no encontrado." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Acción para mostrar el carrito
        public ActionResult Carrito()
        {
            ViewBag.Total = CarritoService.Total;
            return View(CarritoService.CarritoItems);
        }

        // Acción para agregar un teléfono al carrito desde la vista
        [HttpPost]
        public ActionResult Agregar(TelefonoConImagen telefono)
        {
            // Agregar el teléfono al carrito
            CarritoService.AgregarAlCarrito(telefono);
            return RedirectToAction("Carrito");
        }

        // Acción para eliminar un teléfono del carrito
        [HttpPost]
        public JsonResult EliminarDelCarrito(int codTel)
        {
            try
            {
                var telefono = CarritoService.CarritoItems.FirstOrDefault(t => t.COD_TEL == codTel);

                if (telefono != null)
                {
                    CarritoService.EliminarDelCarrito(telefono);  // Asegúrate de que este método funcione correctamente
                    return Json(new { success = true, message = "Producto eliminado correctamente" });
                }

                return Json(new { success = false, message = "El producto no se encuentra en el carrito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        public ActionResult PagarEfectivo()
        {
            return View();
        }

        //PAGAR EFECTIVO
        // Acción para pagar factura en efectivo
        [HttpPost]
        public async Task<ActionResult> PagarFactura(string cedula, decimal total, int cantidad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cedula))
                {
                    ModelState.AddModelError("Cedula", "Por favor, ingrese una cédula.");
                    return View();
                }

                // Paso 1: Obtener el código del cliente
                var responseCliente = await _soapClient.ObtenerCodigoClienteAsync(cedula);

                if (responseCliente == null || responseCliente.Body == null)
                {
                    ModelState.AddModelError("ErrorCliente", "No se pudo obtener el código del cliente.");
                    return View();
                }

                int clienteCodigo = (int)responseCliente.Body.ObtenerCodigoClienteResult.CodigoCliente;

                // Paso 2: Calcular el total con descuento
                decimal totalConDescuento = CalcularTotalConDescuento(total);

                // Paso 3: Crear la solicitud de factura
                var responseFactura = await _soapClient.CrearFacturaAsync(clienteCodigo, totalConDescuento, "EF", cantidad);

                if (responseFactura == null || responseFactura.Body == null)
                {
                    ModelState.AddModelError("ErrorFactura", "No se pudo crear la factura.");
                    return View();
                }

                ViewBag.Mensaje = "Factura generada con éxito.";
                ViewBag.TotalConDescuento = totalConDescuento;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorGeneral", $"Error: {ex.Message}");
                return View();
            }
        }

        // Método para calcular el total con descuento
        private decimal CalcularTotalConDescuento(decimal total)
        {
            if (total <= 0)
            {
                throw new ArgumentException("El total debe ser mayor a cero.");
            }
            return total - (total * DESCUENTO_PORCENTAJE / 100);
        }
        //Guardar Factura
        [HttpPost]
        public async Task<JsonResult> GuardarFactura(string cedula, decimal total, int cantidad)
        {
            try
            {
                // Validar entrada
                if (string.IsNullOrWhiteSpace(cedula))
                {
                    return Json(new { success = false, message = "Por favor, ingrese una cédula válida." });
                }

                if (total <= 0)
                {
                    return Json(new { success = false, message = "El total debe ser mayor a 0." });
                }

                if (cantidad <= 0)
                {
                    return Json(new { success = false, message = "La cantidad debe ser mayor a 0." });
                }

                // Obtener el código del cliente
                var responseCliente = await _soapClient.ObtenerCodigoClienteAsync(cedula);

                if (responseCliente?.Body?.ObtenerCodigoClienteResult?.CodigoCliente == null)
                {
                    return Json(new { success = false, message = "No se pudo obtener el código del cliente." });
                }

                int clienteCodigo = responseCliente.Body.ObtenerCodigoClienteResult.CodigoCliente.Value;

                // Crear la factura
                var responseFactura = await _soapClient.CrearFacturaAsync(clienteCodigo, total, "EF", cantidad);

                if (responseFactura?.Body?.CrearFacturaResult == null)
                {
                    return Json(new { success = false, message = "Error al crear la factura." });
                }

                if (responseFactura.Body.CrearFacturaResult.Mensaje != "Factura creada exitosamente")
                {
                    return Json(new { success = false, message = responseFactura.Body.CrearFacturaResult.Mensaje });
                }

                // Vaciar el carrito
                CarritoService.VaciarCarrito();

                return Json(new
                {
                    success = true,
                    message = "Factura generada exitosamente.",
                    details = new
                    {
                        Cedula = cedula,
                        Total = total,
                        Cantidad = cantidad
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }
        //Ver Facturas
        public ActionResult VerFacturas()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerFacturas()
        {
            try
            {
                // Invocar el servicio SOAP para obtener las facturas
                var response = await _soapClient.ObtenerFacturasAsync();

                // Depurar el contenido de la respuesta
                Console.WriteLine("Respuesta SOAP: " + response?.Body?.ObtenerFacturasResult);

                // Verificar si la respuesta es válida
                if (response?.Body?.ObtenerFacturasResult == null)
                {
                    return Json(new { success = false, message = "No se encontraron facturas." });
                }

                // Procesar la lista de facturas de manera más segura
                var facturas = response.Body.ObtenerFacturasResult
                    .Where(f => f != null) // Filtrar elementos nulos
                    .Select(f => new
                    {
                        IdFactura = f.IdFactura,
                        NombreCliente = f.NombreCliente ?? "Sin nombre",
                        Cedula = f.Cedula ?? "Sin cédula",
                        Fecha = f.Fecha,
                        Total = f.Total,
                        FormaPago = f.FormaPago ?? "EF",
                        Descuento = f.Descuento,
                        Cantidad = f.Cantidad
                    })
                    .ToList();

                if (!facturas.Any())
                {
                    return Json(new { success = false, message = "No hay facturas para mostrar." });
                }

                return Json(new { success = true, facturas });
            }
            catch (Exception ex)
            {
                // Log del error detallado
                Console.WriteLine($"Error detallado: {ex.ToString()}");
                return Json(new
                {
                    success = false,
                    message = "Error al obtener las facturas. Por favor, inténtelo de nuevo.",
                    details = ex.Message
                });
            }
        }

        //Pagar Credito
        // Controlador para Pagar Crédito
        public ActionResult PagarCredito(decimal total)
        {
            ViewBag.Total = total;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PagarCredito(string cedula, decimal total)
        {
            ViewBag.Total = total;

            if (string.IsNullOrWhiteSpace(cedula))
            {
                ViewBag.Error = "Por favor, ingrese una cédula.";
                return View();
            }

            try
            {
                // Verificar si es sujeto a crédito
                var verificarCreditoResponse = await _soapClient.VerificarSujetoCreditoAsync(cedula);

                if (verificarCreditoResponse?.Body?.VerificarSujetoCreditoResult?.EsSujetoCredito != true)
                {
                    // Decodificar el mensaje de error si contiene caracteres HTML codificados
                    ViewBag.Error = WebUtility.HtmlDecode(verificarCreditoResponse?.Body?.VerificarSujetoCreditoResult?.Mensaje ?? "No sujeto a crédito.");
                    return View();
                }

                // Obtener código de cliente
                var codigoClienteResponse = await _soapClient.ObtenerCodigoClienteAsync(cedula);

                if (codigoClienteResponse?.Body?.ObtenerCodigoClienteResult?.CodigoCliente == null)
                {
                    ViewBag.Error = "No se pudo obtener el código del cliente.";
                    return View();
                }

                int clienteCodigo = codigoClienteResponse.Body.ObtenerCodigoClienteResult.CodigoCliente.Value;

                // Calcular monto máximo de crédito
                var montoMaximoCreditoResponse = await _soapClient.CalcularMontoMaximoCreditoAsync(clienteCodigo);

                if (montoMaximoCreditoResponse?.Body?.CalcularMontoMaximoCreditoResult == null)
                {
                    ViewBag.Error = "No se pudo calcular el monto máximo de crédito.";
                    return View();
                }

                double montoMaximoCredito = montoMaximoCreditoResponse.Body.CalcularMontoMaximoCreditoResult.MontoMaximoCredito;

                // Lógica para validar si el cliente puede realizar el crédito
                if (total <= (decimal)montoMaximoCredito)
                {
                    // Guardar el mensaje en TempData para que sea accesible en la siguiente acción
                    TempData["SuccessMessage"] = "El cliente puede realizar el crédito.";

                    // Redirigir a GenerarTabla, pasando el código del cliente y el total
                    return RedirectToAction("GenerarTabla", new { codCliente = clienteCodigo, total = total });
                }

                else
                {
                    ViewBag.Error = "El monto de la factura supera el monto máximo aprobado.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ocurrió un error: {ex.Message}";
            }

            return View();
        }
        public ActionResult GenerarTabla()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GenerarTabla(int codCliente, string total, int numeroCuotas)
        {
            try
            {
                // Validar número de cuotas
                if (numeroCuotas < 3 || numeroCuotas > 18)
                {
                    ModelState.AddModelError("NumeroCuotas", "El número de cuotas debe estar entre 3 y 18");
                    return View();
                }

                // Llamar al servicio SOAP para crear la tabla de amortización
                // Aquí pasamos los parámetros directamente como lo espera el servicio
                var responseCrearTabla = _soapClient.CrearTablaAmortizacion(codCliente, Convert.ToDecimal(total), numeroCuotas);

                // Verificar si la respuesta es válida
                if (responseCrearTabla == null)
                {
                    ModelState.AddModelError("", "No se pudo generar la tabla de amortización");
                    return View();
                }

                // Si la respuesta es válida, procesarla y devolver la vista
                // Suponiendo que `responseCrearTabla` tiene la tabla de amortización que necesitamos
                // Obtener la tabla de amortización
                var requestTablaBody = new ObtenerTablaAmortizacionRequestBody
                {
                    codCliente = codCliente // Aquí deberías pasar solo el código del cliente como un int
                };

                var requestTabla = new ObtenerTablaAmortizacionRequest
                {
                    Body = requestTablaBody
                };

                var responseTabla = await _soapClient.ObtenerTablaAmortizacionAsync(codCliente);

                if (responseTabla?.Body?.ObtenerTablaAmortizacionResult == null)
                {
                    ViewBag.Error = "No se pudo obtener la tabla de amortización.";
                    return View();
                }

                var tablaAmortizacion = responseTabla.Body.ObtenerTablaAmortizacionResult;

                if (tablaAmortizacion.Length == 0)
                {
                    ViewBag.Error = "No se encontró tabla de amortización para esta cédula.";
                    return View();
                }

                // Pasar la tabla de amortización a la vista
                return View("TablaAmortizacion", tablaAmortizacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View();
            }
        }

        // Función para procesar la respuesta de acuerdo al tipo de respuesta de tu servicio SOAP
        private object ProcesarRespuesta(string respuesta)
        {
            try
            {
                // Si la respuesta es un JSON, puedes deserializarla así:
                var tablaAmortizacion = JsonConvert.DeserializeObject(respuesta); // Usando Json.NET (Newtonsoft.Json)
                return tablaAmortizacion;
            }
            catch (Exception ex)
            {
                // Si la respuesta no es JSON, maneja el error aquí
                throw new Exception($"Error al procesar la respuesta: {ex.Message}");
            }
        }






        // Acción para la vista MontoMaximo
        public ActionResult MontoMaximo()
        {
            return View();
        }
        // Acción POST para obtener el monto máximo de crédito
        [HttpPost]
        public async Task<ActionResult> MontoMaximo(string cedula)
        {
            // Verificar que la cédula no esté vacía
            if (string.IsNullOrEmpty(cedula))
            {
                ViewBag.Error = "Por favor ingrese un número de cédula.";
                return View();
            }

            try
            {
                // Obtener el código de cliente con la cédula
                var codigoCliente = await ObtenerCodigoCliente(cedula);

                if (codigoCliente == null)
                {
                    ViewBag.Error = "No se pudo obtener el código del cliente.";
                    return View();
                }

                // Obtener el monto máximo de crédito usando el código de cliente
                double montoMaximo = await ObtenerMontoMaximoCredito(codigoCliente.Value);

                // Mostrar el monto máximo de crédito en la vista
                ViewBag.MontoMaximo = montoMaximo;

                // Si todo fue exitoso, se puede mostrar un mensaje de éxito
                ViewBag.Success = "Monto máximo de crédito obtenido exitosamente.";  

                return View();
            }
            catch (Exception ex)
            {
                // En caso de error, mostrar el mensaje de error
                ViewBag.Error = $"Error al obtener el monto máximo de crédito: {ex.Message}";

                return View();
            }
        }

        // Método para obtener el código del cliente usando la cédula
        private async Task<int?> ObtenerCodigoCliente(string cedula)
        {
            // Llamar al servicio SOAP directamente con la cédula como parámetro
            var response = await _soapClient.ObtenerCodigoClienteAsync(cedula);

            // Verificar que se haya obtenido correctamente el código del cliente
            return response?.Body?.ObtenerCodigoClienteResult?.CodigoCliente;
        }

        // Método para obtener el monto máximo de crédito usando el código de cliente
        private async Task<double> ObtenerMontoMaximoCredito(int codigoCliente)
        {
            // Llamar al servicio SOAP directamente con el código de cliente como parámetro
            var response = await _soapClient.CalcularMontoMaximoCreditoAsync(codigoCliente);

            // Verificar que se haya obtenido el monto máximo de crédito
            return response?.Body?.CalcularMontoMaximoCreditoResult?.MontoMaximoCredito ?? 0;
        }
        public ActionResult TablaAmortizacion()
        {
            // Renderiza la vista para ingresar la cédula
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ObtenerTablaAmortizacion(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                ViewBag.Error = "Por favor, ingrese un número de cédula válido.";
                return View();
            }

            try
            {
                // Obtener el código del cliente
                var requestCodigoClienteBody = new ObtenerCodigoClienteRequestBody
                {
                    cedula = cedula
                };

                var requestCodigoCliente = new ObtenerCodigoClienteRequest
                {
                    Body = requestCodigoClienteBody // Asegúrate de que el body sea el que se pasa
                };

                var responseCodigoCliente = await _soapClient.ObtenerCodigoClienteAsync(cedula);



                if (responseCodigoCliente?.Body?.ObtenerCodigoClienteResult?.CodigoCliente == null)
                {
                    ViewBag.Error = "No se pudo obtener el código del cliente.";
                    return View();
                }

                int codigoCliente = (int)responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente;

                // Obtener la tabla de amortización
                var requestTablaBody = new ObtenerTablaAmortizacionRequestBody
                {
                    codCliente = codigoCliente // Aquí deberías pasar solo el código del cliente como un int
                };

                var requestTabla = new ObtenerTablaAmortizacionRequest
                {
                    Body = requestTablaBody
                };

                var responseTabla = await _soapClient.ObtenerTablaAmortizacionAsync(codigoCliente);

                if (responseTabla?.Body?.ObtenerTablaAmortizacionResult == null)
                {
                    ViewBag.Error = "No se pudo obtener la tabla de amortización.";
                    return View();
                }

                var tablaAmortizacion = responseTabla.Body.ObtenerTablaAmortizacionResult;

                if (tablaAmortizacion.Length == 0)
                {
                    ViewBag.Error = "No se encontró tabla de amortización para esta cédula.";
                    return View();
                }

                // Pasar la tabla de amortización a la vista
                return View("TablaAmortizacion", tablaAmortizacion);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al procesar la solicitud: {ex.Message}";
                return View();
            }
        }
        public class AmortizacionDetalle
        {
            public int NumeroCuota { get; set; }
            public decimal ValorCuota { get; set; }
            public decimal InteresPagado { get; set; }
            public decimal CapitalPagado { get; set; }
            public decimal Saldo { get; set; }
        }

    }
}
