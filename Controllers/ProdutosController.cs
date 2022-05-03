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
    public class ProdutosController : ControllerBase
    {
        private readonly MyContext _context;

        public ProdutosController(MyContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos([FromHeader] Guid AccessToken, [FromHeader] string Nome)
        {
            var emp = await _context.Empresas.FirstOrDefaultAsync(x => x.AccessToken == AccessToken);

            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (String.IsNullOrEmpty(Nome))
                return await _context.Produtos.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();
            else
                return await _context.Produtos.AsNoTracking().Where(x => x.IDEmpresa == emp.Id && x.Nome == Nome).ToListAsync();
        }

        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.FirstOrDefaultAsync(x => x.AccessToken == AccessToken);

            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.IDEmpresa == emp.Id && x.Id == id);

            if (produto == null || produto == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Produto não encontrado!");
                return BadRequest(result);
            }

            return produto;
        }

        // PUT: api/Produtos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(long id, [FromBody]Produto produto, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (id != produto.Id)
            {
                return BadRequest();
            }

            var prod = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (prod == null || prod == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Produto não encontrado!");
                return BadRequest(result);
            }

            produto.IDEmpresa = emp.Id;

            try
            {
                produto.Validate(Enums.EValidateType.vtUpdate);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Entry(produto).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o produto!");
                return BadRequest(result);
            }

            return Ok(produto);
        }

        // POST: api/Produtos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto([FromBody]Produto produto, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            produto.IDEmpresa = emp.Id;

            try
            {
                produto.Validate(Enums.EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir o produto!");
                return BadRequest(result);
            }

            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

        // DELETE: api/Produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (produto == null || produto == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Produto não encontrado!");
                return BadRequest(result);
            }

            var pedidosItens = await _context.PedidosItems.AsNoTracking().Where(x => x.IDProduto == produto.Id).ToListAsync();
            if (pedidosItens != null && pedidosItens.Count > 0)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível deletar o produto pois existe um ou mais pedidos vinculados a ele!");
                return BadRequest(result);
            }

            try
            {
                produto.Validate(Enums.EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir o produto!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Produto excluído com sucesso!");
            return Ok(resposta);
        }
    }
}
