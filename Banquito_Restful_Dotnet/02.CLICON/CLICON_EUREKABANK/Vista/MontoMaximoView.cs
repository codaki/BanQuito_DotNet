using CLICON_EUREKABANK.Servicios;
using System;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    internal class MontoMaximoView
    {
        private readonly BanquitoServices _controller;

        public MontoMaximoView()
        {
            _controller = new BanquitoServices();
        }

        public async Task Run()
        {
            Console.Clear();
            Console.WriteLine("=== Monto Máximo ===");

            Console.Write("Ingrese el número de cédula del cliente: ");
            string cedula = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                Console.WriteLine("Por favor, ingrese un número de cédula válido.");
                return;
            }

            try
            {
                // Fetch client ID using the cedula
                int? idCliente = await _controller.ObtenerCodigoCliente(cedula);

                if (idCliente == null)
                {
                    Console.WriteLine("No se pudo obtener el código del cliente. Verifique la cédula.");
                    return;
                }

                // Fetch the maximum credit amount
                var montoMaximoResponse = await _controller.ObtenerMontoMaximo(idCliente.Value);

                if (montoMaximoResponse != null)
                {
                    Console.WriteLine($"\n{montoMaximoResponse.Mensaje}");
                    Console.WriteLine($"Monto Máximo Crédito: {montoMaximoResponse.MontoMaximoCredito:C}");
                }
                else
                {
                    Console.WriteLine("No se pudo obtener el monto máximo del cliente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
