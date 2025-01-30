using CLICON_EUREKABANK.Modelo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Servicios
{
    class BanquitoServices
    {
        private readonly HttpClient _httpClient;

        string urlIp = "http://localhost:888/api";
        string urlIP = "";

        public BanquitoServices()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors) => true
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task<bool> loginService(UserModel user)
        {
            try
            {
                string url = $"{urlIp}/telefono/ValidarCredenciales?usuario={user.username}&contrasena={user.password}";
                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return bool.Parse(responseContent);
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar credenciales: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> AgregarTelefonoService(string nombre, decimal precio, string foto)
        {
            try
            {
                // Construct the payload for the request
                var telefono = new
                {
                    Nombre = nombre,
                    Precio = precio,
                    Foto = foto
                };

                // Serialize the payload to JSON
                var content = new StringContent(
                    JsonSerializer.Serialize(telefono),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                // Send the POST request
                string url = $"{urlIp}/telefono/insertar";
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Teléfono agregado exitosamente.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al agregar teléfono: {response.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en AgregarTelefonoService: " + ex.Message);
                return false;
            }
        }


        public async Task<bool> EliminarTelefonoService(int id)
        {
            try
            {
                string url = $"{urlIp}/telefono/eliminar/{id}";
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Teléfono eliminado exitosamente.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al eliminar teléfono: {response.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en EliminarTelefonoService: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> ActualizarTelefonoService(int codTel, string nombre, decimal precio, string foto)
        {
            try
            {
                // Construct the payload for the request
                var telefono = new
                {
                    CodTel = codTel,
                    Nombre = nombre,
                    Precio = precio,
                    Foto = foto // Base64 image string
                };

                // Serialize the payload to JSON
                var content = new StringContent(
                    JsonSerializer.Serialize(telefono),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                // Send the PUT request
                string url = $"{urlIp}/telefono/actualizar";
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Teléfono actualizado exitosamente.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al actualizar teléfono: {response.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarTelefonoService: " + ex.Message);
                return false;
            }
        }

        public async Task<List<Telefono>> ObtenerTelefonosService()
        {
            try
            {
                string url = $"{urlIp}/telefono/obtener";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Telefono>>(responseContent);
                }

                Console.WriteLine($"Error al obtener teléfonos: {response.ReasonPhrase}");
                return new List<Telefono>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ObtenerTelefonosService: " + ex.Message);
                return new List<Telefono>();
            }
        }

        public async Task<int?> ObtenerCodigoCliente(string cedula)
        {
            try
            {
                string url = $"{urlIp}/cliente/codigo/{cedula}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize using System.Text.Json
                    var clienteResponse = System.Text.Json.JsonSerializer.Deserialize<ClienteResponse>(responseContent);

                    // Return the CodigoCliente value from the response
                    return clienteResponse?.CodigoCliente;
                }

                Console.WriteLine($"Error al obtener el código del cliente: {response.ReasonPhrase}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCodigoCliente: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> CrearFactura(int codCliente, decimal total, string formaPago,int cantidad)
        {
            try
            {
                // Format the 'total' value to use a dot (.) as the decimal separator
                string formattedTotal = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                // Construct the URL with the formatted total
                string url = $"{urlIp}/factura/crear?codCliente={codCliente}&total={formattedTotal}&formaPago={formaPago}&cantidad={cantidad}";

                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Factura creada exitosamente.");
                    return true;
                }

                Console.WriteLine($"Error al crear la factura: {response.ReasonPhrase}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CrearFactura: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> verificarSujetoAcredito(string cedula)
        {
            try
            {
                string url = $"{urlIp}/credito/verificar/{cedula}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into the VerificarCreditoResponse object
                    var result = System.Text.Json.JsonSerializer.Deserialize<VerificarCreditoResponse>(responseContent);

                    if (result != null)
                    {
                        return result.EsSujetoDCredito;
                    }
                    else
                    {
                        Console.WriteLine("Error: No se pudo deserializar la respuesta.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"Error al verificar sujeto a crédito: {response.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en verificarSujetoAcredito: {ex.Message}");
                return false;
            }
        }

        public async Task<MontoMaximoResponse?> ObtenerMontoMaximo(int idCliente)
        {
            try
            {
                string url = $"{urlIp}/credito/monto-maximo/{idCliente}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize the JSON response into the MontoMaximoResponse object
                    return System.Text.Json.JsonSerializer.Deserialize<MontoMaximoResponse>(responseContent);
                }
                else
                {
                    Console.WriteLine($"Error al obtener el monto máximo: {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerMontoMaximo: {ex.Message}");
                return null;
            }
        }
        public async Task<List<Factura>> ObtenerFacturas()
        {
            try
            {
                string url = $"{urlIp}/factura/obtener-facturas";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Factura>>(responseContent);
                }
                else
                {
                    Console.WriteLine($"Error al obtener facturas: {response.ReasonPhrase}");
                    return new List<Factura>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerFacturas: {ex.Message}");
                return new List<Factura>();
            }
        }

        public async Task<List<Amortizacion>> ObtenerTablaCliente(int idCliente)
        {
            try
            {
                string url = $"{urlIp}/credito/obtener-tabla-amortizacion/{idCliente}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize the JSON response into a list of Amortizacion objects
                    return System.Text.Json.JsonSerializer.Deserialize<List<Amortizacion>>(responseContent);
                }
                else
                {
                    Console.WriteLine($"Error al obtener la tabla de amortización: {response.ReasonPhrase}");
                    return new List<Amortizacion>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerTablaCliente: {ex.Message}");
                return new List<Amortizacion>();
            }
        }

        public async Task<string> CrearTablaAmortizacion(int idCliente, decimal valorPrestamo, int numCuotas)
        {
            try
            {
                // Format the decimal with a dot as the decimal separator
                string formattedValorPrestamo = valorPrestamo.ToString(CultureInfo.InvariantCulture);

                string url = $"{urlIp}/credito/crear-tabla-amortizacion?codCliente={idCliente}&valorPrestamo={formattedValorPrestamo}&numCuotas={numCuotas}";
                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent; // Return the success message
                }
                else
                {
                    return $"Error: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

    }
}
