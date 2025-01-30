using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
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
                // Construir la URL del servicio RESTful con parámetros de consulta
                string url = $"{ApiConfig.BaseUrl}telefono/ValidarCredenciales?usuario={username}&contrasena={password}";

                // Realizar la solicitud HTTP POST
                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer la respuesta como un valor booleano
                    string responseBody = await response.Content.ReadAsStringAsync();
                    bool isValid = bool.Parse(responseBody);

                    if (isValid)
                    {
                        await Navigation.PushAsync(new Menu());
                    }
                    else
                    {
                        await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "OK");
                    }
                }
                else
                {
                    // Obtener más detalles del error
                    string errorBody = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error",
                        $"Error en la solicitud: {response.StatusCode}\n{errorBody}",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
    }
}