using System.Net.Http;
using System.Text;
using System.Text.Json;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class CatalogoTelefonos : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<WSBANQUITO.Telefono> _telefonos;
    private string _base64Image; // Variable global para almacenar la imagen en Base64
    private WSBANQUITO.WSBanquitoSoap _soapClient;


    public CatalogoTelefonos()
    {
        InitializeComponent();
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
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

            // Crear la solicitud SOAP
            var requestBody = new WSBANQUITO.InsertarTelefonoRequestBody(EntryNombre.Text, decimal.Parse(EntryPrecio.Text), base64Image);
            var request = new WSBANQUITO.InsertarTelefonoRequest
            {
                Body = requestBody
            };

            // Llamar al servicio SOAP
            var response = await _soapClient.InsertarTelefonoAsync(request);

            if (response.Body.InsertarTelefonoResult.Equals("Teléfono insertado exitosamente."))
            {
                await DisplayAlert("Éxito", "Teléfono insertado exitosamente", "OK");

                // Limpiar entradas después de la inserción exitosa
                EntryId.Text = string.Empty;
                EntryNombre.Text = string.Empty;
                EntryPrecio.Text = string.Empty;
                ImagePreview.Source = null;
            }
            else
            {
                await DisplayAlert("Error", $"No se pudo insertar el teléfono: {response.Body.InsertarTelefonoResult}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }






    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar entrada de ID
            if (string.IsNullOrWhiteSpace(EntryId.Text) || !int.TryParse(EntryId.Text, out int telefonoId))
            {
                await DisplayAlert("Error", "Por favor ingrese un ID válido", "OK");
                return;
            }

            // Crear la solicitud SOAP
            var requestBody = new WSBANQUITO.EliminarTelefonoRequestBody(telefonoId);
            var request = new WSBANQUITO.EliminarTelefonoRequest
            {
                Body = requestBody
            };

            // Llamar al servicio SOAP
            var response = await _soapClient.EliminarTelefonoAsync(request);

            // Procesar la respuesta correctamente
            if (response.Body.EliminarTelefonoResult.Equals("Teléfono eliminado exitosamente."))
            {
                await DisplayAlert("Éxito", "Teléfono eliminado exitosamente", "OK");

                // Limpiar entradas después de la eliminación exitosa
                EntryId.Text = string.Empty;
                EntryNombre.Text = string.Empty;
                EntryPrecio.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Error", $"No se pudo eliminar el teléfono: {response.Body.EliminarTelefonoResult}", "OK");
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
            var telefonoUpdateRequest = new WSBANQUITO.ActualizarTelefonoRequestBody
            {
                codTel = int.Parse(EntryId.Text),
                nombre = EntryNombre.Text,
                precio = decimal.Parse(EntryPrecio.Text),
                foto = base64Image  
            };

            var request = new WSBANQUITO.ActualizarTelefonoRequest
            {
                Body = telefonoUpdateRequest
            };

            // Llamar al servicio SOAP
            var response = await _soapClient.ActualizarTelefonoAsync(request);

            // Procesar la respuesta
            if (response.Body.ActualizarTelefonoResult.Equals("Teléfono actualizado exitosamente."))
            {
                // Limpiar entradas después de la inserción exitosa
                EntryId.Text = string.Empty;
                EntryNombre.Text = string.Empty;
                EntryPrecio.Text = string.Empty;
                ImagePreview.Source = null;
                await DisplayAlert("Éxito", "Teléfono actualizado exitosamente", "OK");
            }
            else
            {
                await DisplayAlert("Error", $"No se pudo actualizar el teléfono: {response.Body.ActualizarTelefonoResult}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }



    private async void OnVerTelefonosClicked(object sender, EventArgs e)
    {
        try
        {
            // Crear la solicitud SOAP
            var request = new WSBANQUITO.ObtenerTodosTelefonosRequest();

            // Llamar al servicio SOAP
            var response = await _soapClient.ObtenerTodosTelefonosAsync(request);

            // Verifica si la lista tiene elementos antes de procesarla
            if (response.Body.ObtenerTodosTelefonosResult != null && response.Body.ObtenerTodosTelefonosResult.Length > 0)
            {
                // Procesar la respuesta como una lista
                _telefonos = response.Body.ObtenerTodosTelefonosResult.ToList();

                // Asignar la lista a la CollectionView
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




