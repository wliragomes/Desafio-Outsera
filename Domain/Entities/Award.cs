namespace Domain.Entities
{
    public class Award
    {
        public int Id { get; set; } // Chave primária gerada automaticamente
        public int Year { get; set; } // Ano da indicação
        public string? Title { get; set; } // Título do filme
        public string? Studios { get; set; } // Estúdios responsáveis
        public string? Producers { get; set; } // Produtores responsáveis
        public bool IsWinner { get; set; } // Indica se foi vencedor
    }
}
