using CLICON_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    internal class TablaView
    {
        private readonly BanquitoServices _controller;

        public TablaView()
        {
            _controller = new BanquitoServices();
        }

        public async Task Run()
        {
            Console.Clear();
            Console.WriteLine("=== Tabla de Amortización ===");

            Console.Write("\nIngrese la cédula del cliente: ");
            string cedula = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                Console.WriteLine("Por favor, ingrese un número de cédula válido.");
                return;
            }

            try
            {
                // Get client ID using the cedula
                var resultCliente = await _controller.ObtenerCodigoCliente(cedula);
                int? idCliente = resultCliente.CodigoCliente;

                if (idCliente == null)
                {
                    Console.WriteLine("No se pudo obtener el código del cliente. Verifique la cédula.");
                    return;
                }

                // Fetch the amortization table
                var tablaAmortizacion = await _controller.ObtenerTablaAmortizacionService(idCliente.Value);

                if (tablaAmortizacion != null && tablaAmortizacion.Count > 0)
                {
                    Console.WriteLine("\n# Cuota\t\tValor Cuota\tInterés Pagado\tCapital Pagado\tSaldo");
                    Console.WriteLine("------------------------------------------------------------");

                    foreach (var cuota in tablaAmortizacion)
                    {
                        Console.WriteLine($"\t{cuota.NumeroCuota}\t  {cuota.ValorCuota:C}\t  {cuota.InteresPagado:C}\t\t {cuota.CapitalPagado:C}\t\t{cuota.Saldo:C}");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos para la tabla de amortización.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
