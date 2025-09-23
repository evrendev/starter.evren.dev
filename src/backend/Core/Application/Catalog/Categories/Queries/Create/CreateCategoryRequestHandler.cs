using EvrenDev.Application.Catalog.Categories.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Queries.Create;

public class CreateCategoryRequest : IRequest<Guid>
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateCategoryRequestValidator : CustomValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator(IReadRepository<Category> repository,
        IStringLocalizer<CreateCategoryRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) =>
                await repository.FirstOrDefaultAsync(new CategoryByTitleSpec(name), ct) is null)
            .WithMessage((_, title) => string.Format(localizer["catalog.categories.create.alreadyexists"], title));
    }
}

public class CreateCategoryRequestHandler
    (IRepositoryWithEvents<Category> repository) : IRequestHandler<CreateCategoryRequest, Guid>
{
    public async Task<Guid> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Title, request.Description);

        await repository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}
