using Ardalis.GuardClauses;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Domain.Entities.Catalog;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Features.TodoLists.Commands.DeleteTodoList;

public class DeleteTodoListCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteTodoListCommandValidator : AbstractValidator<DeleteTodoListCommand>
{
    private readonly IStringLocalizer<DeleteTodoListCommandValidator> _localizer;

    public DeleteTodoListCommandValidator(IStringLocalizer<DeleteTodoListCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);
    }
}

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<DeleteTodoListCommandHandler> _localizer;

    public DeleteTodoListCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<DeleteTodoListCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists.FindAsync([request.Id], cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        _context.TodoLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
