using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devsu.Models
{
    public class Cliente : Persona
    {
        [Required]
        [MaxLength(50)]
        public string Contrasena { get; set; }

        [Required]
        public string Estado { get; set; }
    }
}