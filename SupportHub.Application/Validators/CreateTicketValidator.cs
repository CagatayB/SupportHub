using FluentValidation;
using SupportHub.Application.DTOs.Ticket;

namespace SupportHub.Application.Validators
{
    public class CreateTicketValidator : AbstractValidator<CreateTicketRequest>
    {
        public CreateTicketValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200).WithMessage("Başlık boş olamaz ve 200 karakteri geçemez.");
            RuleFor(x => x.Description).NotEmpty().MinimumLength(10).WithMessage("Lütfen sorunu detaylı açıklayın.");
        }
    }
}
