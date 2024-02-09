using System;
using System.Collections.Generic;

namespace ApiPrueba.Models;

public partial class Estancia
{
    public int IdEstancia { get; set; }

    public string? IdVehiculo { get; set; }

    public DateTime? HoraEntrada { get; set; }

    public DateTime? HoraSalida { get; set; }

    public decimal? TotalPago { get; set; }

    public virtual Vehiculo? IdVehiculoNavigation { get; set; }
}
