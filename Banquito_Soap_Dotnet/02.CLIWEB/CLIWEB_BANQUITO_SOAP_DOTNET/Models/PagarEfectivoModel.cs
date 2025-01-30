using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIWEB_BANQUITO_SOAP_DOTNET.Models
{
    public class PagarEfectivoModel
    {
        public string Cedula { get; set; }
        public decimal TotalOriginal { get; set; }
        public decimal TotalConDescuento { get; set; }
        public decimal DescuentoPorcentaje { get; set; } = 42m;
        public string Mensaje { get; set; }
    }
}