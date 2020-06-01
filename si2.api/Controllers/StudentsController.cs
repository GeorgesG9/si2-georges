using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using si2.bll.Dtos.Requests.Account;
using si2.bll.Helpers.ResourceParameters;
using si2.bll.Dtos.Results.Student;
using si2.bll.Services;
using si2.dal.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using si2.bll.Dtos.Requests.Student;
using Microsoft.AspNetCore.JsonPatch;
using si2.common;
using Newtonsoft.Json;

namespace si2.api.Controllers
{
    [ApiController]
    [Route("api/students")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class StudentsController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly ILogger<DataflowsController> _logger;
        private readonly IStudentService _studentService;

        public StudentsController(LinkGenerator linkGenerator, ILogger<DataflowsController> logger, IStudentService studentService)
        {
            _linkGenerator = linkGenerator;
            _logger = logger;
            _studentService = studentService;
        }

        [HttpGet(Name = "GetStudents")]
       [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetStudents([FromQuery] StudentResourceParameters pagedResourceParameters, CancellationToken ct)
        {
            var studentDtos = await _studentService.GetStudentsAsync(pagedResourceParameters,ct);

            var previousPageLink = studentDtos.HasPrevious ? CreateDataflowsResourceUri(pagedResourceParameters, Enums.ResourceUriType.PreviousPage) : null;
            var nextPageLink = studentDtos.HasNext ? CreateDataflowsResourceUri(pagedResourceParameters, Enums.ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = studentDtos.TotalCount,
                pageSize = studentDtos.PageSize,
                currentPage = studentDtos.CurrentPage,
                totalPages = studentDtos.TotalPages,
                previousPageLink,
                nextPageLink
            };

            if (studentDtos == null)
                return NotFound();

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(studentDtos);
        }

        [HttpGet("{id}", Name =  "GetStudent")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetStudent(Guid id, CancellationToken ct)
        {
            var studentDto = await _studentService.GetStudentByIdAsync(id, ct);

            if (studentDto == null)
                return NotFound();

            return Ok(studentDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StudentDto))]
        public async Task<ActionResult> CreateStudent([FromBody] CreateStudentDto createStudentDto, CancellationToken ct)
        {
            var studentToReturn = await _studentService.CreateStudentAsync(createStudentDto, ct);
            if (studentToReturn == null)
                return BadRequest();

            return CreatedAtRoute("GetStudent", new { id = studentToReturn.Id }, studentToReturn);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStudent([FromRoute]Guid id, [FromBody] UpdateStudentDto updateStudentDto, CancellationToken ct)
        {
            if (!await _studentService.ExistsAsync(id, ct))
                return NotFound();

            var studentToReturn = await _studentService.UpdateStudentAsync(id, updateStudentDto, ct);
            if (studentToReturn == null)
                return BadRequest();

            return Ok(studentToReturn);
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateStudent([FromRoute]Guid id, [FromBody] JsonPatchDocument<UpdateStudentDto> patchDoc, CancellationToken ct)
        {
            if (!await _studentService.ExistsAsync(id, ct))
                return NotFound();

            var studentToPatch = await _studentService.GetUpdateStudentDto(id, ct);
            patchDoc.ApplyTo(studentToPatch, ModelState);

            TryValidateModel(studentToPatch);


            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            var studentToReturn = await _studentService.PartialUpdateStudentAsync(id, studentToPatch, ct);
            if (studentToReturn == null)
                return BadRequest();

            return Ok(studentToReturn);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteStudent(Guid id, CancellationToken ct)
        {
            await _studentService.DeleteStudentByIdAsync(id, ct);

            return NoContent();
        }




        private string CreateDataflowsResourceUri(StudentResourceParameters pagedResourceParameters, Enums.ResourceUriType type)
        {
            switch (type)
            {
                case Enums.ResourceUriType.PreviousPage:
                    return _linkGenerator.GetUriByName(this.HttpContext, "GetStudents",
                        new
                        {
                            status = pagedResourceParameters.Status,
                            searchQuery = pagedResourceParameters.SearchQuery,
                            pageNumber = pagedResourceParameters.PageNumber - 1,
                            pageSize = pagedResourceParameters.PageSize
                        }); // TODO get the aboslute path 
                case Enums.ResourceUriType.NextPage:
                    return _linkGenerator.GetUriByName(this.HttpContext, "GetStudents",
                        new
                        {
                            status = pagedResourceParameters.Status,
                            searchQuery = pagedResourceParameters.SearchQuery,
                            pageNumber = pagedResourceParameters.PageNumber + 1,
                            pageSize = pagedResourceParameters.PageSize
                        });
                default:
                    return _linkGenerator.GetUriByName(this.HttpContext, "GetStudents",
                       new
                       {
                           status = pagedResourceParameters.Status,
                           searchQuery = pagedResourceParameters.SearchQuery,
                           pageNumber = pagedResourceParameters.PageNumber,
                           pageSize = pagedResourceParameters.PageSize
                       });
            }
        }

    }
}
