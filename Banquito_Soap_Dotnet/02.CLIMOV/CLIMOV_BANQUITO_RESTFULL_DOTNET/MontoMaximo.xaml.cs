using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET
{
    public partial class MontoMaximo : ContentPage
    {
        private WSBANQUITO.WSBanquitoSoap _soapClient;

        public MontoMaximo()
        {
            InitializeComponent();
            _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);

        }

        private async void OnObtenerMontoMaximoClicked(object sender, EventArgs e)
        {
            // Obtener el número de cédula desde el Entry
            var cedula = EntryCedula.Text?.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                LabelResultado.Text = "Por favor ingrese un número de cédula.";
                return;
            }

            try
            {
                // Obtener el código del cliente
                var codigoCliente = await ObtenerCodigoCliente(cedula);

                if (codigoCliente == null)
                {
                    LabelResultado.Text = "No se pudo obtener el código del cliente.";
                    return;
                }

                // Obtener y mostrar el monto máximo de crédito
                double montoMaximo = await ObtenerMontoMaximoCredito(codigoCliente.Value);

                // Mostrar el resultado en el LabelResultado
                LabelResultado.Text = $"Monto Máximo de Crédito: ${montoMaximo:N2}";
            }
            catch (Exception ex)
            {
                LabelResultado.Text = $"Error: {ex.Message}";
            }
        }

        // Método para obtener el monto máximo de crédito usando el código de cliente
        private async Task<double> ObtenerMontoMaximoCredito(int codigoCliente)
        {
            // Crear el request body para calcular el monto máximo de crédito
            var requestMontoMaximoCreditoBody = new WSBANQUITO.CalcularMontoMaximoCreditoRequestBody
            {
                codCliente = codigoCliente
            };

            // Crear el request para calcular el monto máximo de crédito
            var requestMontoMaximoCredito = new WSBANQUITO.CalcularMontoMaximoCreditoRequest
            {
                Body = requestMontoMaximoCreditoBody
            };

            // Llamar al servicio SOAP para obtener el monto máximo de crédito
            var responseMontoMaximoCredito = await _soapClient.CalcularMontoMaximoCreditoAsync(requestMontoMaximoCredito);

            if (responseMontoMaximoCredito == null || responseMontoMaximoCredito.Body == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el monto máximo de crédito.", "OK");
                return 0; // Devolver 0 si no se puede obtener el monto
            }

            // Devolver el monto máximo de crédito
            return responseMontoMaximoCredito.Body.CalcularMontoMaximoCreditoResult.MontoMaximoCredito;
        }


        // Método para obtener el código de cliente usando la cédula
        private async Task<int?> ObtenerCodigoCliente(string cedula)
        {
            // Crear el request body para obtener el código del cliente
            var requestCodigoClienteBody = new WSBANQUITO.ObtenerCodigoClienteRequestBody
            {
                cedula = EntryCedula.Text
            };

            // Crear el request para obtener el código del cliente
            var requestCodigoCliente = new WSBANQUITO.ObtenerCodigoClienteRequest
            {
                Body = requestCodigoClienteBody
            };

            // Llamar al servicio SOAP para obtener el código del cliente
            var responseCodigoCliente = await _soapClient.ObtenerCodigoClienteAsync(requestCodigoCliente);

            if (responseCodigoCliente == null || responseCodigoCliente.Body == null || responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el código del cliente.", "OK");
                return null;
            }

            

            return (int)responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente; ;
        }

        


        // Clases para deserializar la respuesta de la API
        public class ApiResponse
        {
            public int? CodigoCliente { get; set; }
            public string Mensaje { get; set; }
        }

        public class ApiMontoMaximoResponse
        {
            public double? MontoMaximoCredito { get; set; }
            public string Mensaje { get; set; }
        }
    }
}
