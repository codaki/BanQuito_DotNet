using CLIESC_EUREKABANK.Modelo;
using CLIESC_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    /// Lógica de interacción para pagarCreditoView.xaml
    /// </summary>
    public partial class pagarCreditoView : Window
    {
        private BanquitoServices _controller;
        private GenerarTablaView generarTablaView;
        private decimal _total;
        int _cantidad;

        public pagarCreditoView(decimal total)
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            _total = total;
            totalValue.Content = $"Total:{_total:C}";
        }

        public pagarCreditoView(decimal total, int cantidad)
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            _total = total;
            _cantidad = cantidad;
            totalValue.Content = $"Total:{_total:C}";
        }

        private async void pagarCredito_Click(object sender, RoutedEventArgs e)
        {
            string cedula = txtCedula.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                MessageBox.Show("Por favor, ingrese el número de cédula.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Call the method to verify credit eligibility
                 var responseSujeto = await _controller.VerificarSujetoCreditoService(cedula);
                bool isEligible = responseSujeto.EsSujetoCredito;

                var responseCliente = await _controller.ObtenerCodigoCliente(cedula);
                int? idCliente = responseCliente.CodigoCliente;

                var responseMonto = await _controller.CalcularMontoMaximoCreditoService(idCliente.Value);
                double montoMaximo = responseMonto.MontoMaximoCredito;

                // Display the result
                if (isEligible && ((decimal)montoMaximo >= _total))
                {
                    var response = MessageBox.Show("El cliente es sujeto a crédito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (response == MessageBoxResult.OK) 
                    {
                        generarTablaView = new GenerarTablaView(cedula,_total,_cantidad);
                        generarTablaView.Show();
                    }
                }
                else
                {
                    MessageBox.Show("El cliente no es sujeto a crédito.", "Información", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
