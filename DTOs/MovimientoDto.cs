using System;
using System.ComponentModel.DataAnnotations;

namespace devsu.DTOs
{
    public class MovimientoDto
    {
        public int MovimientoId { get; set; }
        
        public DateTime Fecha { get; set; }
        
        [Required]
        public string TipoMovimiento { get; set; }
        
        [Required]
        public decimal Valor { get; set; }
        
        public decimal Saldo { get; set; }
        
        [Required]
        public int CuentaId { get; set; }
        
        public string NumeroCuenta { get; set; }
    }
}