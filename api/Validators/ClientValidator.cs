using api.Models;
using FluentValidation;
using System.Runtime.CompilerServices;

namespace api.Validators
{
    public class ClientValidator : AbstractValidator<Client>
    {

        public ClientValidator() 
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.PhoneNumber).Matches(@"^[0-9*#+]+$");
        }


    }
}
