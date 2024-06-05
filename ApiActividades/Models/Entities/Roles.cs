using System;
using System.Collections.Generic;

namespace ApiActividades.Models.Entities;

public partial class Roles
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}
