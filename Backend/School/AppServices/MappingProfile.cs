using AutoMapper;
using Dtos.Course;
using DTOs.Classes;
using Entities.Classes;
using Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public class MappingProfile : Profile
    {

        public MappingProfile() {

            CreateMap<ClassCreateDto, Class>();
            CreateMap<Class, ClassResponseDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FirstName + src.Teacher.LastName))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));


            CreateMap<Class, ClassPatchDto>().ReverseMap();

            /*
             * 
             * 
             * CreateMap<Class, ClassPatchDto>();
               CreateMap<ClassPatchDto, Class>();
            */

            CreateMap<CourseCreateDto, Course>();
            CreateMap<Course, CourseResponseDto>();


            CreateMap<Course, CoursePatchDto>().ReverseMap();

        }
    }
}
