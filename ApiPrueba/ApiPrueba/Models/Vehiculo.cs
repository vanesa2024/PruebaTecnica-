using System;
using System.Collections.Generic;

namespace ApiPrueba.Models;

public partial class Vehiculo
{
    public string NumeroPlaca { get; set; } = null!;

    public int? IdTipoVehiculo { get; set; }

    public virtual ICollection<Estancia> Estancia { get; set; } = new List<Estancia>();

    public virtual TipoVehiculo? IdTipoVehiculoNavigation { get; set; }
}
