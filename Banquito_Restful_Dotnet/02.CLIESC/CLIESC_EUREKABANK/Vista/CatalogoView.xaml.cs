using CLIESC_EUREKABANK.Servicios;
using System.Windows;

namespace CLIESC_EUREKABANK.Vista
{
    /// <summary>
    /// Lógica de interacción para CatalogoView.xaml
    /// </summary>
    public partial class CatalogoView : Window
    {
        private BanquitoServices _controller;
        private string _base64Image = null;

        public CatalogoView()
        {
            InitializeComponent();
            _controller = new BanquitoServices();
        }

        private async void AgregarButton_Click(object sender, RoutedEventArgs e)
        {
            string nombre = NombreTelefonoTextBox.Text.Trim();
            string precioText = PrecioTextBox.Text.Trim();
            decimal precio;

            if (string.IsNullOrEmpty(nombre) || !decimal.TryParse(precioText, out precio) || string.IsNullOrEmpty(_base64Image))
            {
                MensajeLabel.Content = "Por favor complete todos los campos, incluyendo la imagen.";
                return;
            }

            try
            {
                bool result = await _controller.AgregarTelefonoService(nombre, precio, _base64Image);

                MensajeLabel.Content = result
                    ? "Teléfono agregado exitosamente."
                    : "Error al agregar el teléfono.";
            }
            catch (Exception ex)
            {
                MensajeLabel.Content = $"Error: {ex.Message}";
            }
        }

        private async void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            string idText = IdTelefonoTextBox.Text.Trim();
            int id;

            // Validate input
            if (!int.TryParse(idText, out id))
            {
                MensajeLabel.Content = "Por favor ingrese un ID válido.";
                return;
            }

            try
            {
                // Call the service to delete the phone
                bool result = await _controller.EliminarTelefonoService(id);

                // Update the label with success or error message
                MensajeLabel.Content = result
                    ? "Teléfono eliminado exitosamente."
                    : "Error al eliminar el teléfono.";
            }
            catch (Exception ex)
            {
                MensajeLabel.Content = $"Error: {ex.Message}";
            }
        }

        private async void ActualizarButton_Click(object sender, RoutedEventArgs e)
        {
            
            string codTelText = IdTelefonoTextBox.Text.Trim();
            string nombre = NombreTelefonoTextBox.Text.Trim();
            string precioText = PrecioTextBox.Text.Trim();

            int id, codTel;
            decimal precio;

            // Validate input
            if (!int.TryParse(codTelText, out codTel) || !decimal.TryParse(precioText, out precio) || string.IsNullOrEmpty(nombre))
            {
                MensajeLabel.Content = "Por favor ingrese valores válidos para ID, Código, Nombre y Precio.";
                return;
            }

            try
            {
                // Call the service to update the phone
                bool result = await _controller.ActualizarTelefonoService(codTel, nombre, precio,_base64Image);

                // Update the label with success or error message
                MensajeLabel.Content = result
                    ? "Teléfono actualizado exitosamente."
                    : "Error al actualizar el teléfono.";
            }
            catch (Exception ex)
            {
                MensajeLabel.Content = $"Error: {ex.Message}";
            }
        }

        private async void ActualizarTablaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the service to get the list of phones
                var telefonos = await _controller.ObtenerTelefonosService();

                // Bind the data to the DataGrid
                TelefonoDataGrid.ItemsSource = telefonos;
            }
            catch (Exception ex)
            {
                MensajeLabel.Content = $"Error al cargar los teléfonos: {ex.Message}";
            }
        }

        private void SeleccionarImagenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select an image
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Read the image file and convert it to Base64
                var filePath = openFileDialog.FileName;
                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                _base64Image = Convert.ToBase64String(imageBytes);

                // Update the UI to show the selected file path
                ImagenSeleccionadaLabel.Content = $"Imagen seleccionada: {System.IO.Path.GetFileName(filePath)}";
            }
        }

    }
}
