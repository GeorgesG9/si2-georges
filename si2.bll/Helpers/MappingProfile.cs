 using AutoMapper;
using si2.bll.Dtos.Requests.Dataflow;
using si2.bll.Dtos.Results.Dataflow;
using si2.bll.Dtos.Requests.Student;
using si2.bll.Dtos.Results.Student;
using si2.bll.Helpers.PagedList;
using si2.dal.Entities;
using System.Linq;
using System;

namespace si2.bll.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateDataflowDto, Dataflow>();
            CreateMap<UpdateDataflowDto, Dataflow>();
            CreateMap<Dataflow, DataflowDto>();
            CreateMap<Dataflow, UpdateDataflowDto>();

            //STUDENT
            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();
            CreateMap<Student, StudentDto>()
                .ForMember(dest=> dest.Name, opt=> opt.MapFrom(src=> $"{ src.FirstName } , { src.LastName}"));
            CreateMap<Student, UpdateStudentDto>();

            /* CreateMap<StudentDto, CreateStudentDto>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src=> $"{ src.Name.Substring(0, src.Name.IndexOf(","))  }"))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => $"{ src.Name.Substring(src.Name.LastIndexOf(" ")+1, src.Name.Length )  }"))
                ;*/

        }
    }
}
