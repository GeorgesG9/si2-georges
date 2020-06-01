using Microsoft.AspNetCore.JsonPatch;
using si2.bll.Dtos.Requests.Student;
using si2.bll.Dtos.Results.Student;
using si2.bll.Helpers.PagedList;
using si2.bll.Helpers.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace si2.bll.Services
{
    public interface IStudentService : IServiceBase
    {
        Task<StudentDto> CreateStudentAsync(CreateStudentDto CreateStudentDto, CancellationToken ct);
        Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto, CancellationToken ct);
        Task<StudentDto> PartialUpdateStudentAsync(Guid id, UpdateStudentDto patchDoc, CancellationToken ct);
        Task<UpdateStudentDto> GetUpdateStudentDto(Guid id, CancellationToken ct);
        Task<StudentDto> GetStudentByIdAsync(Guid id, CancellationToken ct);
        Task DeleteStudentByIdAsync(Guid id, CancellationToken ct);
        Task<PagedList<StudentDto>> GetStudentsAsync(StudentResourceParameters pagedResourceParameters, CancellationToken ct);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    }
}
