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

namespace CLIESC_EUREKABANK.Vista
{
    /// <summary>
    /// Lógica de interacción para VerFacturasView.xaml
    /// </summary>
    public partial class VerFacturasView : Window
    {
        private BanquitoServices _controller;
        public VerFacturasView()
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            LoadFacturas();
        }

        private async void LoadFacturas()
        {
            try
            {
                var facturas = await _controller.ObtenerFacturas();

                foreach (var factura in facturas)
                {
                    Border border = new Border
                    {
                        Background = System.Windows.Media.Brushes.Black,
                        Padding = new Thickness(15),
                        CornerRadius = new CornerRadius(5),
                        BorderBrush = System.Windows.Media.Brushes.White,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(10),
                        Width = 400 // Adjusted width to match image
                    };

                    StackPanel mainStack = new StackPanel { Orientation = Orientation.Vertical };

                    // Title: FACTURA N° 75
                    TextBlock title = new TextBlock
                    {
                        Text = $"FACTURA N° {factura.IdFactura}",
                        Foreground = System.Windows.Media.Brushes.White,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    mainStack.Children.Add(title);

                    // Details stack
                    StackPanel detailsStack = new StackPanel { Orientation = Orientation.Vertical };

                    detailsStack.Children.Add(CreateLabelValue("Cliente:", factura.NombreCliente));
                    detailsStack.Children.Add(CreateLabelValue("Cédula:", factura.Cedula));
                    detailsStack.Children.Add(CreateLabelValue("Dirección:", "Sangolqui"));
                    detailsStack.Children.Add(CreateLabelValue("Cantidad:", factura.Cantidad.ToString()));

                    // Phone List
                    detailsStack.Children.Add(new TextBlock { Text = "Productos:", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold });

                    List<string> phones = new List<string> { "Samsung", "Infinix", "Oppo", "Samsung" };
                    var selectedPhones = phones.Take(factura.Cantidad).ToList();

                    foreach (var phone in selectedPhones)
                    {
                        detailsStack.Children.Add(new TextBlock
                        {
                            Text = $"- {phone}",
                            Foreground = System.Windows.Media.Brushes.White,
                            Margin = new Thickness(10, 0, 0, 0) // Indented for list effect
                        });
                    }

                    detailsStack.Children.Add(CreateLabelValue("Fecha:", factura.Fecha.ToString("dd/MM/yyyy")));

                    // Forma de Pago (aligned right)
                    StackPanel paymentStack = new StackPanel { Orientation = Orientation.Horizontal };
                    paymentStack.Children.Add(new TextBlock
                    {
                        Text = "Forma de Pago:",
                        Foreground = System.Windows.Media.Brushes.White,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 5, 5, 0)
                    });
                    paymentStack.Children.Add(new TextBlock
                    {
                        Text = factura.FormaPago,
                        Foreground = System.Windows.Media.Brushes.White,
                        FontWeight = FontWeights.Normal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(5, 5, 0, 0)
                    });
                    detailsStack.Children.Add(paymentStack);

                    // Total a Pagar
                    StackPanel totalStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };

                    totalStack.Children.Add(new TextBlock
                    {
                        Text = "Total a Pagar:",
                        Foreground = System.Windows.Media.Brushes.White,
                        FontWeight = FontWeights.Bold
                    });

                    totalStack.Children.Add(new TextBlock
                    {
                        Text = $"${factura.Total:0.00}",
                        Foreground = System.Windows.Media.Brushes.Green,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(5, 0, 0, 0)
                    });

                    detailsStack.Children.Add(totalStack);

                    mainStack.Children.Add(detailsStack);
                    border.Child = mainStack;
                    FacturasContainer.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private StackPanel CreateLabelValue(string label, string value)
        {
            StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };
            stack.Children.Add(new TextBlock
            {
                Text = label,
                Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 5, 0)
            });
            stack.Children.Add(new TextBlock
            {
                Text = value,
                Foreground = System.Windows.Media.Brushes.White
            });

            return stack;
        }
    }
}
