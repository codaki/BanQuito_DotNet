using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private WSBANQUITO.WSBanquitoSoap _soapClient;


        public MainPage()
        {
            InitializeComponent();
            _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);

            _httpClient = new HttpClient();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingresa usuario y contraseña.", "OK");
                return;
            }

            try
            {

                // Crear la solicitud SOAP
                var requestBody = new WSBANQUITO.ValidarCredencialesRequestBody(username,password);
                var request = new WSBANQUITO.ValidarCredencialesRequest
                {
                    Body = requestBody
                };

                // Llamar al servicio SOAP ValidarCredencialesAsync
                var response = await _soapClient.ValidarCredencialesAsync(request);

                // Verificar si la respuesta es válida
                if (response.Body.ValidarCredencialesResult)
                {
                    await Navigation.PushAsync(new Menu());
                }
                else
                {
                    await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

    }
}