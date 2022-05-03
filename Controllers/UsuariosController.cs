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
    public class UsuariosController : ControllerBase
    {
        private readonly MyContext _context;

        public UsuariosController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetUsuarios([FromHeader] Guid AccessToken) 
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var usuarios = await _context.Usuarios.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();

            foreach (Usuario usuario in usuarios) 
            {
                usuario.Senha = "";
            }

            return usuarios;

        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (usuario == null || usuario == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Usuário não encontrado!");
                return BadRequest(result);
            }

            usuario.Senha = "";
            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(long id, [FromBody] Usuario usuario, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            if (id != usuario.Id)
            {
                return BadRequest();
            }

            var us = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (us == null || us == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Usuário não encontrado!");
                return BadRequest(result);
            }

            usuario.Senha = String.IsNullOrEmpty(usuario.Senha) ? us.Senha : usuario.Senha;
            usuario.IDEmpresa = emp.Id;
            
            if (usuario.Login != us.Login)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível alterar o login!");
                return BadRequest(result);
            }

            try
            {
                usuario.Validate(EValidateType.vtUpdate);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Entry(usuario).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar atualizar o usuário!");
                return BadRequest(result);
            }

            usuario.Login = "";
            usuario.Senha = "";
            return Ok(usuario);
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] Usuario usuario, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var us = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Login == usuario.Login);
            if (us != null && us != default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Login já cadastrado!");
                return BadRequest(result);
            }

            usuario.IDEmpresa = emp.Id;

            try
            {
                usuario.Validate(EValidateType.vtInsert);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar inserir o usuário!");
                return BadRequest(result);
            }

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(long id, [FromHeader] Guid AccessToken)
        {
            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.AccessToken == AccessToken);
            if (emp == null || emp == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Access Token inválido!");
                return BadRequest(result);
            }

            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IDEmpresa == emp.Id);
            if (usuario == null || usuario == default)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Usuário não encontrado!");
                return BadRequest(result);
            }

            var usuarios = await _context.Usuarios.AsNoTracking().Where(x => x.IDEmpresa == emp.Id).ToListAsync();
            if (usuarios.Count() <= 1)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Não é possível apagar todos os usuários!");
                return BadRequest(result);
            }

            try
            {
                usuario.Validate(EValidateType.vtDelete);
            }
            catch (ValidateException e)
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", e.Message);
                return BadRequest(result);
            }

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            catch
            {
                var result = new Dictionary<string, string>();
                result.Add("Error", "Ocorreu um erro ao tentar excluir o usuário!");
                return BadRequest(result);
            }

            var resposta = new Dictionary<string, string>();
            resposta.Add("Mensagem", "Usuário excluído com sucesso!");
            return Ok(resposta);
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<Dictionary<string, Guid>>> Login([FromBody] LoginModel usuario)
        {
            if (String.IsNullOrEmpty(usuario.Login) || String.IsNullOrEmpty(usuario.Senha))
            {
                var resposta = new Dictionary<string, string>();
                resposta.Add("Error", "Usuário inválido!");
                return BadRequest(resposta);
            }

            var us = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Login == usuario.Login && x.Senha == usuario.Senha);
            if (us == null || us == default)
            {
                var resposta = new Dictionary<string, string>();
                resposta.Add("Error", "Usuário ou senha preenchido incorretamente!");
                return BadRequest(resposta);
            }

            var emp = await _context.Empresas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == us.IDEmpresa);
            var result = new Dictionary<string, Guid>();
            result.Add("AccessToken", emp.AccessToken);

            if (emp != null && emp != default)
            {
                var cli = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Nome == "visitante" && x.IDEmpresa == emp.Id);
                if (cli == null || cli == default) 
                {
                    cli = new Cliente();
                    cli.Nome = "visitante";
                    cli.CPF = "000.000.000-00";
                    cli.IDEmpresa = emp.Id;

                    try
                    {
                        _context.Clientes.Add(cli);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        var resposta = new Dictionary<string, string>();
                        resposta.Add("Error", "Ocorreu um erro ao tentar cadastrar o cliente visitante!");
                        return BadRequest(resposta);
                    }

                }
            }

            return Ok(result);

        }
    }
}
