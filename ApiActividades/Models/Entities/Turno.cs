﻿using System;
using System.Collections.Generic;

namespace ApiActividades.Models.Entities;

public partial class Turno
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public DateTime HoraInicial { get; set; }

    public DateTime? HoraFinal { get; set; }

    public string NumeroTurno { get; set; } = null!;

    public TimeOnly TiempoInicio { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Users IdUsuarioNavigation { get; set; } = null!;
}
