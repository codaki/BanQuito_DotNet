using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIESC_EUREKABANK.Modelo
{
    public class VerificarCreditoResponse
    {
        public bool EsSujetoDCredito { get; set; }
        public string Mensaje { get; set; }
    }
}
