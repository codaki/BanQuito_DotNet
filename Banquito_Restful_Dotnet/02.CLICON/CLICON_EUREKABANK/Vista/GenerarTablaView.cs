using CLICON_EUREKABANK.Servicios;
using System;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    public class GenerarTablaView
    {
        private readonly BanquitoServices _controller;
        private readonly string _cedulaCliente;
        private readonly decimal _totalCliente;
        int _cantidad;

        public GenerarTablaView(string cedula, decimal total, int cantidad)
        {
            _controller = new BanquitoServices();
            _cedulaCliente = cedula;
            _totalCliente = total;
            _cantidad = cantidad;
        }

        public async Task Run()
        {
            Console.Clear();
            Console.WriteLine("=== Generar Tabla de Amortización ===");
            Console.WriteLine($"Cliente: {_cedulaCliente}");
            Console.WriteLine($"Total del préstamo: {_totalCliente:C}");

            Console.Write("\nIngrese el número de cuotas (entre 3 y 18): ");
            if (!int.TryParse(Console.ReadLine(), out int numCuotas) || numCuotas < 3 || numCuotas > 18)
            {
                Console.WriteLine("Por favor, ingrese un número de cuotas válido entre 3 y 18.");
                return;
            }

            try
            {
                // Fetch the client ID
                int? idCliente = await _controller.ObtenerCodigoCliente(_cedulaCliente);

                if (idCliente == null)
                {
                    Console.WriteLine("No se pudo obtener el código del cliente. Verifique la cédula.");
                    return;
                }

                // Generate the amortization table
                string result = await _controller.CrearTablaAmortizacion(idCliente.Value, _totalCliente, numCuotas);
                var resultado = await _controller.CrearFactura(idCliente.Value, _totalCliente, "CR", _cantidad);

                Console.WriteLine(result);

                // Fetch and display the generated table
                await FetchAndDisplayTable(idCliente.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task FetchAndDisplayTable(int idCliente)
        {
            Console.WriteLine("\n=== Tabla de Amortización Generada ===");

            try
            {
                var tablaAmortizacion = await _controller.ObtenerTablaCliente(idCliente);

                if (tablaAmortizacion != null && tablaAmortizacion.Count > 0)
                {
                    Console.WriteLine("\n# Cuota\tValor Cuota\tInterés Pagado\tCapital Pagado\tSaldo");
                    Console.WriteLine("------------------------------------------------------------");

                    foreach (var cuota in tablaAmortizacion)
                    {
                        Console.WriteLine($"{cuota.NumeroCuota}\t{cuota.ValorCuota:C}\t{cuota.InteresPagado:C}\t{cuota.CapitalPagado:C}\t{cuota.Saldo:C}");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos para la tabla de amortización.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la tabla de amortización: {ex.Message}");
            }
        }
    }
}
