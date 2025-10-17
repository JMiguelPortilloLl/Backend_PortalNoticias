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
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ListarCategoriasActivos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> ListarCategoriasActivos()
        {
            // Filtrar con estado "Activo"
            var CategoriasActivos = await _context.Categorias
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return CategoriasActivos;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return categoria;
        }

        // PUT: api/Categorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarCategoria(int id, string descripcion)
        {
            // Busca la persona por su ID
            var categoriaActual = await _context.Categorias.FindAsync(id);

            if (categoriaActual == null)
            {
                return NotFound("La categoria no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            categoriaActual.Descripcion = descripcion;

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(categoriaActual);
        }

        // POST: api/Categorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> CrearCategoria(string descripcion)
        {

            Categoria categoria = new Categoria()
            {
                Descripcion = descripcion,
                Estado = "Activo"
            };

            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            return Ok(categoria);
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategorias(int id)
        {
            var Categoria = await _context.Categorias.FindAsync(id);

            if (Categoria == null)
            {
                return NotFound("La categoria no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            Categoria.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "La categoria ha sido desactivado." });
        }
    }
}
