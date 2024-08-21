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

namespace College.WebApi.Controllers;

//[Authorize]
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
    public IActionResult StoreMemento(string userId, [FromBody] TeacherMemento teacherMemento)
    {
        var memento = _mementoService.CreateMemento(userId, teacherMemento);
        _storage.RedisCacheService = _redisCacheService;
        _storage[userId] = memento.State;
        return Ok(string.Format("{0} is stored", typeof(TeacherMemento).Name));
    }

    [HttpGet]
    public IActionResult RestoreMemento(string userId)
    {
        _storage.RedisCacheService = _redisCacheService;
        _mementoService.RestoreMemento(_storage[userId]);
        return Ok(_mementoService.State);
    }

    [HttpGet]
    public IActionResult RemoveMemento(string userId)
    {
        _storage.RedisCacheService = _redisCacheService;
        _storage.RemoveMemento(userId);
        return Ok(string.Format("{0} deletion process is completed", typeof(TeacherMemento).Name));
    }
}
