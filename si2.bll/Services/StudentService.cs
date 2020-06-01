using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using si2.bll.Dtos.Requests.Student;
using si2.bll.Dtos.Results.Student;
using si2.bll.Helpers.PagedList;
using si2.bll.Helpers.ResourceParameters;
using si2.dal.Entities;
using si2.dal.UnitOfWork;
using Si2.common.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static si2.common.Enums;

namespace si2.bll.Services
{
    public class StudentService : ServiceBase, IStudentService
    {
        public StudentService(IUnitOfWork uow, IMapper mapper, ILogger<StudentService> logger) : base(uow, mapper, logger)
        {
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto, CancellationToken ct)
        {
            StudentDto studentDto = null;
            try
            {
                var studentEntity = _mapper.Map<Student>(createStudentDto);
                await _uow.Students.AddAsync(studentEntity, ct);
                //  await _uow.SaveChangesAsync(ct);
                var result = await _uow.SaveChangesAsync(ct);
                if (result  == -1)
                    return null;

                studentDto = _mapper.Map<StudentDto>(studentEntity);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, string.Empty);

            }
            return studentDto;
        }

        public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto, CancellationToken ct)
        {
            StudentDto studentDto = null;

            var updatedEntity = _mapper.Map<Student>(updateStudentDto);
            updatedEntity.Id = id;
            await _uow.Students.UpdateAsync(updatedEntity, id, ct, updatedEntity.RowVersion);
            await _uow.SaveChangesAsync(ct);
            var studentEntity = await _uow.Students.GetAsync(id, ct);
            studentDto = _mapper.Map<StudentDto>(studentEntity);

            return studentDto;
        }

        public async Task<StudentDto> PartialUpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto, CancellationToken ct)
        {
            var studentEntity = await _uow.Students.GetAsync(id, ct);

            _mapper.Map(updateStudentDto, studentEntity);

            await _uow.Students.UpdateAsync(studentEntity, id, ct, studentEntity.RowVersion);
            await _uow.SaveChangesAsync(ct);

            studentEntity = await _uow.Students.GetAsync(id, ct);
            var studentDto = _mapper.Map<StudentDto>(studentEntity);

            return studentDto;
        }

        public async Task<UpdateStudentDto> GetUpdateStudentDto(Guid id, CancellationToken ct)
        {
            var studentEntity = await _uow.Students.GetAsync(id, ct);
            var updateStudentDto = _mapper.Map<UpdateStudentDto>(studentEntity);
            return updateStudentDto;
        }

        public async Task<StudentDto> GetStudentByIdAsync(Guid id, CancellationToken ct)
        {
            StudentDto studentDto = null;

            var studentEntity = await _uow.Students.GetAsync(id, ct);
            if (studentEntity != null)
            {
                studentDto = _mapper.Map<StudentDto>(studentEntity);
            }

            return studentDto;
        }

        public async Task DeleteStudentByIdAsync(Guid id, CancellationToken ct)
        {
            try
            {
                var studentEntity = await _uow.Students.FirstAsync(c => c.Id == id, ct);
                _uow.Students.Delete(studentEntity);
                await _uow.SaveChangesAsync(ct);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, string.Empty);
            }
        }

        public async Task<PagedList<StudentDto>> GetStudentsAsync(StudentResourceParameters resourceParameters, CancellationToken ct)
        {
            var studentEntities = _uow.Students.GetAll();

            /*if (!string.IsNullOrEmpty(resourceParameters.Status))
            {
                if (Enum.TryParse(resourceParameters.Status, true, out DataflowStatus status))
                {
                    studentEntities = studentEntities.Where(a => a.Status == status);
                }
            }*/

            if (!string.IsNullOrEmpty(resourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = resourceParameters.SearchQuery.Trim().ToLowerInvariant();
                studentEntities = studentEntities
                    .Where(a => a.Code.ToLowerInvariant().Contains(searchQueryForWhereClause)
                            || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                            || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            var pagedListEntities = await PagedList<Student>.CreateAsync(studentEntities,
                resourceParameters.PageNumber, resourceParameters.PageSize, ct);

            var result = _mapper.Map<PagedList<StudentDto>>(pagedListEntities);
            result.TotalCount = pagedListEntities.TotalCount;
            result.TotalPages = pagedListEntities.TotalPages;
            result.CurrentPage = pagedListEntities.CurrentPage;
            result.PageSize = pagedListEntities.PageSize;

            return result;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        {
            if (await _uow.Students.GetAsync(id, ct) != null)
                return true;

            return false;
        }
    }
}
