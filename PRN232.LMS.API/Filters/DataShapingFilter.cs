using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;

namespace PRN232.LMS.API.Filters;

public class DataShapingFilter : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        Console.WriteLine($"\n=== [DataShapingFilter] Start processing request: {context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString} ===");

        // Only process successful ObjectResult responses
        if (context.Result is not ObjectResult objectResult)
        {
            Console.WriteLine($"[DataShapingFilter] Result is not ObjectResult (it is {context.Result?.GetType().Name ?? "null"}). Skipping.");
            return;
        }

        if (objectResult.Value == null)
        {
            Console.WriteLine("[DataShapingFilter] ObjectResult.Value is null. Skipping.");
            return;
        }

        // Get fields from query string case-insensitively
        var fieldsEntry = context.HttpContext.Request.Query
            .FirstOrDefault(q => string.Equals(q.Key, "fields", StringComparison.OrdinalIgnoreCase));
        var fieldsQuery = fieldsEntry.Value.ToString();
        
        Console.WriteLine($"[DataShapingFilter] Found fields query parameter: '{fieldsQuery}'");

        if (string.IsNullOrWhiteSpace(fieldsQuery))
        {
            Console.WriteLine("[DataShapingFilter] fields query parameter is empty. Skipping.");
            return;
        }

        var fields = fieldsQuery.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(f => f.Trim())
                                .ToList();

        if (!fields.Any())
        {
            Console.WriteLine("[DataShapingFilter] No valid fields after splitting. Skipping.");
            return;
        }

        var value = objectResult.Value;
        var valueType = value.GetType();
        Console.WriteLine($"[DataShapingFilter] Response value type: {valueType.FullName}");

        // Check if value is ApiResponse<T>
        if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(PRN232.LMS.Services.Models.Response.ApiResponse<>))
        {
            Console.WriteLine("[DataShapingFilter] Matching ApiResponse<T>");
            var successProp = valueType.GetProperty("Success")?.GetValue(value);
            var messageProp = valueType.GetProperty("Message")?.GetValue(value);
            var errorsProp = valueType.GetProperty("Errors")?.GetValue(value);
            var dataProp = valueType.GetProperty("Data")?.GetValue(value);

            var shapedData = ShapeData(dataProp, fields);

            objectResult.Value = new
            {
                Success = successProp,
                Message = messageProp,
                Data = shapedData,
                Errors = errorsProp
            };
            Console.WriteLine("[DataShapingFilter] Successfully shaped ApiResponse data!");
        }
        // Check if value is PagedResponse<T>
        else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(PRN232.LMS.Services.Models.Response.PagedResponse<>))
        {
            Console.WriteLine("[DataShapingFilter] Matching PagedResponse<T>");
            var successProp = valueType.GetProperty("Success")?.GetValue(value);
            var messageProp = valueType.GetProperty("Message")?.GetValue(value);
            var errorsProp = valueType.GetProperty("Errors")?.GetValue(value);
            var paginationProp = valueType.GetProperty("Pagination")?.GetValue(value);
            var dataProp = valueType.GetProperty("Data")?.GetValue(value);

            var shapedData = ShapeData(dataProp, fields);

            objectResult.Value = new
            {
                Success = successProp,
                Message = messageProp,
                Data = shapedData,
                Pagination = paginationProp,
                Errors = errorsProp
            };
            Console.WriteLine("[DataShapingFilter] Successfully shaped PagedResponse data!");
        }
        else
        {
            Console.WriteLine("[DataShapingFilter] Response type does not match ApiResponse<T> or PagedResponse<T>. Skipping.");
        }
        Console.WriteLine("=== [DataShapingFilter] End processing ===\n");
    }

    private object? ShapeData(object? data, List<string> fields)
    {
        if (data == null) return null;

        // If it's a collection, shape each element
        if (data is IEnumerable enumerable && data is not string)
        {
            var list = new List<object>();
            foreach (var item in enumerable)
            {
                if (item != null)
                {
                    var shapedItem = ShapeSingleObject(item, fields);
                    list.Add(shapedItem);
                }
            }
            return list;
        }

        return ShapeSingleObject(data, fields);
    }

    private object ShapeSingleObject(object obj, List<string> fields)
    {
        var shapedObject = new ExpandoObject() as IDictionary<string, object?>;
        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // 1. Loop through all properties of the original DTO object
        foreach (var prop in properties)
        {
            var camelCaseName = JsonNamingPolicy.CamelCase.ConvertName(prop.Name);
            
            // Check if this property was requested in the fields parameter (case-insensitive)
            var isRequested = fields.Any(f => string.Equals(f, prop.Name, StringComparison.OrdinalIgnoreCase));
            if (isRequested)
            {
                shapedObject[camelCaseName] = prop.GetValue(obj);
            }
            else
            {
                // If not requested, set to null
                shapedObject[camelCaseName] = null;
            }
        }

        // 2. Also, if there are custom fields requested that are NOT properties of the DTO, add them as null
        foreach (var field in fields)
        {
            var exists = properties.Any(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));
            if (!exists)
            {
                var camelCaseName = JsonNamingPolicy.CamelCase.ConvertName(field);
                shapedObject[camelCaseName] = null;
            }
        }

        return shapedObject;
    }
}
