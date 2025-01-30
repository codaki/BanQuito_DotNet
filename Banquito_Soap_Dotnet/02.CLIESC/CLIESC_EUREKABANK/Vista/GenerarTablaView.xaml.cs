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
    /// Lógica de interacción para GenerarTablaView.xaml
    /// </summary>
    public partial class GenerarTablaView : Window
    {
        private BanquitoServices _controller;
        string cedulaCliente;
        decimal totalCliente;
        int _cantidad;

        public GenerarTablaView(string cedula,decimal total)
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            cedulaCliente = cedula;
            totalCliente = total;
        }

        public GenerarTablaView(string cedula, decimal total,int cantidad)
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            cedulaCliente = cedula;
            _cantidad = cantidad;
            totalCliente = total;
        }

        private async void GenerarTabla_Click(object sender, RoutedEventArgs e)
        {
            string cedula = cedulaCliente;

            if (string.IsNullOrEmpty(cedula))
            {
                MessageBox.Show("Por favor, ingrese la cédula del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Fetch the client ID using the cedula
                var response = await _controller.ObtenerCodigoCliente(cedula);
                int? idCliente = response.CodigoCliente;

                if (idCliente == null)
                {
                    MessageBox.Show("No se pudo obtener el código del cliente. Verifique la cédula.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Generate the amortization table
                decimal valorPrestamo = totalCliente; // Set your desired loan value
                int numCuotas = 1; // Set your desired number of installments
                int.TryParse(clienteCuotas.Text, out numCuotas);
                
                    if (numCuotas > 3 && numCuotas < 18 && _cantidad >0) 
                    {
                        string result = await _controller.CrearTablaAmortizacionService(idCliente.Value, valorPrestamo, numCuotas);
                    var resultado = await _controller.CrearFacturaService(idCliente.Value,valorPrestamo,"CR",_cantidad);
                        // Show the success message
                        MessageBox.Show(result, "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Fetch and display the generated table
                        await FetchAndDisplayTable(idCliente.Value);
                }
                else
                {
                    MessageBox.Show("Ingrese un numero entre 3 y 18", "Resultado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                    
                    
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task FetchAndDisplayTable(int idCliente)
        {
            try
            {
                var tablaAmortizacion = await _controller.ObtenerTablaAmortizacionService(idCliente);

                if (tablaAmortizacion != null && tablaAmortizacion.Count > 0)
                {
                    // Bind the data to the DataGrid
                    MovimientosDataGrid.ItemsSource = tablaAmortizacion;
                }
                else
                {
                    MessageBox.Show("No se encontraron datos para la tabla de amortización.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
