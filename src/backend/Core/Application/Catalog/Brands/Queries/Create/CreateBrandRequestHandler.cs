using EvrenDev.Application.Catalog.Brands.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Queries.Create;

public class CreateBrandRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateBrandRequestValidator : CustomValidator<CreateBrandRequest>
{
    public CreateBrandRequestValidator(IReadRepository<Brand> repository, IStringLocalizer<CreateBrandRequestValidator> localizer) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new BrandByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["brand.alreadyexists"], name));
}

public class CreateBrandRequestHandler(IRepositoryWithEvents<Brand> repository) : IRequestHandler<CreateBrandRequest, Guid>
{
    public async Task<Guid> Handle(CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var brand = new Brand(request.Name, request.Description);

        await repository.AddAsync(brand, cancellationToken);

        return brand.Id;
    }
}
