using Microsoft.AspNetCore.Mvc;
using College.BLL.DTO.Teachers.Drafts;
using College.BLL.Services.DraftStorage.Interfaces;

namespace College.WebApi.Controllers;

/// <summary>Controller with operations for storing the Teacher draft in cache.</summary>
[ApiController]
[Route("[controller]/[action]")]
public class TeacherDraftStorageController(IDraftStorageService<TeacherWithNameDto> draftStorageService)
    : DraftStorageController<TeacherWithNameDto>(draftStorageService)
{
}