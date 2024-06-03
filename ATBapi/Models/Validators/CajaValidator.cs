using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ATBapi.Models.Validators
{
    public class CajaValidator : AbstractValidator<CajaDTO>
    {
        private readonly atbContext context;

        public CajaValidator(atbContext context)
        {
            RuleFor(x => x.Nombre)
               .NotEmpty().WithMessage("El nombre no debe estar vacío.")
               .Must(BeUniqueName).WithMessage("El nombre ya está en uso.");
            this.context = context;
        }
        private bool BeUniqueName(string nombre)
        {
            return !context.Caja.Any(c => c.Nombre == nombre);
        }
    }
}
