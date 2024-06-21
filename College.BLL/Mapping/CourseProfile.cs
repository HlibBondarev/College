using AutoMapper;
using College.BLL.DTO.Courses;
using College.BLL.DTO.Teachers;
using College.DAL.Entities;

namespace College.BLL.Mapping;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, TeacherCoursesDto>();
        CreateMap<CreateCourseRequestDto, Course>();
        CreateMap<Course, CreateCourseResponseDto>();
    }
}