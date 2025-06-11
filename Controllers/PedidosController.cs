using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidoApi.Data;
using PedidoApi.Models;
using PedidoApi.Hubs;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.SignalR; // Adicione aqui

namespace PedidoApi.Controllers
{
    [Route("orders")]
    [ApiController]
    [Tags("Orders")] // <-- Adicione esta linha para mudar o nome da tag no Swagger
    public class PedidosController : ControllerBase
    {
        private readonly PedidoContext _context;
        private readonly IHubContext<PedidoHub> _hubContext;

        public PedidosController(PedidoContext context, IHubContext<PedidoHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return await _context.Pedidos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(Guid id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();
            return pedido;
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Notifica todos os clientes sobre a criação do pedido
            await _hubContext.Clients.All.SendAsync("PedidoAtualizado", pedido);

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(Guid id, Pedido pedido)
        {
            if (id != pedido.Id) return BadRequest();

            var pedidoExistente = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (pedidoExistente == null) return NotFound();

            bool statusMudou = pedidoExistente.Status != pedido.Status;

            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Notifica todos os clientes sobre a atualização do pedido
            await _hubContext.Clients.All.SendAsync("PedidoAtualizado", pedido);

            // Se quiser manter a notificação específica para mudança de status, pode deixar também:
            if (statusMudou)
            {
                await _hubContext.Clients.All.SendAsync("StatusPedidoAlterado", new { pedido.Id, NovoStatus = pedido.Status });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(Guid id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            // Notifica todos os clientes sobre a exclusão do pedido
            await _hubContext.Clients.All.SendAsync("PedidoAtualizado", id);

            return NoContent();
        }
    }
}
