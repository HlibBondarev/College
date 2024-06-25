using AutoMapper;
using College.BLL.DTO.Courses;
using College.DAL.Entities;

namespace College.BLL.Mapping;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CreateCourseRequestDto, Course>();
        CreateMap<Course, CreateCourseResponseDto>();
        CreateMap<UpdateCourseRequestDto, Course>();
        CreateMap<Course, UpdateCourseResponseDto>();
        CreateMap<Course, GetAllCoursesResponseDto>()
            .ForPath(dto => dto.TeacherName, conf => conf.MapFrom(course => course.Teacher!.Name));
        CreateMap<Course, GetByIdCourseResponseDto>()
            .ForPath(dto => dto.TeacherName, conf => conf.MapFrom(course => course.Teacher!.Name));
        CreateMap<Course, CourseDto>();
    }
}