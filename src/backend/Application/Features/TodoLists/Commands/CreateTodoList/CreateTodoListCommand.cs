using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Catalog;
using EvrenDev.Shared.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Features.TodoLists.Commands.CreateTodoList;

public class CreateTodoListCommand : IRequest<Result<Guid>>
{
    public string? Title { get; set; }
    public string? Colour { get; set; }
}

public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    private readonly IStringLocalizer<CreateTodoListCommandValidator> _localizer;

    public CreateTodoListCommandValidator(IStringLocalizer<CreateTodoListCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.create.title.required"])
            .MaximumLength(200).WithMessage(_localizer["api.todo-lists.create.title.maxlength"]);

        RuleFor(v => v.Colour)
            .Must(BeValidColour).WithMessage(_localizer["api.todo-lists.create.colour.invalid"])
            .When(v => !string.IsNullOrWhiteSpace(v.Colour));
    }

    private static bool BeValidColour(string? colour)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(colour))
                return true;

            _ = Colour.From(colour);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList
        {
            Title = request.Title,
            Colour = string.IsNullOrWhiteSpace(request.Colour) ? Colour.White : Colour.From(request.Colour)
        };

        _context.TodoLists.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
