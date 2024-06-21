using AutoMapper;
using College.BLL.DTO.Teachers;
using College.DAL.Entities;

namespace College.BLL.Mapping;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<CreateTeacherRequestDto, Teacher>();
        CreateMap<Teacher, CreateTeacherResponseDto>();
        CreateMap<UpdateTeacherRequestDto, Teacher>();
        CreateMap<Teacher, UpdateTeacherResponseDto>();
        CreateMap<Teacher, GetAllTeachersResponseDto>();
        CreateMap<Teacher, GetByIdTeacherResponseDto>();
    }
}
