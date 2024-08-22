using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using College.BLL.DTO.Teachers;
using College.BLL.MediatR.Teacher.Create;
using College.BLL.MediatR.Teacher.Delete;
using College.BLL.MediatR.Teacher.GetAll;
using College.BLL.MediatR.Teacher.Update;
using College.BLL.MediatR.Teacher.GetById;
using College.BLL.Services.Memento.Models;
using College.BLL.Services.Memento.Interfaces;
using College.Redis;
using College.BLL.Common;

namespace College.WebApi.Controllers;

[Authorize]
public class TeacherController : BaseApiController
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly IMementoService<TeacherMemento> _mementoService;
    private readonly IStorage _storage;


    public TeacherController(IRedisCacheService redisCacheService,
                             IMementoService<TeacherMemento> mementoService,
                             IStorage storage)
    {
        _redisCacheService = redisCacheService;
        _mementoService = mementoService;
        _storage = storage;
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTeachersQuery()));
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetByIdTeacherQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateTeacherCommand(createRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTeacherRequestDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateTeacherCommand(updateRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteTeacherRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteTeacherCommand(deleteRequest)));
    }

    [HttpPost]
    public async Task<IActionResult> StoreMemento([FromBody] TeacherMemento teacherMemento)
    {
        var userId = GettingUserProperties.GetUserId(User);
        var memento = _mementoService.CreateMemento(userId, teacherMemento);
        _storage.RedisCacheService = _redisCacheService;
        await _storage.SetMementoValueAsync(memento.State);
        return Ok(string.Format("{0} is stored", typeof(TeacherMemento).Name));
    }

    [HttpGet]
    public async Task<IActionResult> RestoreMemento()
    {
        var mementoKey = _mementoService.GetMementoKey(GettingUserProperties.GetUserId(User));
        _storage.RedisCacheService = _redisCacheService;
        _mementoService.RestoreMemento(await _storage.GetMementoValueAsync(mementoKey));
        return Ok(_mementoService.State);
    }

    [HttpGet]
    public async Task<IActionResult> RemoveMemento()
    {
        var mementoKey = _mementoService.GetMementoKey(GettingUserProperties.GetUserId(User));
        _storage.RedisCacheService = _redisCacheService;
        await _storage.RemoveMementoAsync(mementoKey);
        return Ok(string.Format("{0} deletion process is completed", typeof(TeacherMemento).Name));
    }
}
