using EvrenDev.Application.Catalog.Brands.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Queries.Update;

public class UpdateBrandRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class UpdateBrandRequestValidator : CustomValidator<UpdateBrandRequest>
{
    public UpdateBrandRequestValidator(IRepository<Brand> repository, IStringLocalizer<UpdateBrandRequestValidator> localizer) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (brand, name, ct) =>
                await repository.FirstOrDefaultAsync(new BrandByNameSpec(name), ct)
                    is not Brand existingBrand || existingBrand.Id == brand.Id)
            .WithMessage((_, name) => string.Format(localizer["brand.alreadyexists"], name));
}

public class UpdateBrandRequestHandler(IRepositoryWithEvents<Brand> repository, IStringLocalizer<UpdateBrandRequestHandler> localizer)
    : IRequestHandler<UpdateBrandRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents

    public async Task<Guid> Handle(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        var brand = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = brand ?? throw new NotFoundException(string.Format(localizer["brand.notfound"], request.Id));

        brand.Update(request.Name, request.Description);

        await repository.UpdateAsync(brand, cancellationToken);

        return request.Id;
    }
}
