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
    public class ConfiguracionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfiguracionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarConfiguracionActivos")]
        public async Task<ActionResult<IEnumerable<Configuracion>>> ListarConfiguracionActivos()
        {
            // Filtrar con estado "Activo"
            var ConfigurarionActivos = await _context.Configuraciones
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return ConfigurarionActivos;
        }

        // GET: api/Configuraciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Configuracion>>> GetConfiguraciones()
        {
            return await _context.Configuraciones.ToListAsync();
        }

        // GET: api/Configuraciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Configuracion>> GetConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones.FindAsync(id);

            if (configuracion == null)
            {
                return NotFound();
            }

            return configuracion;
        }

        // PUT: api/Configuraciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarConfiguracion(int id, string nombre, string correo, int telefono, string direccion)
        {
            // Busca la persona por su ID
            var configuracionActual = await _context.Configuraciones.FindAsync(id);

            if (configuracionActual == null)
            {
                return NotFound("La configuracion no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            configuracionActual.Nombre = nombre;
            configuracionActual.Correo = correo;
            configuracionActual.Telefono = telefono;
            configuracionActual.Direccion = direccion;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(configuracionActual);
        }

        // POST: api/Configuraciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearConfiguracion(string nombre, string correo, int telefono, string direccion)
        {

            Configuracion configuracion = new Configuracion()
            {
                Nombre = nombre,
                Correo = correo,
                Telefono = telefono,
                Direccion = direccion,
                Estado = "Activo"
            };

            await _context.Configuraciones.AddAsync(configuracion);
            await _context.SaveChangesAsync();

            return Ok(configuracion);
        }

        // DELETE: api/Configuraciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones.FindAsync(id);

            if (configuracion == null)
            {
                return NotFound("La configuracion no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            configuracion.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "La configuracion ha sido desactivado." });
        }
    }
}
