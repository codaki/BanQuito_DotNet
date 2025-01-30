using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Text.Json;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class PagarCredito : ContentPage
{
    private string total;
    private WSBANQUITO.WSBanquitoSoap _soapClient;
    int _cantidad;

    public PagarCredito(string total, int cantidad)
    {
        InitializeComponent();
        _cantidad = cantidad;
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
        this.total = total;
        LabelTotal.Text = $"Total: ${total}";
    }

    private async void OnVerificarCreditoClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EntryCedula.Text))
            {
                await DisplayAlert("Error", "Por favor, ingrese una c�dula.", "OK");
                return;
            }

            // Crear el request body para verificar si es sujeto a cr�dito
            var requestVerificarCreditoBody = new WSBANQUITO.VerificarSujetoCreditoRequestBody
            {
                cedula = EntryCedula.Text
            };

            // Crear el request para verificar el sujeto a cr�dito
            var requestVerificarCredito = new WSBANQUITO.VerificarSujetoCreditoRequest
            {
                Body = requestVerificarCreditoBody
            };

            // Llamar al servicio SOAP para verificar el cr�dito
            var responseVerificarCredito = await _soapClient.VerificarSujetoCreditoAsync(requestVerificarCredito);

            if (responseVerificarCredito == null || responseVerificarCredito.Body == null || !responseVerificarCredito.Body.VerificarSujetoCreditoResult.EsSujetoCredito)
            {
                await DisplayAlert("No sujeto a cr�dito", responseVerificarCredito?.Body?.VerificarSujetoCreditoResult.Mensaje ?? "No se pudo verificar el cr�dito.", "OK");
                return;
            }

            // Crear el request body para obtener el c�digo del cliente
            var requestCodigoClienteBody = new WSBANQUITO.ObtenerCodigoClienteRequestBody
            {
                cedula = EntryCedula.Text
            };

            // Crear el request para obtener el c�digo del cliente
            var requestCodigoCliente = new WSBANQUITO.ObtenerCodigoClienteRequest
            {
                Body = requestCodigoClienteBody
            };

            // Llamar al servicio SOAP para obtener el c�digo del cliente
            var responseCodigoCliente = await _soapClient.ObtenerCodigoClienteAsync(requestCodigoCliente);

            if (responseCodigoCliente == null || responseCodigoCliente.Body == null || responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el c�digo del cliente.", "OK");
                return;
            }

            int clienteCodigo = (int)responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente;

            // Crear el request body para calcular el monto m�ximo de cr�dito
            var requestMontoMaximoCreditoBody = new WSBANQUITO.CalcularMontoMaximoCreditoRequestBody
            {
                codCliente = clienteCodigo
            };

            // Crear el request para calcular el monto m�ximo de cr�dito
            var requestMontoMaximoCredito = new WSBANQUITO.CalcularMontoMaximoCreditoRequest
            {
                Body = requestMontoMaximoCreditoBody
            };

            // Llamar al servicio SOAP para obtener el monto m�ximo de cr�dito
            var responseMontoMaximoCredito = await _soapClient.CalcularMontoMaximoCreditoAsync(requestMontoMaximoCredito);

            if (responseMontoMaximoCredito == null || responseMontoMaximoCredito.Body == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el monto m�ximo de cr�dito.", "OK");
                return;
            }

            var montoMaximoCredito = responseMontoMaximoCredito.Body.CalcularMontoMaximoCreditoResult.MontoMaximoCredito;

            // Verificar si el monto total es menor o igual al monto m�ximo
            if (double.TryParse(total, out double totalDouble) && totalDouble <= montoMaximoCredito)
            {
                await DisplayAlert("�xito", "El cliente puede realizar el cr�dito.", "OK");
                // Redirigir a la p�gina GenerarTabla
                await Navigation.PushAsync(new GenerarTabla(clienteCodigo, total, _cantidad));
            }
            else
            {
                await DisplayAlert("Cr�dito no aprobado", "El monto de la factura supera el monto m�ximo aprobado.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
        }
    }



    // Clases para deserializar las respuestas de las API
    private class CreditoResponse
    {
        public bool EsSujetoDCredito { get; set; }
        public string Mensaje { get; set; }
    }

    private class ClienteResponse
    {
        public int CodigoCliente { get; set; }
        public string Mensaje { get; set; }
    }

    private class MontoMaximoResponse
    {
        public double MontoMaximoCredito { get; set; }
        public string Mensaje { get; set; }
    }
}
