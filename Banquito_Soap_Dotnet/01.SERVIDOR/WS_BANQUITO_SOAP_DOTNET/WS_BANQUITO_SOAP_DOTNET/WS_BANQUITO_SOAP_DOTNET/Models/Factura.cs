using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_BANQUITO_SOAP_DOTNET.Models
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

    public class VerificarSujetoCreditoResult
    {
        public bool EsSujetoCredito { get; set; }
        public string Mensaje { get; set; }
    }

    public class CalcularMontoMaximoCreditoResult
    {
        public double MontoMaximoCredito { get; set; }
        public string Mensaje { get; set; }
    }

    public class ObtenerCodigoClienteResult
    {
        public int? CodigoCliente { get; set; }
        public string Mensaje { get; set; }
    }

    public class CrearFacturaResult
    {
        public string Mensaje { get; set; }
        public int IdFactura { get; set; }
        public decimal TotalFinal { get; set; }
        public int Descuento { get; set; }
    }


}