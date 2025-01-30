using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Text.Json;
namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;
public partial class PagarEfectivo : ContentPage
{
    private decimal totalOriginal;
    private const decimal DESCUENTO_PORCENTAJE = 42m;
    private decimal totalConDescuento;
    string totalStr;
    int cantidadTotal;
    

    public PagarEfectivo(string total, int cantidad)
    {
        InitializeComponent();
        totalStr = total;
        cantidadTotal = cantidad;

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

            using var client = new HttpClient();

            // Obtener código del cliente
            string urlCliente = $"{ApiConfig.BaseUrl}cliente/codigo/{EntryCedula.Text}";
            var responseCliente = await client.GetAsync(urlCliente);

            if (!responseCliente.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "No se pudo obtener el código del cliente.", "OK");
                return;
            }

            var clienteJson = await responseCliente.Content.ReadAsStringAsync();
            var clienteData = JsonSerializer.Deserialize<ClienteResponse>(clienteJson);

            // Crear factura con el total con descuento
            string urlFactura = $"{ApiConfig.BaseUrl}factura/crear?codCliente={clienteData.CodigoCliente}&total={totalStr}&formaPago=EF&cantidad={cantidadTotal}";
            var responseFactura = await client.PostAsync(urlFactura, null);

            if (!responseFactura.IsSuccessStatusCode)
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