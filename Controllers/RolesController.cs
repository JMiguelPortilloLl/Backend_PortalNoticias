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
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("ListarRolesActivos")]
        public async Task<ActionResult<IEnumerable<Rol>>> ListarRolActivos()
        {
            // Filtrar con estado "Activo"
            var RolesActivos = await _context.Roles
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return RolesActivos;
        }


        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return rol;
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarRol(int id, string descripcion)
        {
            // Busca la persona por su ID
            var rolActual = await _context.Roles.FindAsync(id);

            if (rolActual == null)
            {
                return NotFound("El rol no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            rolActual.Descripcion = descripcion;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(rolActual);
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearRol(string descripcion)
        {

            Rol rol = new Rol()
            {
                Descripcion = descripcion,
                Estado = "Activo"
            };

            await _context.Roles.AddAsync(rol);
            await _context.SaveChangesAsync();

            return Ok(rol);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var Rol = await _context.Roles.FindAsync(id);

            if (Rol == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            Rol.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El usuario ha sido desactivado." });
        }
    }
}
