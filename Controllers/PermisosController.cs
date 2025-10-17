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
    public class PermisosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermisosController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("ListarPermisosActivos")]
        public async Task<ActionResult<IEnumerable<Permiso>>> ListarPermisosActivos()
        {
            // Filtrar con estado "Activo"
            var PermisoActivos = await _context.Permisos
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return PermisoActivos;
        }



        // GET: api/Permisos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permiso>>> GetPermisos()
        {
            return await _context.Permisos.ToListAsync();
        }

        // GET: api/Permisos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Permiso>> GetPermiso(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);

            if (permiso == null)
            {
                return NotFound();
            }

            return permiso;
        }

        // PUT: api/Permisos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarPermiso(int id, string descripcion)
        {
            // Busca la persona por su ID
            var permisosActual = await _context.Permisos.FindAsync(id);

            if (permisosActual == null)
            {
                return NotFound("El permiso no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            permisosActual.Descripcion = descripcion;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(permisosActual);
        }

        // POST: api/Permisos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearUsuario(string descripcion)
        {

            Permiso permiso = new Permiso()
            {
                Descripcion = descripcion,
                Estado = "Activo"
            };

            await _context.Permisos.AddAsync(permiso);
            await _context.SaveChangesAsync();

            return Ok(permiso);
        }

        // DELETE: api/Permisos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermiso(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);

            if (permiso == null)
            {
                return NotFound("El permiso no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            permiso.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El permiso ha sido desactivado." });
        }
    }
}
