using ApiActividades.Models.DTOs;
using FluentValidation;

namespace ApiActividades.Models.Validators
{
    public class ActividadValidator : AbstractValidator<ActividadDTO>
    {
        public ActividadValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no debe estar vacio.");
            RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripción no debe estar vacio.");
            RuleFor(x => x.FechaRealizacion).NotNull().WithMessage("La fecha de realización no debe estar vacía.");
            RuleFor(x => x.FechaRealizacion).Must(BeInFuture).WithMessage("La fecha de realización no puede ser en el futuro.");
        }

        private bool BeInFuture(DateOnly? date)
        {
            return date.HasValue && date.Value <= DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
