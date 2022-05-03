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
    public class PedidosController : ControllerBase
    {
        private readonly MyContext _context;

        public PedidosController(MyContext context)
        {
            _context = context;
        }

        // GET: api/Pedidoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos([FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            return await _context.Pedidos.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();
        }

        // GET: api/Pedidoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (pedido == null || pedido == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            return pedido;
        }

        // PUT: api/Pedidoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(long id, [FromBody] Pedido pedido, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (id != pedido.Id)
            {
                return BadRequest();
            }

            var ped = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (ped == null || ped == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            var cli = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedido.IDCliente && x.IDEmpresa == emp.Id);
            if (cli == null || cli == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Cliente não encontrado!");
                return BadRequest(result);
            }

            pedido.IDEmpresa = emp.Id;
            pedido.Status = ped.Status;

            try
            {
                pedido.Validate(Enums.EValidateType.vtUpdate);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Entry(pedido).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o Pedido!");
                return BadRequest(result);
            }

            return Ok(pedido);
        }

        // POST: api/Pedidoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido([FromBody] Pedido pedido, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var cli = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedido.IDCliente && x.IDEmpresa == emp.Id);
            if (cli == null || cli == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Cliente não encontrado!");
                return BadRequest(result);
            }

            pedido.IDEmpresa = emp.Id;
            pedido.Status = "Pendente";

            try
            {
                pedido.Validate(Enums.EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir o pedido!");
                return BadRequest(result);
            }

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedidoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (pedido == null || pedido == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            var pedItens = await _context.PedidosItems.AsNoTracking().Where(x => x.IDPedido == pedido.Id).ToListAsync();
            if (pedItens != null && pedItens.Count > 0)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "O pedido não pode ser excluído pois existe um ou mais itens vinculados a ele!");
                return BadRequest(result);
            }

            try
            {
                pedido.Validate(Enums.EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir o Pedido!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Pedido excluído com sucesso!");
            return Ok(resposta);
        }

        [Route("{id}/Realizar")]
        [HttpPost]
        public async Task<IActionResult> RealizarPedido(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (pedido == null || pedido == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            var Itens = await _context.PedidosItems.AsNoTracking().Where(x => x.IDPedido == pedido.Id && x.IDEmpresa == pedido.IDEmpresa).ToListAsync();
            if (Itens == null || Itens.Count == 0)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Para realizar um pedido ele deve ter ao menos um item!");
                return BadRequest(result);
            }

            foreach (PedidoItem pedidoItem in Itens)
            {
                var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedidoItem.IDProduto && x.IDEmpresa == pedidoItem.IDEmpresa);
                if (produto == null || produto == default)
                {
                    var result = new Dictionary<string, string>();
                    result.Add("Error", "Não é possível realizar o pedido, pois o Produto " + pedidoItem.IDProduto.ToString() + " não existe no banco!");
                    return BadRequest(result);
                }

                if ((produto.Quantidade - pedidoItem.Quantidade) < 0)
                {
                    var result = new Dictionary<string, string>();
                    result.Add("Error", "Não é possível realizar o pedido, pois o Produto " + produto.Nome + " ficaria com um saldo negativo!");
                    return BadRequest(result);
                }

            }

            try
            {
                pedido.Validate(Enums.EValidateType.vtRealize);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }


            foreach (PedidoItem pedidoItem in Itens)
            {
                var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pedidoItem.IDProduto && x.IDEmpresa == pedidoItem.IDEmpresa);
                if (produto == null || produto == default)
                {
                    var result = new Dictionary<string, string>();
                    result.Add("Error", "Não é possível realizar o pedido, pois o Produto " + pedidoItem.IDProduto.ToString() + " não existe no banco!");
                    return BadRequest(result);
                }

                produto.Quantidade -= pedidoItem.Quantidade;
                _context.Entry(produto).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Entry(produto).State = EntityState.Detached;

            }

            pedido.Status = "Realizado";
            try
            {
                _context.Entry(pedido).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o Pedido!");
                return BadRequest(result);
            }

            Dictionary<string, string> resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Pedido realizado com sucesso!");
            return Ok(resposta);
        }

        [Route("{id}/Cancelar")]
        [HttpPost]
        public async Task<IActionResult> CancelarPedido(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (pedido == null || pedido == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Pedido não encontrado!");
                return BadRequest(result);
            }

            try
            {
                pedido.Validate(Enums.EValidateType.vtCancel);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            pedido.Status = "Cancelado";
            try
            {
                _context.Entry(pedido).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o Pedido!");
                return BadRequest(result);
            }

            Dictionary<string, string> resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Pedido cancelado com sucesso!");
            return Ok(resposta);

        }
    }
}
