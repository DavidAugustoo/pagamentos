using FCG.Domain.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCG.Application.DTOs
{
    public class PagamentoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        public int JogoId { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        public int FormaPagamentoId { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        public int Quantidade { get; set; }
    }
}
