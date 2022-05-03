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
using APIGestao.Enums;

namespace APIGestao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly MyContext _context;

        public ClientesController(MyContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes([FromHeader] Guid AccessToken, [FromHeader] string Nome)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (String.IsNullOrEmpty(Nome))
                return await _context.Clientes.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();
            else
                return await _context.Clientes.AsNoTracking().Where(x => x.IDEmpresa == emp.Id && x.Nome == Nome).ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (cliente == null || cliente == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Cliente não encontrado!");
                return BadRequest(result);
            }

            return cliente;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(long id, [FromBody]Cliente cliente, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (id != cliente.Id)
            {
                return BadRequest();
            }

            var cli = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (cli == null || cli == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Cliente não encontrado!");
                return BadRequest(result);
            }

            cliente.IDEmpresa = emp.Id;

            try 
            {
                cliente.Validate(Enums.EValidateType.vtUpdate);
            } 
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Entry(cliente).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o cliente!");
                return BadRequest(result);
            }

            return Ok(cliente);
        }

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente([FromBody] Cliente cliente, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            cliente.IDEmpresa = emp.Id;

            try
            {
                cliente.Validate(EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir o cliente!");
                return BadRequest(result);
            }

            return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (cliente == null || cliente == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Cliente não encontrado!");
                return BadRequest(result);
            }

            var pedidos = await _context.Pedidos.AsNoTracking().Where(x => x.IDCliente == cliente.Id).ToListAsync();
            if (pedidos != null && pedidos.Count > 0)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível deletar o Cliente pois existe um ou mais pedidos vinculados a ele!");
                return BadRequest(result);
            }

            try
            {
                cliente.Validate(EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir o cliente!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem","Cliente excluído com sucesso!");
            return Ok(resposta);
        }

    }
}
