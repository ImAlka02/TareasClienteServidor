using ApiActividades.Models.DTOs;
using ApiActividades.Models.Entities;
using ApiActividades.Repositories;
using FluentValidation;

namespace ApiActividades.Models.Validators
{
    public class DepartamentoValidator : AbstractValidator<DepartamentoDTO>
    {
        private readonly ItesrcneActividadesContext context;

        public DepartamentoValidator(ItesrcneActividadesContext context)
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre no debe estar vacío.")
                .Must(Repetido).WithMessage("El nombre ya esta en uso."); ;
            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("El correo no debe estar vacío.")
                .EmailAddress().WithMessage("El correo no es válido.")
                .Must(Repetido).WithMessage("El correo ya esta en uso.");
            RuleFor(x => x.Contraseña)
                .NotEmpty().WithMessage("La contraseña no debe estar vacía.");
            //RuleFor(x => x.IdSuperior).NotEmpty().WithMessage("El ID del superior no debe estar vacío.")
            //    .Must(ExisteIdSuperior).WithMessage("El ID del superior no existe.");
            this.context = context;
        }

        private bool ExisteIdSuperior(int id)
        {
            return context.Departamentos.Any(a => a.Id == id);
        }
        private bool Repetido(string dato)
        {
            return !context.Departamentos.Any(x=> x.Nombre == dato  || x.Username == dato );
        }
    
    }
}
