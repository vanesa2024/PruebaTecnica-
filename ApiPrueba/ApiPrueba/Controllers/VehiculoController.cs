using ApiPrueba.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculoController : ControllerBase
    {
        public readonly EstacionamientoContext _econtext;

        public VehiculoController(EstacionamientoContext _context)
        {
            _econtext = _context;
        }


        [HttpPost("dar-de-alta-oficial")]
        public IActionResult DarDeAltaOficial(string numeroPlaca)
        {
            // Verificar si el vehículo ya está registrado como oficial
            var vehiculoOficial = _econtext.Vehiculos.FirstOrDefault(v => v.NumeroPlaca == numeroPlaca && v.IdTipoVehiculoNavigation.TipoVehiculo1 == "Oficial");
            if (vehiculoOficial != null)
            {
                return Conflict("El vehículo ya está registrado como vehículo oficial.");
            }

            // Agregar el vehículo a la lista de vehículos oficiales
            var nuevoVehiculoOficial = new Vehiculo
            {
                NumeroPlaca = numeroPlaca,
                IdTipoVehiculo = ObtenerIdTipoVehiculoOficial()
            };
            _econtext.Vehiculos.Add(nuevoVehiculoOficial);
            _econtext.SaveChanges();

            return Ok("El vehículo se añadió correctamente.");
        }

        // Método privado para obtener el ID del tipo de vehículo "Oficial"
        private int ObtenerIdTipoVehiculoOficial()
        {
            return 1;
        }


        [HttpPost("dar-de-alta-residente")]
        public IActionResult DarDeAltaResidente(string numeroPlaca)
        {
            // Verificar si el vehículo ya está registrado como residente
            var vehiculoResidente = _econtext.Vehiculos.FirstOrDefault(v => v.NumeroPlaca == numeroPlaca && v.IdTipoVehiculoNavigation.TipoVehiculo1 == "Residente");
            if (vehiculoResidente != null)
            {
                return Conflict("El vehículo ya está registrado como vehículo de residente.");
            }

            // Agregar el vehículo a la lista de vehículos de residentes
            var nuevoVehiculoResidente = new Vehiculo
            {
                NumeroPlaca = numeroPlaca,
                IdTipoVehiculo = ObtenerIdTipoVehiculoResidente()
            };
            _econtext.Vehiculos.Add(nuevoVehiculoResidente);
            _econtext.SaveChanges();

            return Ok("El vehículo se añadió correctamente.");
        }

        // Método privado para obtener el ID del tipo de vehículo "Residente"
        private int ObtenerIdTipoVehiculoResidente()
        {
            return 2;
        }
    }
}
