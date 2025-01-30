using CLICON_EUREKABANK.Servicios;
using System;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    public class PagarEfectivoView
    {
        private readonly BanquitoServices _controller;
        private readonly decimal _total;
        int _cantidad;

        public PagarEfectivoView(decimal total,int cantidad)
        {
            _controller = new BanquitoServices();
            _total = total;
            _cantidad = cantidad;
        }

        public async Task Run()
        {
            Console.Clear();
            Console.WriteLine("=== Pagar Efectivo ===");

            Console.WriteLine($"Total: {_total:C}");

            // Calculate discount
            decimal descuento = _total * 0.42m;
            decimal totalConDescuento = _total - descuento;

            Console.WriteLine($"Descuento del 42%: {descuento:C}");
            Console.WriteLine($"Total con descuento: {totalConDescuento:C}");

            Console.Write("\nIngrese la cédula del cliente: ");
            string cedula = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                Console.WriteLine("Por favor, ingrese una cédula válida.");
                return;
            }

            try
            {
                // Get client ID from cédula
                var resultCodigo = await _controller.ObtenerCodigoCliente(cedula);
                int? codCliente = resultCodigo.CodigoCliente;

                if (codCliente == null)
                {
                    Console.WriteLine("No se pudo obtener el código del cliente. Verifique la cédula.");
                    return;
                }

                // Create the factura
                var responseFactura = await _controller.CrearFacturaService(codCliente.Value, _total, "EF",_cantidad);
                string success = responseFactura.Mensaje;

                if (success == "Factura creada exitosamente")
                {
                    Console.WriteLine("Factura creada exitosamente.");
                }
                else
                {
                    Console.WriteLine("Error al crear la factura. Intente de nuevo.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
