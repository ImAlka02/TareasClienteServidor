using System;
using System.Collections.Generic;

namespace ATBapi.Models.Entities;

public partial class Users
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public int IdRole { get; set; }

    public virtual Roles IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Usuarioxcaja> Usuarioxcaja { get; set; } = new List<Usuarioxcaja>();
}
