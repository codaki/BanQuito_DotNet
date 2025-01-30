using CLIESC_EUREKABANK.Modelo;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CLIESC_EUREKABANK.Vista
{
    public partial class CarritoView : Window
    {
        private List<WSBANQUITO.Telefono> _cartTelefonos;
        pagarCreditoView creditoView;
        PagarEfectivoView _pagination;
        private decimal _total = 0; // Holds the total price of the items in the cart
        int _cantidad;


        public CarritoView(List<WSBANQUITO.Telefono> cartTelefonos)
        {
            InitializeComponent();
            _cartTelefonos = cartTelefonos;
            LoadTelefonos();
            UpdateTotal();
        }

        private void LoadTelefonos()
        {
            // Clear the container
            TelefonosContainer.Children.Clear();

            // Create and display each phone card
            foreach (var telefono in _cartTelefonos)
            {
                var tarjetaTelefono = CreateTarjetaTelefono(telefono);
                TelefonosContainer.Children.Add(tarjetaTelefono);
            }
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
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            // Add details panel
            var detailsPanel = CreateDetailsPanel(telefono);
            grid.Children.Add(detailsPanel);
            Grid.SetColumn(detailsPanel, 0);

            // Add delete button
            var deleteButton = CreateDeleteButton(telefono);
            grid.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);

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

        private Button CreateDeleteButton(WSBANQUITO.Telefono telefono)
        {
            // Create the delete button
            Button deleteButton = new Button
            {
                Width = 120,
                Height = 50,
                Background = Brushes.Red,
                Foreground = Brushes.White,
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new TextBlock { Text = "Borrar", Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center },
                        new Image { Source = new BitmapImage(new Uri("pack://application:,,/Imagenes/cubo-de-basura.png")), Width = 25, Height = 25 }
                    }
                },
                Style = (Style)FindResource("RoundedButton"),
                Margin = new Thickness(10)
            };

            // Add click event to remove the phone from the cart
            deleteButton.Click += (s, e) =>
            {
                var result = MessageBox.Show($"¿Seguro que deseas borrar {telefono.NOMBRE} del carrito?",
                                            "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _cartTelefonos.Remove(telefono);
                    LoadTelefonos(); // Refresh the view
                    UpdateTotal();
                }
                
            };

            return deleteButton;
        }

        // Helper method to load an image from a Base64 string
        private ImageSource LoadImageFromBase64(string base64String)
        {
            try
            {
                byte[] imageBytes = System.Convert.FromBase64String(base64String);
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
                return new BitmapImage(new Uri("pack://application:,,,/Imagenes/placeholder.png"));
            }
        }

        private void UpdateTotal()
        {
            // Calculate the total price
            _total = 0;
            _cantidad =  0;
            foreach (var telefono in _cartTelefonos)
            {
                _total += telefono.PRECIO;
                _cantidad++;
            }

            // Update the TotalLabel with the calculated total
            TotalLabel.Text = $"Total: ${_total}";
        }


        private void pagarCredito_Click(object sender, RoutedEventArgs e)
        {
            if (_total > 0)
            {
                creditoView = new pagarCreditoView(_total,_cantidad);
                creditoView.Show();
            }
            else
            {
                MessageBox.Show("El carrito está vacío. No puedes realizar un pago.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void pagarEfectivo_Click(object sender, RoutedEventArgs e)
        {
            if (_total > 0)
            {
                _pagination = new PagarEfectivoView(_total,_cantidad);
                _pagination.Show();
            }
            else
            {
                MessageBox.Show("El carrito está vacío. No puedes realizar un pago.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
