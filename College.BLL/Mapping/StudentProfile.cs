using AutoMapper;
using College.BLL.DTO.Students;
using College.DAL.Entities;

namespace College.BLL.Mapping;

internal class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<CreateStudentRequestDto, Student>();
        CreateMap<Student, CreateStudentResponseDto>();
        CreateMap<UpdateStudentRequestDto, Student>();
        CreateMap<Student, UpdateStudentResponseDto>();
        CreateMap<Student, GetAllStudentsResponseDto>();
        CreateMap<Student, GetByIdStudentResponseDto>();
        CreateMap<Student, StudentDto>();
    }
}