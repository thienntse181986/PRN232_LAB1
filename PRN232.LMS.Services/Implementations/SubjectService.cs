using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResponse<SubjectResponse>> GetAllAsync(QueryParameters query)
    {
        var q = _unitOfWork.Subjects.GetQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(x => x.SubjectName.ToLower().Contains(s) || x.SubjectCode.ToLower().Contains(s));
        }

        q = QueryHelper.ApplySorting(q, query.Sort);
        var (items, total) = await QueryHelper.ApplyPagingAsync(q, query.Page, query.Size);

        return PagedResponse<SubjectResponse>.Ok(items.Select(MapToResponse), new PaginationMeta
        {
            Page = query.Page, PageSize = query.Size, TotalItems = total
        });
    }

    public async Task<ApiResponse<SubjectResponse>> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        if (entity is null) return ApiResponse<SubjectResponse>.Fail($"Subject with ID {id} not found.");
        return ApiResponse<SubjectResponse>.Ok(MapToResponse(entity));
    }

    public async Task<ApiResponse<SubjectResponse>> CreateAsync(SubjectCreateRequest request)
    {
        var entity = new Subject
        {
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            Credit = request.Credit
        };
        await _unitOfWork.Subjects.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<SubjectResponse>.Ok(MapToResponse(entity), "Subject created successfully.");
    }

    public async Task<ApiResponse<SubjectResponse>> UpdateAsync(int id, SubjectUpdateRequest request)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        if (entity is null) return ApiResponse<SubjectResponse>.Fail($"Subject with ID {id} not found.");

        entity.SubjectCode = request.SubjectCode;
        entity.SubjectName = request.SubjectName;
        entity.Credit = request.Credit;
        _unitOfWork.Subjects.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<SubjectResponse>.Ok(MapToResponse(entity), "Subject updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        if (entity is null) return ApiResponse<bool>.Fail($"Subject with ID {id} not found.");
        
        _unitOfWork.Subjects.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.Ok(true, "Subject deleted successfully.");
    }

    private static SubjectResponse MapToResponse(Subject s) => new()
    {
        SubjectId = s.SubjectId,
        SubjectCode = s.SubjectCode,
        SubjectName = s.SubjectName,
        Credit = s.Credit
    };
}
