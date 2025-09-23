using EvrenDev.Application.Catalog.Categories.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Queries.Update;

public class UpdateCategoryRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}

public class UpdateCategoryRequestValidator : CustomValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator(IRepository<Category> repository,
        IStringLocalizer<UpdateCategoryRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (category, title, ct) =>
                await repository.FirstOrDefaultAsync(new CategoryByTitleSpec(title), ct)
                    is not Category existingCategory || existingCategory.Id == category.Id)
            .WithMessage((_, name) => string.Format(localizer["catalog.categories.update.alreadyexists"], name));
    }
}

public class UpdateCategoryRequestHandler(IRepositoryWithEvents<Category> repository,
        IStringLocalizer<UpdateCategoryRequestHandler> localizer)
    : IRequestHandler<UpdateCategoryRequest, Guid>
{
    public async Task<Guid> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = category ??
            throw new NotFoundException(string.Format(localizer["catalog.categories.update.notfound"], request.Id));

        category.Update(request.Title, request.Description);

        await repository.UpdateAsync(category, cancellationToken);

        return request.Id;
    }
}
