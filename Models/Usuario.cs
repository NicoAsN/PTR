using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTR.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string primer_nom { get; set; }
        public string segundo_nom { get; set; }
        public string primer_ape { get; set; }
        public string segundo_ape { get; set; }
        public int telefono { get; set; }
        public int rut { get; set; }
        public string DV { get; set; }
        public string correo { get; set; }
        public string contrasenia { get; set; }
        public string confirmar_contrasenia { get; set; }
    }
}