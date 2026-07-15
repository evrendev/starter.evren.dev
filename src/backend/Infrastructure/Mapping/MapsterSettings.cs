using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Infrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping

        // This one is actually not necessary as it's mapped by convention
        // TypeAdapterConfig<Product, ProductDto>.NewConfig().Map(dest => dest.BrandName, src => src.Brand.Name);

        // NoteDto.CreatedOn deliberately comes from LastModifiedOn: the entity's
        // CreatedOn is get-only without a DB column and would evaluate to "now"
        TypeAdapterConfig<Note, NoteDto>.NewConfig()
            .Map(dest => dest.CreatedOn, src => src.LastModifiedOn);
    }
}
