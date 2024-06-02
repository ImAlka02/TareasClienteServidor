using System;
using System.Collections.Generic;

namespace ATBapi.Models.Entities;

public partial class Usuarioxcaja
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdCaja { get; set; }

    public DateTime HoraInicial { get; set; }

    public DateTime? HoraFinal { get; set; }

    public virtual Caja IdCajaNavigation { get; set; } = null!;

    public virtual Users IdUsuarioNavigation { get; set; } = null!;
}
