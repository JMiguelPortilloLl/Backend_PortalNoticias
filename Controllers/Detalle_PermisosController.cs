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
    public class Detalle_PermisosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Detalle_PermisosController(AppDbContext context)
        {
            _context = context;
        }


		[HttpGet]
		[Route("ListarDetallePermisosActivosUsuario")]
		public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> ListarDetallePermisosActivosUsuario(int id)
		{
			// Filtrar con estado "Activo"
			var detallePermisosActivos = await _context.DetallePermisos
				.Where(e => e.idusuario == id)
				.ToListAsync();

			// Retornar la lista de activos
			return detallePermisosActivos;
		}

		[HttpGet]
        [Route("ListarDetallePermisosActivos")]
        public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> ListarDetallePermisosActivos()
        {
            // Filtrar con estado "Activo"
            var DetallePermisosActivos = await _context.DetallePermisos
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            // Retornar la lista de activos
            return DetallePermisosActivos;
        }

        // GET: api/Detalle_Permisos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detalle_Permiso>>> GetDetallePermisos()
        {
            return await _context.DetallePermisos.ToListAsync();
        }

        // GET: api/Detalle_Permisos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detalle_Permiso>> GetDetalle_Permiso(int id)
        {
            var detalle_Permiso = await _context.DetallePermisos.FindAsync(id);

            if (detalle_Permiso == null)
            {
                return NotFound();
            }

            return detalle_Permiso;
        }

        // PUT: api/Detalle_Permisos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Actualizar")]
        public async Task<IActionResult> ActualizarUsuario(int id, int idpermiso, int idusuario)
        {
            // Busca la persona por su ID
            var DetallePermisosActual = await _context.DetallePermisos.FindAsync(id);

            if (DetallePermisosActual == null)
            {
                return NotFound("El detalle permisos no fue encontrado.");
            }


            // Actualiza los campos con los nuevos valores
            DetallePermisosActual.idpermiso = idpermiso;
            DetallePermisosActual.idusuario = idusuario;


            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(DetallePermisosActual);
        }

		// POST: api/Detalle_Permisos
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost("Crear")]
		public async Task<IActionResult> CrearDetallePermiso([FromBody] AsignarPermisosRequest request)
		{
			if (request == null || request.Permisos == null || !request.Permisos.Any())
			{
				return BadRequest("La solicitud no es válida.");
			}

			try
			{
				// Obtener los permisos actuales del usuario
				var permisosActuales = await _context.DetallePermisos
					.Where(dp => dp.idusuario == request.IdUsuario)
					.ToListAsync();

				// Eliminar permisos que ya no están seleccionados
				var permisosAEliminar = permisosActuales
					.Where(pa => !request.Permisos.Contains(pa.idpermiso))
					.ToList();

				if (permisosAEliminar.Any())
				{
					_context.DetallePermisos.RemoveRange(permisosAEliminar);
					await _context.SaveChangesAsync();
				}

				// Agregar nuevos permisos
				var permisosAAgregar = request.Permisos
					.Where(id => !permisosActuales.Any(pa => pa.idpermiso == id))
					.Select(id => new Detalle_Permiso
					{
						idpermiso = id,
						idusuario = request.IdUsuario,
						Estado = "Activo"
					})
					.ToList();

				if (permisosAAgregar.Any())
				{
					await _context.DetallePermisos.AddRangeAsync(permisosAAgregar);
					await _context.SaveChangesAsync();
				}

				return Ok(new { Message = "Permisos asignados correctamente." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}

	
		public class AsignarPermisosRequest
		{
			public int IdUsuario { get; set; } // ID del usuario
			public List<int> Permisos { get; set; } // Lista de IDs de permisos
		}
	

	[HttpPost("ActualizarPermisos")]
		public IActionResult ActualizarPermisos([FromBody] UpdatePermissionsRequest request)
		{
			try
			{
				// Lógica para añadir permisos
				foreach (var permisoId in request.AddedPermissions)
				{
					// Añadir permiso al usuario
				}

				// Lógica para eliminar permisos
				foreach (var permisoId in request.RemovedPermissions)
				{
					// Eliminar permiso del usuario
				}

				return Ok("Permisos actualizados exitosamente");
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error al actualizar los permisos");
			}
		}

		public class UpdatePermissionsRequest
		{
			public int IdUsuario { get; set; }
			public List<int> AddedPermissions { get; set; }
			public List<int> RemovedPermissions { get; set; }
		}


		// DELETE: api/Detalle_Permisos/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallePermiso(int id)
        {
            var DetallePermiso = await _context.DetallePermisos.FindAsync(id);

            if (DetallePermiso == null)
            {
                return NotFound("El usuario no fue encontrado.");
            }

            // Cambiar el estado a "Inactivo" en lugar de eliminar
            DetallePermiso.Estado = "Inactivo";

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "El usuario ha sido desactivado." });
        }
    }
}
