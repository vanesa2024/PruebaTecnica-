using System;
using System.Collections.Generic;

namespace ApiPrueba.Models;

public partial class TipoVehiculo
{
    public int IdTipoVehiculo { get; set; }

    public string? TipoVehiculo1 { get; set; }

    public decimal? Tarifa { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
