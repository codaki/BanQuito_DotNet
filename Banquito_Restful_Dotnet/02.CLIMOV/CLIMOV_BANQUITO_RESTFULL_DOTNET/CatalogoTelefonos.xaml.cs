using System.Net.Http;
using System.Text;
using System.Text.Json;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class CatalogoTelefonos : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<Telefono> _telefonos;
    private string _base64Image; // Variable global para almacenar la imagen en Base64

    public CatalogoTelefonos()
    {
        InitializeComponent();
        BindingContext = this;
        _httpClient = new HttpClient();

        // Wire up button click events
        BtnAgregar.Clicked += OnAgregarClicked;
        BtnEliminar.Clicked += OnEliminarClicked;
        BtnActualizar.Clicked += OnActualizarClicked;
        BtnVerTelefonos.Clicked += OnVerTelefonosClicked;
        BtnSeleccionarFoto.Clicked += OnSeleccionarFotoClicked;

    }
    // Existing orientation code
    DeviceOrientationService service;
    protected override void OnAppearing()
    {
        service = new DeviceOrientationService();
        service.SeOrientation(DisplayOrientation.Landscape);
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        service.SeOrientation(DisplayOrientation.Portrait);
    }

    void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        Console.WriteLine($"ScrollX: {e.ScrollX}, ScrollY: {e.ScrollY}");
    }

    private async void OnSeleccionarFotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Seleccione una imagen",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Asigna la imagen al ImagePreview
                ImagePreview.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));

                // Convierte la imagen a Base64 y almacénala
                _base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
        }
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar entradas
            if (string.IsNullOrWhiteSpace(EntryNombre.Text) || string.IsNullOrWhiteSpace(EntryPrecio.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese nombre y precio", "OK");
                return;
            }

            if (ImagePreview.Source == null)
            {
                await DisplayAlert("Error", "Por favor seleccione una foto", "OK");
                return;
            }

            // Convertir imagen a Base64
            Stream stream = await ((StreamImageSource)ImagePreview.Source).Stream(CancellationToken.None);
            if (stream == null)
            {
                await DisplayAlert("Error", "No se pudo procesar la imagen seleccionada", "OK");
                return;
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }
            string base64Image = Convert.ToBase64String(imageBytes);

            // Crear un objeto de teléfono para enviar como JSON
            var telefono = new
            {
                Nombre = EntryNombre.Text,
                Precio = decimal.Parse(EntryPrecio.Text),
                Foto = base64Image
            };

            // Serializar el objeto a JSON
            var jsonContent = new StringContent(JsonSerializer.Serialize(telefono), Encoding.UTF8, "application/json");

            // URL de la API
            string url = $"{ApiConfig.BaseUrl}telefono/insertar";

            // Enviar solicitud POST
            var response = await _httpClient.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Éxito", result, "OK");

                // Limpiar entradas después de la inserción exitosa
                EntryId.Text = string.Empty;
                EntryNombre.Text = string.Empty;
                EntryPrecio.Text = string.Empty;
                ImagePreview.Source = null;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"No se pudo insertar el teléfono: {errorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }




    // Eliminar (Delete) Telefono
    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate ID input
            if (string.IsNullOrWhiteSpace(EntryId.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese un ID", "OK");
                return;
            }

            string url = $"{ApiConfig.BaseUrl}telefono/eliminar/{EntryId.Text}";

            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Éxito", result, "OK");

                // Clear inputs after successful deletion
                EntryId.Text = string.Empty;
                EntryNombre.Text = string.Empty;
                EntryPrecio.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el teléfono", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar entradas
            if (string.IsNullOrWhiteSpace(EntryId.Text) ||
                string.IsNullOrWhiteSpace(EntryNombre.Text) ||
                string.IsNullOrWhiteSpace(EntryPrecio.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese todos los datos", "OK");
                return;
            }

            // Convertir imagen a base64 (si se seleccionó una foto)
            string base64Image = null;
            if (ImagePreview.Source != null)
            {
                Stream stream = await ((StreamImageSource)ImagePreview.Source).Stream(CancellationToken.None);
                if (stream == null)
                {
                    await DisplayAlert("Error", "No se pudo procesar la imagen seleccionada", "OK");
                    return;
                }

                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                base64Image = Convert.ToBase64String(imageBytes);
            }

            // Crear el objeto con los datos
            var telefonoUpdateRequest = new
            {
                CodTel = int.Parse(EntryId.Text),
                Nombre = EntryNombre.Text,
                Precio = decimal.Parse(EntryPrecio.Text),
                Foto = base64Image  // La imagen en base64, si está presente
            };

            // Convertir el objeto a JSON
            var json = JsonSerializer.Serialize(telefonoUpdateRequest);

            // Crear la solicitud PUT
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string url = $"{ApiConfig.BaseUrl}telefono/actualizar";

            // Enviar solicitud PUT
            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Éxito", result, "OK");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                await DisplayAlert("Error", $"No se pudo insertar el teléfono: {errorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }


    // Ver (Get) Telefonos
    private async void OnVerTelefonosClicked(object sender, EventArgs e)
    {
        try
        {
            string url = $"{ApiConfig.BaseUrl}telefono/obtener";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _telefonos = JsonSerializer.Deserialize<List<Telefono>>(json);

                // Bind the collection to the CollectionView
                TelefonosCollection.ItemsSource = _telefonos;
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron obtener los teléfonos", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }
}

// Model class for Telefono
public class Telefono
{
    public int COD_TEL { get; set; }
    public string NOMBRE { get; set; }
    public decimal PRECIO { get; set; }
    public string FOTO { get; set; }

    public ImageSource FotoImage
    {
        get
        {
            if (!string.IsNullOrEmpty(FOTO))
            {
                return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(FOTO)));
            }
            return null;
        }
    }


}