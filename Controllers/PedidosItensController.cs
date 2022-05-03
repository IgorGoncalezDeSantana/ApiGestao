using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIGestao.Context;
using APIGestao.Models;
using APIGestao.Exceptions;

namespace APIGestao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosItensController : ControllerBase
    {
        private readonly MyContext _context;

        public PedidosItensController(MyContext context)
        {
            _context = context;
        }

        // GET: api/PedidosItens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoItem>>> GetPedidosItems([FromHeader] Guid AccessToken, [FromHeader] int idPedido)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            return await _context.PedidosItems.AsNoTracking().Where(x => x.IDEmpresa == emp.Id && x.IDPedido == idPedido).ToListAsync();
        }

        // GET: api/PedidosItens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoItem>> GetPedidoItem(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var pedidoItem = await _context.PedidosItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);

            if (pedidoItem == null || pedidoItem == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido item não encontrado!");
                return BadRequest(result);
            }

            return pedidoItem;
        }

        // PUT: api/PedidosItens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoItem(long id, [FromBody]PedidoItem pedidoItem, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (id != pedidoItem.Id)
            {
                return BadRequest();
            }

            var pitem = await _context.PedidosItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (pitem == null || pitem == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido Item não encontrado!");
                return BadRequest(result);
            }

            pedidoItem.IDEmpresa = emp.Id;

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedidoItem.IDPedido && x.IDEmpresa == emp.Id);
            if (pedido == null || pedido == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            if (pedido.Status != "Pendente")
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível alterar um item de um pedido com o status diferente de pendente!");
                return BadRequest(result);
            }

            var prod = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedidoItem.IDProduto && x.IDEmpresa == emp.Id);
            if (prod == null || prod == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Produto não encontrado!");
                return BadRequest(result);
            }

            var item = await _context.PedidosItems.AsNoTracking().FirstOrDefaultAsync(
                x => x.IDPedido == pedidoItem.IDPedido &&
                x.IDProduto == pedidoItem.IDProduto &&
                x.Id != pedidoItem.Id);
            if (item != null && item != default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Já existe um item neste pedido que está usando este produto!");
                return BadRequest(result);
            }


            try
            {
                pedidoItem.Validate(Enums.EValidateType.vtUpdate);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Entry(pedidoItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o Item do pedido!");
                return BadRequest(result);
            }

            return Ok(pedidoItem);
        }

        // POST: api/PedidosItens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PedidoItem>> PostPedidoItem([FromBody]PedidoItem pedidoItem, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            pedidoItem.IDEmpresa = emp.Id;

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedidoItem.IDPedido && x.IDEmpresa == emp.Id);
            if (pedido == null || pedido == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            if (pedido.Status != "Pendente")
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível inserir um item em um pedido com o status diferente de pendente!");
                return BadRequest(result);
            }

            var prod = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedidoItem.IDProduto && x.IDEmpresa == emp.Id);
            if (prod == null || prod == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Produto não encontrado!");
                return BadRequest(result);
            }

            var item = await _context.PedidosItems.AsNoTracking().FirstOrDefaultAsync(
               x => x.IDPedido == pedidoItem.IDPedido &&
               x.IDProduto == pedidoItem.IDProduto &&
               x.Id != pedidoItem.Id);
            if (item != null && item != default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Já existe um item neste pedido que está usando este produto!");
                return BadRequest(result);
            }

            try
            {
                pedidoItem.Validate(Enums.EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.PedidosItems.Add(pedidoItem);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir o Pedido Item!");
                return BadRequest(result);
            }

            return CreatedAtAction("GetPedidoItem", new { id = pedidoItem.Id }, pedidoItem);
        }

        // DELETE: api/PedidosItens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoItem(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var pitem = await _context.PedidosItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (pitem == null || pitem == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido Item não encontrado!");
                return BadRequest(result);
            }

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pitem.IDPedido && x.IDEmpresa == emp.Id);
            if (pedido.Status != "Pendente")
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível excluir um item de um pedido com o status diferente de pendente!");
                return BadRequest(result);
            }

            try
            {
                pitem.Validate(Enums.EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.PedidosItems.Remove(pitem);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir o Pedido Item!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Pedido item excluído com sucesso!");
            return Ok(resposta);
        }

    }
}
