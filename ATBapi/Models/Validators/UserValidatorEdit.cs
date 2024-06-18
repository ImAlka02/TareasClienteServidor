using ATBapi.Models.DTOs;
using ATBapi.Models.Entities;
using FluentValidation;

namespace ATBapi.Models.Validators
{
    public class UserValidatorEdit: AbstractValidator<UserCompleteDTO>
    {
        private readonly atbContext context;

        public UserValidatorEdit(atbContext context)
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre no debe estar vacío.");
            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("El correo no debe estar vacío.")
                .EmailAddress().WithMessage("El correo no es válido.");
            RuleFor(x => x.Contraseña)
                .NotEmpty().WithMessage("La contraseña no debe estar vacía.");
            RuleFor(x => x.IdRol).NotEmpty().WithMessage("El rol no debe estar vacío.");
            this.context = context;
        }
        
    }
    
    
}
