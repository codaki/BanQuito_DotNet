using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIESC_EUREKABANK.Modelo
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public string NombreCliente { get; set; }
        public string Cedula { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string FormaPago { get; set; }
        public int Descuento { get; set; }
        public int Cantidad { get; set; }
    }

}
