using System.ComponentModel.DataAnnotations;

namespace devsu.DTOs
{
    public class CuentaDto
    {
        public int CuentaId { get; set; }
        
        [Required]
        public string NumeroCuenta { get; set; }
        
        [Required]
        public string TipoCuenta { get; set; }
        
        [Required]
        public decimal SaldoInicial { get; set; }
        
        public bool Estado { get; set; } = true;
        
        [Required]
        public int ClienteId { get; set; }
        
        public string ClienteNombre { get; set; }
    }
    
    // DTO para operaciones PATCH - permite valores nulos
    public class CuentaPatchDto
    {
        public string? NumeroCuenta { get; set; }
        public string? TipoCuenta { get; set; }
        public decimal? SaldoInicial { get; set; }
        public bool? Estado { get; set; }
        public int? ClienteId { get; set; }
    }
}