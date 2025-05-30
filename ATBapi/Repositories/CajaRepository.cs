﻿using ApiActividades.Repositories;
using ATBapi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATBapi.Repositories
{
    public class CajaRepository : Repository<Caja>
    {
        public CajaRepository(atbContext context) : base(context)
        {
            
        }

        public Caja? GetByNombre(string nombre)
        {
            var caja = context.Caja.FirstOrDefault(x=>x.Nombre == nombre);
            if (caja != null)
            {
                return caja;
            }
            return null;
        }

        public Caja? GetById (int? id) 
        {
            var caja = context.Caja.Include(x=>x.Users).FirstOrDefault(x=>x.Id == id);
            if (caja != null)
            {
                return caja;
            }
            return null;
        }
    }
}
