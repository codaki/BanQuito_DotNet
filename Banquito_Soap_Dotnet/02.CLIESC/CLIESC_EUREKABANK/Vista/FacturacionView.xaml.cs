using CLIESC_EUREKABANK.Modelo;
using CLIESC_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CLIESC_EUREKABANK.Vista
{
    /// <summary>
    /// Lógica de interacción para FacturacionView.xaml
    /// </summary>
    public partial class FacturacionView : Window
    {
        BanquitoServices _controller;
        private List<WSBANQUITO.Telefono> _telefonos;
        decimal total;
        PagarEfectivoView _pagination;
        pagarCreditoView creditoView;
        private List<WSBANQUITO.Telefono> _cartTelefonos = new List<WSBANQUITO.Telefono>();
        private DispatcherTimer _searchTimer;


        public FacturacionView()
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            total = 0;
            LoadTelefonos();
            // Initialize the debounce timer
            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // Adjust the delay as needed
            };
            _searchTimer.Tick += PerformSearch;

        }

        private async void LoadTelefonos()
        {
            try
            {
                // Fetch the list of phones
                _telefonos = await _controller.ObtenerTelefonosService();

                // Clear the container
                ClearTelefonosContainer();

                // Create and display each phone card
                foreach (var telefono in _telefonos)
                {
                    var tarjetaTelefono = CreateTarjetaTelefono(telefono);
                    TelefonosContainer.Children.Add(tarjetaTelefono);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al cargar los teléfonos: {ex.Message}");
            }
        }

        private void Buscador_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_telefonos == null || _telefonos.Count == 0)
                return;

            _searchTimer.Stop(); // Reset the timer
            _searchTimer.Start(); // Restart timer when user types
        }

        private async void PerformSearch(object sender, EventArgs e)
        {
            _searchTimer.Stop(); // Stop debounce timer

            string searchText = buscador.Text.ToLower();

            // Show loading indicator
            LoadingIndicator.Visibility = Visibility.Visible;

            await Task.Delay(100); // Small delay to ensure UI updates before filtering

            List<WSBANQUITO.Telefono> filteredTelefonos;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredTelefonos = _telefonos; // Show all if search is empty
            }
            else
            {
                // Perform filtering
                filteredTelefonos = _telefonos
                    .Where(t => t.NOMBRE != null && t.NOMBRE.ToLower().Contains(searchText))
                    .ToList();
            }

            // Clear and update UI
            ClearTelefonosContainer();
            foreach (var telefono in filteredTelefonos)
            {
                var tarjetaTelefono = CreateTarjetaTelefono(telefono);
                TelefonosContainer.Children.Add(tarjetaTelefono);
            }

            // Hide loading indicator after filtering
            LoadingIndicator.Visibility = Visibility.Collapsed;
        }


        private void ClearTelefonosContainer()
        {
            // Clear the container in case it's not empty
            TelefonosContainer.Children.Clear();
        }

        private Border CreateTarjetaTelefono(WSBANQUITO.Telefono telefono)
        {
            // Create a Border (TarjetaTelefono)
            Border tarjetaTelefono = new Border
            {
                BorderBrush = Brushes.White,
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(2),
                Background = Brushes.Black,
                Margin = new Thickness(10),
                Height = 160
            };

            // Create the Grid for the card content
            Grid grid = CreateTarjetaGrid(telefono);

            // Add the Grid to the Border
            tarjetaTelefono.Child = grid;

            return tarjetaTelefono;
        }

        private Grid CreateTarjetaGrid(WSBANQUITO.Telefono telefono)
        {
            // Create a Grid for the card content
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            // Add details panel
            var detailsPanel = CreateDetailsPanel(telefono);
            grid.Children.Add(detailsPanel);
            Grid.SetColumn(detailsPanel, 0);

            // Add button
            var addButton = CreateAddButton(telefono);
            grid.Children.Add(addButton);
            Grid.SetColumn(addButton, 1);

            return grid;
        }

        private StackPanel CreateDetailsPanel(WSBANQUITO.Telefono telefono)
        {
            // Create the StackPanel for image and details
            StackPanel detailsPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            // Add Image
            Image image = new Image
            {
                Source = LoadImageFromBase64(telefono.FOTO),
                Height = 80,
                Width = 80,
                Margin = new Thickness(10)
            };
            detailsPanel.Children.Add(image);

            // Add Name
            TextBlock nombreText = new TextBlock
            {
                Text = telefono.NOMBRE,
                Foreground = Brushes.White,
                Margin = new Thickness(10, 5, 10, 0)
            };
            detailsPanel.Children.Add(nombreText);

            // Add Price
            TextBlock precioText = new TextBlock
            {
                Text = $"Precio: ${telefono.PRECIO}",
                Foreground = Brushes.White,
                Margin = new Thickness(10, 0, 10, 5)
            };
            detailsPanel.Children.Add(precioText);

            return detailsPanel;
        }

        private Button CreateAddButton(WSBANQUITO.Telefono telefono)
        {
            // Add Button
            Button addButton = new Button
            {
                Width = 120,
                Height = 50,
                Background = Brushes.Green,
                Foreground = Brushes.White,
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new TextBlock { Text = "Agregar", Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center },
                new Image { Source = new BitmapImage(new Uri("pack://application:,,/Imagenes/carrito-de-compras.png")), Width = 20, Height = 20 }
            }
                },
                Style = (Style)FindResource("RoundedButton"),
                Margin = new Thickness(10)
            };

            // Add button click event
            addButton.Click += (s, e) => AddTelefonoToTotal(telefono);

            return addButton;
        }

        private void AddTelefonoToTotal(WSBANQUITO.Telefono telefono)
        {
            // Add the phone to the cart list
            _cartTelefonos.Add(telefono);

            // Update the total amount
            total += telefono.PRECIO;

            // Update the TextBlock with the current cart count
            CantidadCarrito.Text = $"({_cartTelefonos.Count.ToString()})" ;

            // Show a confirmation message
            MessageBox.Show($"Teléfono agregado: {telefono.NOMBRE} - Precio: ${telefono.PRECIO}\nTotal: ${total}",
                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Helper method to load an image from a Base64 string
        private ImageSource LoadImageFromBase64(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var stream = new System.IO.MemoryStream(imageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch
            {
                // Return a placeholder image in case of an error
                return new BitmapImage(new Uri("pack://application:,,/Imagenes/placeholder.png"));
            }
        }

        private void pagarCredito_Click(object sender, RoutedEventArgs e)
        {
            creditoView = new pagarCreditoView(total);
            creditoView.Show();
        }

        private void pagarEfectivo_Click(object sender, RoutedEventArgs e)
        {
            _pagination = new PagarEfectivoView(total);
            _pagination.Show();

        }
        private void VerCarrito_Click(object sender, RoutedEventArgs e)
        {
            var carritoView = new CarritoView(_cartTelefonos); // Pass the cart list
            carritoView.Show();
        }

        

    }
}
