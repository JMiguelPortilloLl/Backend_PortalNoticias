using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Periodico.Context;
using Periodico.Models;
using Microsoft.AspNetCore.Cors;

namespace Periodico.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class PublicacionesController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _environment;

		public PublicacionesController(AppDbContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		[HttpGet]
		[Route("ListarPorCategoria/{idCategoria}")]
		public async Task<ActionResult<IEnumerable<Publicacion>>> ListarPorCategoria(int idCategoria, [FromQuery] string estado = "Activo")
		{
			var query = _context.Publicaciones
				.Where(p => p.idcategoria == idCategoria);

			// Filtrar por estado si se especifica
			if (!string.IsNullOrEmpty(estado))
			{
				query = query.Where(p => p.Estado == estado);
			}

			// Ordenar por fecha (más reciente primero)
			var publicaciones = await query
				.OrderByDescending(p => p.Fecha)
				.ToListAsync();

			if (!publicaciones.Any())
			{
				return NotFound("No se encontraron publicaciones para los criterios especificados");
			}

			return publicaciones;
		}

		[HttpGet]
		[Route("ListarPublicacionesActivos")]
		public async Task<ActionResult<IEnumerable<Publicacion>>> ListarPublicacionActivos()
		{
			return await _context.Publicaciones
				.Where(e => e.Estado == "Activo")
				.OrderByDescending(p => p.Fecha) // Ordenar por fecha (más reciente primero)
				.ToListAsync();
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Publicacion>>> GetPublicaciones()
		{
			return await _context.Publicaciones
				.OrderByDescending(p => p.Fecha) // Ordenar por fecha (más reciente primero)
				.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Publicacion>> GetPublicacion(int id)
		{
			var publicacion = await _context.Publicaciones.FindAsync(id);
			if (publicacion == null) return NotFound();

			// Incrementar el contador de visualizaciones
			publicacion.Visualizacion += 1;
			await _context.SaveChangesAsync();

			return publicacion;
		}

		[HttpPut]
		[Route("Actualizar")]
		public async Task<IActionResult> ActualizarPublicacion(int id, [FromForm] PublicacionUpdateDto publicacionUpdate)
		{
			var publicacionActual = await _context.Publicaciones.FindAsync(id);
			if (publicacionActual == null) return NotFound("La publicación no fue encontrada.");

			// Actualizar solo los campos permitidos
			publicacionActual.Titulo = publicacionUpdate.Titulo;
			publicacionActual.Descripcion = publicacionUpdate.Descripcion;
			publicacionActual.Fecha = publicacionUpdate.Fecha;
			publicacionActual.idcategoria = publicacionUpdate.idcategoria;

			// Manejar imagen si se proporciona
			if (publicacionUpdate.Imagen != null)
			{
				// Eliminar imagen anterior si existe
				if (!string.IsNullOrEmpty(publicacionActual.Imagen))
				{
					var oldFilePath = Path.Combine(_environment.WebRootPath, "uploads", publicacionActual.Imagen);
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				// Guardar nueva imagen
				publicacionActual.Imagen = await GuardarImagen(publicacionUpdate.Imagen);
			}

			await _context.SaveChangesAsync();
			return Ok(publicacionActual);
		}

		[HttpPut]
		[Route("ActualizarSImg")]
		public async Task<IActionResult> ActualizarSImg(
	int id,
	[FromQuery] string titulo,
	[FromQuery] string descripcion,
	[FromQuery] string fecha,
	[FromQuery] int idcategoria,
	[FromQuery] string imagen = "")
		{
			var publicacionActual = await _context.Publicaciones.FindAsync(id);
			if (publicacionActual == null) return NotFound("Publicación no encontrada");

			// Parsear fecha
			if (!DateTime.TryParse(fecha, out var fechaParsed))
				return BadRequest("Formato de fecha inválido");

			// Actualizar campos
			publicacionActual.Titulo = titulo;
			publicacionActual.Descripcion = descripcion;
			publicacionActual.Fecha = fechaParsed;
			publicacionActual.idcategoria = idcategoria;

			// No actualizamos la imagen aquí, solo validamos que esté presente si es requerido
			await _context.SaveChangesAsync();

			return Ok(publicacionActual);
		}


		[HttpPost]
		[Route("Crear")]
		public async Task<IActionResult> CrearPublicacion([FromForm] PublicacionCreateDto publicacionCreate)
		{
			var publicacion = new Publicacion()
			{
				Titulo = publicacionCreate.Titulo,
				Descripcion = publicacionCreate.Descripcion,
				Fecha = publicacionCreate.Fecha,
				Visualizacion = 0, // Inicializar contador en 0
				idusuario = publicacionCreate.idusuario,
				idcategoria = publicacionCreate.idcategoria,
				Estado = "Activo"
			};

			if (publicacionCreate.Imagen != null)
			{
				publicacion.Imagen = await GuardarImagen(publicacionCreate.Imagen);
			}

			await _context.Publicaciones.AddAsync(publicacion);
			await _context.SaveChangesAsync();
			return Ok(publicacion);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePublicacion(int id)
		{
			var publicacion = await _context.Publicaciones.FindAsync(id);
			if (publicacion == null) return NotFound("La publicación no fue encontrada.");

			publicacion.Estado = "Inactivo";
			await _context.SaveChangesAsync();

			return Ok(new { message = "La publicación ha sido desactivada." });
		}

		private async Task<string> GuardarImagen(IFormFile imagen)
		{
			if (imagen == null || imagen.Length == 0)
				throw new ArgumentException("El archivo de imagen no es válido");

			var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileExtension = Path.GetExtension(imagen.FileName);
			var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
			var filePath = Path.Combine(uploadsFolder, uniqueFileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await imagen.CopyToAsync(stream);
			}

			return uniqueFileName;
		}
	}

	public class PublicacionCreateDto
	{
		public string Titulo { get; set; }
		public string Descripcion { get; set; }
		public DateTime Fecha { get; set; }
		public int idusuario { get; set; }
		public int idcategoria { get; set; }
		public IFormFile Imagen { get; set; }
	}

	public class PublicacionUpdateDto
	{
		public string Titulo { get; set; }
		public string Descripcion { get; set; }
		public DateTime Fecha { get; set; }
		public int idcategoria { get; set; }
		public IFormFile Imagen { get; set; }
	}
}