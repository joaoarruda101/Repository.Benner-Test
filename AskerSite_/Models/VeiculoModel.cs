using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Estacionamento.Models
{
    public class VeiculoModel
    {
        [Key]
        public int Id { get; set; }

        [RegularExpression(@"^[A-Z]{3}\d{4}$", ErrorMessage = "A placa deve estar no formato AAA9999")]
        public string Placa { get; set; }

        public DateTime Entrada { get; set; }

        public DateTime Saida { get; set; }

        public TimeSpan Duracao { get; set; }

        public int? TempoCobrado { get; set; }

        [DataType("Price")]
        [DefaultValue(0.00)]
        public double? PrecoEstatacionamento { get; set; }

        public double? ValorPagar { get; set; }

    }
}
