using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_BANQUITO_SOAP_DOTNET.Models
{
    public class Amortizacion
    {
        public int NumeroCuota { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal InteresPagado { get; set; }
        public decimal CapitalPagado { get; set; }
        public decimal Saldo { get; set; }
    }

}