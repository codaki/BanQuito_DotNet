using CLICON_EUREKABANK.Modelo;
using CLICON_EUREKABANK.Servicios;
using System;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    public class PagarCreditoView
    {
        private readonly BanquitoServices _controller;
        private readonly decimal _total;
        int _cantidad;

        public PagarCreditoView(decimal total, int cantidad)
        {
            _controller = new BanquitoServices();
            _total = total;
            _cantidad = cantidad;
        }

        public async Task Run()
        {
            Console.Clear();
            Console.WriteLine("=== Pagar con Crédito ===");
            Console.WriteLine($"Total: {_total:C}");

            Console.Write("\nIngrese el número de cédula del cliente: ");
            string cedula = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                Console.WriteLine("Por favor, ingrese un número de cédula válido.");
                return;
            }

            try
            {
                // Verify if the client is eligible for credit
                var responseSujeto = await _controller.VerificarSujetoCreditoService(cedula);
                bool isEligible = responseSujeto.EsSujetoCredito;
                var responseCliente = await _controller.ObtenerCodigoCliente(cedula);
                int? idCliente = responseCliente.CodigoCliente;

                if (idCliente == null)
                {
                    Console.WriteLine("Cliente no encontrado. Verifique el número de cédula.");
                    return;
                }

                var resultMonto = await _controller.CalcularMontoMaximoCreditoService(idCliente.Value);
                double montoMaximo = resultMonto.MontoMaximoCredito;

                // Determine if the client qualifies for credit and has sufficient credit limit
                if (isEligible && ((decimal)montoMaximo >= _total))
                {
                    Console.WriteLine("\nEl cliente es sujeto a crédito.");

                    Console.Write("\n¿Desea generar una tabla de amortización? (s/n): ");
                    string choice = Console.ReadLine()?.ToLower();

                    if (choice == "s")
                    {
                        GenerarTablaView generarTablaView = new GenerarTablaView(cedula, _total,_cantidad);
                        await generarTablaView.Run();
                    }
                }
                else
                {
                    Console.WriteLine("\nEl cliente no es sujeto a crédito o no tiene suficiente límite.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
