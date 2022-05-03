using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIGestao.Context;
using APIGestao.Models;
using APIGestao.Enums;
using APIGestao.Exceptions;

namespace APIGestao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        private readonly MyContext _context;

        public EmpresasController(MyContext context)
        {
            _context = context;
        }

        // GET: api/Empresas/5
        [HttpGet("")]
        public async Task<ActionResult<Empresa>> GetEmpresa([FromHeader] Guid AccessToken )
        {
            var empresa = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);

            if (empresa == null || empresa == default )
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Empresa não encontrada!");
                return BadRequest(result);
            }

            return empresa;
        }

        // PUT: api/Empresas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("")]
        public async Task<IActionResult> PutEmpresa([FromHeader] Guid AccessToken, [FromBody] Empresa empresa)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);

            if (emp == null || emp == default )
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Empresa não encontrada!");
                return BadRequest(result);
            }

            try
            {
                empresa.Validate(EValidateType.vtUpdate);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            empresa.AccessToken = emp.AccessToken;

            try
            {
                _context.Entry(empresa).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar a empresa!");
                return BadRequest(result);
            }

            return Ok(empresa);
        }

        // POST: api/Empresas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Empresa>> PostEmpresa([FromBody] Empresa empresa)
        {
            try
            {
                empresa.Validate(EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            empresa.AccessToken = Guid.NewGuid();

            try
            {
                _context.Empresas.Add(empresa);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir a empresa!");
                return BadRequest(result);
            }


            return CreatedAtAction("GetEmpresa", new { id = empresa.Id }, empresa);
        }

        // DELETE: api/Empresas/5
        [HttpDelete("")]
        public async Task<IActionResult> DeleteEmpresa([FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);

            if (emp == null || emp == default )
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Empresa não encontrada!");
                return BadRequest(result);
            }

            var usuarios = await _context.Usuarios.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();

            if (usuarios != null && usuarios.Count > 0)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "A empresa não pode ser excluída pois existem usuários vinculados a ela!");
                return BadRequest(result);
            }

            try
            {
                emp.Validate(EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Empresas.Remove(emp);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir a empresa!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Empresa excluída com sucesso!");
            return Ok(resposta);
        }

    }
}
