using EvrenDev.Application.Catalog.Categories.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Queries.Create;

public class CreateCategoryRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateCategoryRequestValidator : CustomValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator(IReadRepository<Category> repository, IStringLocalizer<CreateCategoryRequestValidator> localizer) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new CategoryByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["catalog.categories.create.alreadyexists"], name));
}

public class CreateCategoryRequestHandler(IRepositoryWithEvents<Category> repository) : IRequestHandler<CreateCategoryRequest, Guid>
{
    public async Task<Guid> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name, request.Description);

        await repository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}
