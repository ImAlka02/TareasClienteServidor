using System;
using System.Collections.Generic;

namespace ApiActividades.Models.Entities;

public partial class Colaespera
{
    public int Id { get; set; }

    public string NumeroTurno { get; set; } = null!;

    public DateTime DateTurnoCreado { get; set; }
}
