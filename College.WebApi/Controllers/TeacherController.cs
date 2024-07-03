using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using College.BLL.DTO.Teachers;
using College.BLL.MediatR.Teacher.Create;
using College.BLL.MediatR.Teacher.Delete;
using College.BLL.MediatR.Teacher.GetAll;
using College.BLL.MediatR.Teacher.Update;
using College.BLL.MediatR.Teacher.GetById;

namespace College.WebApi.Controllers;

[Authorize]
public class TeacherController : BaseApiController
{
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
}
