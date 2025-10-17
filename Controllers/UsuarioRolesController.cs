using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Periodico.Context;
using Periodico.Models;

namespace Periodico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioRolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioRolesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("ListarUsuariosRolesActivos")]
        public async Task<ActionResult<IEnumerable<UsuarioRol>>> ListarUsuarioRolActivos()
        {
            // Filtrar con estado "Activo"
            var UsuarioRolActivos = await _context.UsuarioRoles
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return UsuarioRolActivos;
        }

        // GET: api/UsuarioRoles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioRol>>> GetUsuarioRoles()
        {
            return await _context.UsuarioRoles.ToListAsync();
        }

        // GET: api/UsuarioRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioRol>> GetUsuarioRol(int id)
        {
            var usuarioRol = await _context.UsuarioRoles.FindAsync(id);

            if (usuarioRol == null)
            {
                return NotFound();
            }

            return usuarioRol;
        }

        // PUT: api/UsuarioRoles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarUsuario(int id, int idrol, int idusuario)
        {
            // Busca la persona por su ID
            var DetallePermisosActual = await _context.UsuarioRoles.FindAsync(id);

            if (DetallePermisosActual == null)
            {
                return NotFound("El UsuarioRol no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            DetallePermisosActual.idrol = idrol;
            DetallePermisosActual.idusuario = idusuario;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(DetallePermisosActual);
        }

        // POST: api/UsuarioRoles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearUsuarioRol(int idrol, int idusuario)
        {

            UsuarioRol usuariorol = new UsuarioRol()
            {
                idusuario = idusuario,
                idrol = idrol,
                Estado = "Activo"
            };

            await _context.UsuarioRoles.AddAsync(usuariorol);
            await _context.SaveChangesAsync();

            return Ok(usuariorol);
        }

        // DELETE: api/UsuarioRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioRol(int id)
        {
            var usuarioRol = await _context.UsuarioRoles.FindAsync(id);

            if (usuarioRol == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            usuarioRol.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El usuario ha sido desactivado." });
        }
    }
}
