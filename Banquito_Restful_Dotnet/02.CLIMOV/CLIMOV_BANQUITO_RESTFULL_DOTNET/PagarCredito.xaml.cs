using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Text.Json;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class PagarCredito : ContentPage
{
    private string total;
    int cantidadTotal;

    public PagarCredito(string total, int cantidad)
    {
        InitializeComponent();
        this.total = total;
        this.cantidadTotal = cantidad;
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

            using var client = new HttpClient();

            // Verificar si es sujeto a cr�dito
            string urlVerificarCredito = $"{ApiConfig.BaseUrl}credito/verificar/{EntryCedula.Text}";
            var responseVerificar = await client.GetAsync(urlVerificarCredito);

            if (!responseVerificar.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "No se pudo verificar el cr�dito.", "OK");
                return;
            }

            var verificarJson = await responseVerificar.Content.ReadAsStringAsync();
            var verificarData = JsonSerializer.Deserialize<CreditoResponse>(verificarJson);

            if (!verificarData.EsSujetoDCredito)
            {
                await DisplayAlert("No sujeto a cr�dito", verificarData.Mensaje, "OK");
                return;
            }

            // Obtener c�digo del cliente
            string urlCliente = $"{ApiConfig.BaseUrl}cliente/codigo/{EntryCedula.Text}";
            var responseCliente = await client.GetAsync(urlCliente);

            if (!responseCliente.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "No se pudo obtener el c�digo del cliente.", "OK");
                return;
            }

            var clienteJson = await responseCliente.Content.ReadAsStringAsync();
            var clienteData = JsonSerializer.Deserialize<ClienteResponse>(clienteJson);

            // Obtener monto m�ximo de cr�dito
            string urlMontoMaximo = $"{ApiConfig.BaseUrl}credito/monto-maximo/{clienteData.CodigoCliente}";
            var responseMonto = await client.GetAsync(urlMontoMaximo);

            if (!responseMonto.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "No se pudo obtener el monto m�ximo de cr�dito.", "OK");
                return;
            }

            var montoJson = await responseMonto.Content.ReadAsStringAsync();
            var montoData = JsonSerializer.Deserialize<MontoMaximoResponse>(montoJson);

            if (double.TryParse(total, out double totalDouble) && totalDouble <= montoData.MontoMaximoCredito)
            {
                await DisplayAlert("�xito", "El cliente puede realizar el cr�dito.", "OK");
                // Redirigir a la p�gina GenerarTabla
                await Navigation.PushAsync(new GenerarTabla(clienteData.CodigoCliente, total,cantidadTotal));
            }
            else if (montoData.Mensaje == "Monto m�ximo de cr�dito calculado exitosamente.")
            {
                await DisplayAlert("Cr�dito no aprobado", "El monto de la factura supera el monto m�ximo aprobado.", "OK");
                
            }
            else
            {
                await DisplayAlert("Cr�dito no aprobado", montoData.Mensaje, "OK");
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
