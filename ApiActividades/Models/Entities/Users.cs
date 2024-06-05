using System;
using System.Collections.Generic;

namespace ApiActividades.Models.Entities;

public partial class Users
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public int IdRole { get; set; }

    public int? IdCaja { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Caja? IdCajaNavigation { get; set; }

    public virtual Roles IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Turno> Turno { get; set; } = new List<Turno>();
}
