using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using ApiPrueba.Models;
using System.Text;

namespace ApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstanciaController : ControllerBase
    {
        public readonly EstacionamientoContext _econtext;

        public EstanciaController(EstacionamientoContext _context)
        {
            _econtext = _context;
        }


        [HttpPost("registra-entrada")]
        public IActionResult RegistraEntrada(string numeroPlaca)
        {
            try
            {
                // Verificar si el vehículo ya ha sido registrado
                var vehiculo = _econtext.Vehiculos.FirstOrDefault(v => v.NumeroPlaca == numeroPlaca);
                if (vehiculo == null)
                {
                    return BadRequest("El vehículo no se encuentra registrado en la base de datos.");
                }

                // Crear una nueva entrada de estancia para el vehículo
                var nuevaEstancia = new Estancia
                {
                    IdVehiculo = vehiculo.NumeroPlaca,
                    HoraEntrada = DateTime.Now
                };

                // Guardar la nueva entrada de estancia en la base de datos
                _econtext.Estancia.Add(nuevaEstancia);
                _econtext.SaveChanges();

                return Ok("Registro Exitoso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la entrada: {ex.Message}");
            }
        }

        [HttpPut("registra-salida")]
        public IActionResult RegistrarSalida(string numeroPlaca)
        {
            try
            {
                // Buscar el vehículo por número de placa
                var vehiculo = _econtext.Vehiculos
                    .Include(v => v.IdTipoVehiculoNavigation) // Incluir la entidad relacionada TipoVehiculo
                    .FirstOrDefault(v => v.NumeroPlaca == numeroPlaca);

                if (vehiculo == null)
                {
                    return NotFound($"No se encontró un vehículo con el número de placa {numeroPlaca}");
                }

                // Obtener la última estancia del vehículo
                var ultimaEstancia = _econtext.Estancia
                .Where(e => e.IdVehiculo == numeroPlaca)  // Filtrar por número de placa
                .OrderByDescending(e => e.HoraEntrada)    // Ordenar por hora de entrada descendente
                .FirstOrDefault();

                if (ultimaEstancia == null)
                {
                    return BadRequest($"No se encontró una estancia previa para el vehículo con número de placa {numeroPlaca}");
                }

                // Calcular el tiempo estacionado en minutos
                DateTime horaSalida = DateTime.Now;
                TimeSpan duracionEstancia = horaSalida - ultimaEstancia.HoraEntrada.Value;
                int tiempoEstacionado = (int)duracionEstancia.TotalMinutes;

                // Calcular el total a pagar basado en la tarifa del tipo de vehículo
                decimal totalPagar = tiempoEstacionado * (vehiculo.IdTipoVehiculoNavigation.Tarifa ?? 0);

                // Actualizar la hora de salida y el total del pago
                ultimaEstancia.HoraSalida = DateTime.Now;
                ultimaEstancia.TotalPago = totalPagar;

                _econtext.SaveChanges();

                // Si el vehículo es no residente, devolver el importe a pagar
                if (vehiculo.IdTipoVehiculoNavigation.TipoVehiculo1 == "No residente")
                {
                    return Ok($"Salida registrada exitosamente. Importe a pagar: {totalPagar}");
                }

                return Ok("Salida registrada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la salida: {ex.Message}");
            }
        }

        
        [HttpDelete("comienza-mes")]
        public IActionResult ComenzarMes()
        {
            try
            {
                // Eliminar las estancias registradas en los coches oficiales
                var estanciasOficiales = _econtext.Estancia.Where(e => e.IdVehiculoNavigation.IdTipoVehiculoNavigation.TipoVehiculo1 == "Oficial");
                _econtext.Estancia.RemoveRange(estanciasOficiales);

                // Eliminar las estancias registradas en los coches de residentes
                var estanciasResidentes = _econtext.Estancia.Where(e => e.IdVehiculoNavigation.IdTipoVehiculoNavigation.TipoVehiculo1 == "Residente");
                _econtext.Estancia.RemoveRange(estanciasResidentes);

                // Guardar los cambios en la base de datos
                _econtext.SaveChanges();

                return Ok("Se ha comenzado el mes exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al comenzar el mes: {ex.Message}");
            }
        }


        [HttpPost("reporte-pagos-residentes")]
        public IActionResult GenerarPagosResidentes(string nombreArchivo)
        {
            try
            {
                // Obtener la lista de vehículos residentes
                var vehiculosResidentes = _econtext.Vehiculos
                    .Include(v => v.IdTipoVehiculoNavigation)
                    .Where(v => v.IdTipoVehiculoNavigation.TipoVehiculo1 == "Residente")
                    .ToList();

                // Crear una cadena para almacenar los datos del archivo
                StringBuilder fileContent = new StringBuilder();
                fileContent.AppendLine("Núm. placa\tTiempo estacionado (min.)\tCantidad a pagar");

                // Calcular el tiempo estacionado y el importe a pagar por cada vehículo
                foreach (var vehiculo in vehiculosResidentes)
                {
                    // Calcular el tiempo estacionado en minutos
                    int tiempoEstacionado = CalcularTiempoEstacionado(vehiculo);

                    if (vehiculo.IdTipoVehiculoNavigation != null && vehiculo.IdTipoVehiculoNavigation.Tarifa != null)
                    {
                        decimal cantidadPagar = tiempoEstacionado * vehiculo.IdTipoVehiculoNavigation.Tarifa.Value;
                        // Agregar los datos del vehículo al archivo
                        fileContent.AppendLine($"{vehiculo.NumeroPlaca}\t\t\t{tiempoEstacionado}\t\t\t{cantidadPagar.ToString("F2")}");
                    }
                }

                // Escribir los datos en el archivo
                string filePath = $"C:/Users/Vanesa/Desktop/Proyectos APIS/{nombreArchivo}.txt";
                System.IO.File.WriteAllText(filePath, fileContent.ToString());

                return Ok($"Se ha generado el archivo de pagos para residentes en: {filePath}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el archivo de pagos para residentes: {ex.Message}");
            }
        }

        private int CalcularTiempoEstacionado(Vehiculo vehiculo)
        {
            // Obtener todas las estancias del vehículo
            var estancias = _econtext.Estancia
                .Where(e => e.IdVehiculo == vehiculo.NumeroPlaca)
                .ToList();

            // Calcular el tiempo total estacionado sumando la duración de cada estancia
            int tiempoTotalEstacionado = 0;
            foreach (var estancia in estancias)
            {
                if (estancia.HoraEntrada.HasValue && estancia.HoraSalida.HasValue)
                {
                    TimeSpan duracionEstancia = estancia.HoraSalida.Value - estancia.HoraEntrada.Value;
                    tiempoTotalEstacionado += (int)duracionEstancia.TotalMinutes;
                }
            }

            return tiempoTotalEstacionado;
        }
    }
}
