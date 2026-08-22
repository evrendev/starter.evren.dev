using EvrenDev.Application.Catalog.CourseEnrollments.Entities;
using EvrenDev.Application.Catalog.Courses.Entities;
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

        // Task X1: ChapterCount expression is lifted verbatim from Task X0's live-SQL-verified
        // experiment — compiles to a single correlated COUNT subquery via Mapster's
        // ProjectToType(), not a client-side N+1 loop.
        TypeAdapterConfig<Course, CourseDto>.NewConfig()
            .Map(dest => dest.ChapterCount, src => src.Chapters!.Count(c => c.DeletedOn == null));

        // NextChapterId/NextChapterTitle: incomplete chapters (per this user's ChapterProgress)
        // sort before completed ones, ties broken by Order, so the first row is the earliest
        // not-yet-100%-complete chapter — or, if every chapter is done, the course's very first
        // chapter (Order 0), matching the ?? chapters[0].id fallback the frontend used to do
        // itself (see my-courses.vue/catalog.vue pre-X1). Only null when the course has zero
        // chapters. X0's original expression (Where + FirstOrDefault, no fallback) would have
        // regressed the "Review" button on a 100%-complete course — corrected here before
        // shipping, not carried over verbatim.
        TypeAdapterConfig<CourseEnrollment, CourseEnrollmentDto>.NewConfig()
            .Map(dest => dest.CategoryTitle, src => src.Course.Category.Title)
            .Map(dest => dest.ChapterCount, src => src.Course.Chapters!.Count(c => c.DeletedOn == null))
            .Map(dest => dest.NextChapterId, src => src.Course.Chapters!
                .OrderByDescending(c => !c.Progress.Any(p => p.UserId == src.UserId && p.PercentComplete >= 100))
                .ThenBy(c => c.Order)
                .Select(c => (Guid?)c.Id)
                .FirstOrDefault())
            .Map(dest => dest.NextChapterTitle, src => src.Course.Chapters!
                .OrderByDescending(c => !c.Progress.Any(p => p.UserId == src.UserId && p.PercentComplete >= 100))
                .ThenBy(c => c.Order)
                .Select(c => c.Title)
                .FirstOrDefault());
    }
}
