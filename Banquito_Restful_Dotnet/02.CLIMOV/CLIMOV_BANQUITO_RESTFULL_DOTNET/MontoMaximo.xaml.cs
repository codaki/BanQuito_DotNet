using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET
{
    public partial class MontoMaximo : ContentPage
    {

        public MontoMaximo()
        {
            InitializeComponent();
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
                // Llamar a la API para obtener el código del cliente
                var codigoCliente = await ObtenerCodigoCliente(cedula);

                if (codigoCliente == null)
                {
                    LabelResultado.Text = "No se pudo obtener el código del cliente.";
                    return;
                }

                // Llamar a la API para obtener el monto máximo de crédito
                await ObtenerMontoMaximoCredito(codigoCliente.Value); // Ahora no necesitas asignar a una variable

            }
            catch (Exception ex)
            {
                LabelResultado.Text = $"Error: {ex.Message}";
            }
        }


        // Método para obtener el código de cliente usando la cédula
        private async Task<int?> ObtenerCodigoCliente(string cedula)
        {
            using var client = new HttpClient();
            var url = $"{ApiConfig.BaseUrl}cliente/codigo/{cedula}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ApiResponse>(content);

                if (data?.CodigoCliente.HasValue == true)
                {
                    return data.CodigoCliente;
                }
            }

            return null;
        }

        // Método para obtener el monto máximo de crédito usando el código de cliente
        private async Task ObtenerMontoMaximoCredito(int codigoCliente)
        {
            using var client = new HttpClient();
            var url = $"{ApiConfig.BaseUrl}credito/monto-maximo/{codigoCliente}";

            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ApiMontoMaximoResponse>(content);

                    if (data?.MontoMaximoCredito.HasValue == true)
                    {
                        // Mostrar el monto máximo y mensaje en una ventana emergente (DisplayAlert)
                        await DisplayAlert("Resultado",
                            $"Monto máximo de crédito: {data.MontoMaximoCredito}\nMensaje: {data.Mensaje}",
                            "OK");
                    }
                    else
                    {
                        // Si no se obtuvo el monto máximo de crédito
                        await DisplayAlert("Error", "No se pudo obtener el monto máximo de crédito.", "OK");
                    }
                }
                else
                {
                    // Si la respuesta no fue exitosa
                    await DisplayAlert("Error", "Hubo un problema al contactar la API.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones en caso de error de red o conexión
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
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
