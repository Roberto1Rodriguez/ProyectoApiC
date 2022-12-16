using System;
using System.Collections.Generic;
using System.Text;

namespace ClienteMovil.Models
{
   public class Vuelos
    {
        public int Id { get; set; }
        public string CodigoVuelo { get; set; } = null!;
        public string Destino { get; set; } = null!;
        public DateTime HorarioSalida { get; set; }
        public string PuertaSalida { get; set; } = null!;
        public string Estado { get; set; } = "Programado";
    }
}
