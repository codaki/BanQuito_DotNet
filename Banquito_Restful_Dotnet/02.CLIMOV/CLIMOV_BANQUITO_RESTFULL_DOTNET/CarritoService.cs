using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET
{
    public static class CarritoService
    {
        public static ObservableCollection<Telefono> CarritoItems { get; } = new ObservableCollection<Telefono>();

        public static decimal Total => CarritoItems.Sum(t => t.PRECIO);
    }
}
