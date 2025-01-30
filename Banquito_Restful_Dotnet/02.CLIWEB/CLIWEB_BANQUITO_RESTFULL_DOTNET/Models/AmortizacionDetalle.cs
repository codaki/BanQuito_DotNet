using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIWEB_BANQUITO_RESTFULL_DOTNET.Models
{
    public class AmortizacionDetalle
    {
        public int NumeroCuota { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal InteresPagado { get; set; }
        public decimal CapitalPagado { get; set; }
        public decimal Saldo { get; set; }
    }
}