namespace PedidoApi.Models
{
    public enum StatusPedido
    {
        Pendente,
        Processando,
        Finalizado
    }

    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Cliente { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Produto { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    }
}
