using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Text.Json;
namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;
public partial class PagarEfectivo : ContentPage
{
    private decimal totalOriginal;
    private const decimal DESCUENTO_PORCENTAJE = 42m;
    private decimal totalConDescuento;
    private WSBANQUITO.WSBanquitoSoap _soapClient;
    int _cantidad;

    string totalStr;

    public PagarEfectivo(string total, int cantidad)
    {
        InitializeComponent();
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
        _cantidad = cantidad;
        totalStr = total;

        // Convertir el total a decimal
        if (decimal.TryParse(total, out totalOriginal))
        {
            // Calcular total con descuento
            
            totalConDescuento = CalcularTotalConDescuento(totalOriginal);

            // Mostrar valores
            LabelTotal.Text = $"Total: ${totalOriginal:N2}";
            LabelDescuento.Text = $"Descuento: {DESCUENTO_PORCENTAJE}%";
            LabelTotalConDescuento.Text = $"Total con Descuento: ${totalConDescuento:N2}";
        }
        
    }

    private decimal CalcularTotalConDescuento(decimal total)
    {
        // Calcular el total con descuento del 42%
        return total - (total * (DESCUENTO_PORCENTAJE / 100m));
    }

    private async void OnGuardarFacturaClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EntryCedula.Text))
            {
                await DisplayAlert("Error", "Por favor, ingrese una cédula.", "OK");
                return;
            }

            // Crear la solicitud SOAP
            var requestBody = new WSBANQUITO.ObtenerCodigoClienteRequestBody(EntryCedula.Text);
            var requestCliente = new WSBANQUITO.ObtenerCodigoClienteRequest
            {
                Body = requestBody
            };

            // Llamar al servicio SOAP para obtener el código del cliente
            var responseCliente = await _soapClient.ObtenerCodigoClienteAsync(requestCliente);

            if (responseCliente == null || responseCliente.Body == null )
            {
                await DisplayAlert("Error", "No se pudo obtener el código del cliente.", "OK");
                return;
            }

            int clienteCodigo = (int)responseCliente.Body.ObtenerCodigoClienteResult.CodigoCliente;


            var requestBody1 = new WSBANQUITO.CrearFacturaRequestBody(clienteCodigo,Convert.ToDecimal(totalStr),"EF", _cantidad);
            var requestFactura = new WSBANQUITO.CrearFacturaRequest
            {
                Body = requestBody1
            };
            

            // Llamar al servicio SOAP para crear la factura
            var responseFactura = await _soapClient.CrearFacturaAsync(requestFactura);

            if (responseFactura == null || responseFactura.Body.CrearFacturaResult.Mensaje != "Factura creada exitosamente") 
            {
                await DisplayAlert("Error", "No se pudo generar la factura.", "OK");
                return;
            }

            // Mostrar mensaje de éxito con detalles
            await DisplayAlert("Éxito",
                $"Factura generada exitosamente.\n\n" +
                $"Cédula: {EntryCedula.Text}\n" +
                $"Total Original: ${totalOriginal:N2}\n" +
                $"Descuento: {DESCUENTO_PORCENTAJE}%\n" +
                $"Total con Descuento: ${totalConDescuento:N2}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }


    // Clase para deserializar la respuesta del cliente
    private class ClienteResponse
    {
        public int CodigoCliente { get; set; }
        public string Mensaje { get; set; }
    }
}