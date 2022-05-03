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
    public class DespesasController : ControllerBase
    {
        private readonly MyContext _context;

        public DespesasController(MyContext context)
        {
            _context = context;
        }

        // GET: api/Despesas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Despesa>>> GetDespesas([FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            return await _context.Despesas.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();
        }

        // GET: api/Despesas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Despesa>> GetDespesa(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var despesa = await _context.Despesas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (despesa == null || despesa == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Despesa não encontrada!");
                return BadRequest(result);
            }

            return despesa;
        }

        // PUT: api/Despesas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDespesa(long id, [FromBody]Despesa despesa, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (id != despesa.Id)
            {
                return BadRequest();
            }

            var dp = await _context.Despesas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (dp == null || dp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Despesa não encontrada!");
                return BadRequest(result);
            }

            despesa.IDEmpresa = emp.Id;

            try
            {
                despesa.Validate(EValidateType.vtUpdate);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Entry(despesa).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar a despesa!");
                return BadRequest(result);

            }

            return NoContent();
        }

        // POST: api/Despesas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Despesa>> PostDespesa([FromBody] Despesa despesa, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            despesa.IDEmpresa = emp.Id;
  
            try
            {
                despesa.Validate(EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Despesas.Add(despesa);
                await _context.SaveChangesAsync();
            }
            catch 
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir a despesa!");
                return BadRequest(result);
            }

            return CreatedAtAction("GetDespesa", new { id = despesa.Id }, despesa);
        }

        // DELETE: api/Despesas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Despesa>> DeleteDespesa(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var despesa = await _context.Despesas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (despesa == null || despesa == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Despesa não encontrada!");
                return BadRequest(result);
            }

            try
            {
                despesa.Validate(EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Despesas.Remove(despesa);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir a despesa!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Despesa excluída com sucesso!");
            return Ok(resposta);
        }

    }
}
