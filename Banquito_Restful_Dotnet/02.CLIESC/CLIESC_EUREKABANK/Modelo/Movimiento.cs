using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIESC_EUREKABANK.Modelo
{
    public class Movimiento
    {
        public string chr_cuencodigo { get; set; }
        public int int_movinumero { get; set; }
        public DateTime dtt_movifecha { get; set; }
        public string vch_tipodescripcion { get; set; }
        public decimal dec_moviimporte { get; set; }
    }

}
