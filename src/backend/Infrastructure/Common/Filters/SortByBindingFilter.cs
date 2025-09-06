using Microsoft.AspNetCore.Mvc.Filters;
using EvrenDev.Application.Common.Models;

namespace EvrenDev.Infrastructure.Common.Filters;

public class SortByBindingFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;

        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (typeof(PaginationFilter).IsAssignableFrom(parameter.ParameterType))
            {
                var filterObject = context.ActionArguments[parameter.Name];
                if (filterObject is PaginationFilter filter)
                {
                    var sortByItems = new List<SortBy>();

                    var sortByKeys = request.Query.Keys.Where(k => k.StartsWith("sortBy[") && k.EndsWith("][key]")).ToList();

                    foreach (var keyParam in sortByKeys)
                    {
                        var indexStr = keyParam.Substring(7, keyParam.IndexOf(']') - 7);
                        if (int.TryParse(indexStr, out int index))
                        {
                            var keyValue = request.Query[keyParam].FirstOrDefault();
                            var orderValue = request.Query[$"sortBy[{index}][order]"].FirstOrDefault();

                            if (!string.IsNullOrEmpty(keyValue))
                            {
                                sortByItems.Add(new SortBy
                                {
                                    Key = keyValue,
                                    Order = orderValue
                                });
                            }
                        }
                    }

                    if (sortByItems.Any())
                    {
                        filter.SortBy = sortByItems;
                    }
                }
            }
        }

        base.OnActionExecuting(context);
    }
}
