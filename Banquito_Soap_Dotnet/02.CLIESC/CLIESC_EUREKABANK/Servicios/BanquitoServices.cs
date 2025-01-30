using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using WSBANQUITO;

namespace CLIESC_EUREKABANK.Servicios
{
    class BanquitoServices
    {
        private readonly WSBanquitoSoapClient _client;

        public BanquitoServices()
        {
            _client = new WSBanquitoSoapClient(WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
        }

        public async Task<bool> LoginService(string username, string password)
        {
            try
            {
                var response = await _client.ValidarCredencialesAsync(username,password);
                return response.Body.ValidarCredencialesResult;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during login: " + ex.Message);
                return false;
            }
        }

        public async Task<string> AgregarTelefonoService(string nombre, decimal precio, string foto)
        {
            try
            {
                var response = await _client.InsertarTelefonoAsync(nombre, precio, foto);
                
                return response.Body.InsertarTelefonoResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting phone: " + ex.Message);
                return null;
            }
        }

        public async Task<List<Telefono>> ObtenerTelefonosService()
        {
            try
            {
                var response = await _client.ObtenerTodosTelefonosAsync();
                return response.Body.ObtenerTodosTelefonosResult.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching phones: " + ex.Message);
                return new List<Telefono>();
            }
        }

        public async Task<string> ActualizarTelefonoService(int codTel, string nombre, decimal precio, string foto)
        {
            try
            {
                var response = await _client.ActualizarTelefonoAsync(codTel, nombre, precio, foto);
                return response.Body.ActualizarTelefonoResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating phone: " + ex.Message);
                return null;
            }
        }

        public async Task<string> EliminarTelefonoService(int codTel)
        {
            try
            {
                var result = await _client.EliminarTelefonoAsync(codTel);
                return result.Body.EliminarTelefonoResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while deleting phone: " + ex.Message);
                return null;
            }
        }

        public async Task<VerificarSujetoCreditoResult> VerificarSujetoCreditoService(string cedula)
        {
            try
            {
                var result = await _client.VerificarSujetoCreditoAsync(cedula);
                return result.Body.VerificarSujetoCreditoResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while verifying credit subject: " + ex.Message);
                return null;
            }
        }

        public async Task<CalcularMontoMaximoCreditoResult> CalcularMontoMaximoCreditoService(int codCliente)
        {
            try
            {
                var result = await _client.CalcularMontoMaximoCreditoAsync(codCliente);
                return result.Body.CalcularMontoMaximoCreditoResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while calculating max credit amount: " + ex.Message);
                return null;
            }
        }

        public async Task<ObtenerCodigoClienteResult> ObtenerCodigoCliente(string cedula)
        {
            try
            {
                var result = await _client.ObtenerCodigoClienteAsync(cedula);
                return result.Body.ObtenerCodigoClienteResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching client code: " + ex.Message);
                return null;
            }
        }

        public async Task<string> CrearTablaAmortizacionService(int codCliente, decimal valorPrestamo, int numCuotas)
        {
            try
            {
                var result = await _client.CrearTablaAmortizacionAsync(codCliente, valorPrestamo, numCuotas);
                return result.Body.CrearTablaAmortizacionResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while creating amortization table: " + ex.Message);
                return null;
            }
        }

        public async Task<List<Amortizacion>> ObtenerTablaAmortizacionService(int codCliente)
        {
            try
            {
                var result = await _client.ObtenerTablaAmortizacionAsync(codCliente);
                return result.Body.ObtenerTablaAmortizacionResult.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching amortization table: " + ex.Message);
                return new List<Amortizacion>();
            }
        }

        public async Task<CrearFacturaResult> CrearFacturaService(int codCliente, decimal total, string formaPago,int cantidad)
        {
            try
            {
                var result = await _client.CrearFacturaAsync(codCliente, total, formaPago,cantidad);
                return result.Body.CrearFacturaResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while creating invoice: " + ex.Message);
                return null;
            }
        }

        public async Task<List<Factura>> ObtenerFacturasService()
        {
            try
            {
                var result = await _client.ObtenerFacturasAsync();
                return result.Body.ObtenerFacturasResult.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching invoices: " + ex.Message);
                return new List<Factura>();
            }
        }
    }
}
