using System;
using System.Collections.Generic;

namespace ATBapi.Models.Entities;

public partial class Configuracion
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;
}
