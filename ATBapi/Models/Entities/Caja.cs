using System;
using System.Collections.Generic;

namespace ATBapi.Models.Entities;

public partial class Caja
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuarioxcaja> Usuarioxcaja { get; set; } = new List<Usuarioxcaja>();
}
