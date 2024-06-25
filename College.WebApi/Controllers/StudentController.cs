using Microsoft.AspNetCore.Mvc;
using College.BLL.DTO.Students;
using College.BLL.MediatR.Student.Create;
using College.BLL.MediatR.Student.Update;
using College.BLL.MediatR.Student.Delete;
using College.BLL.MediatR.Student.GetAll;
using College.BLL.MediatR.Student.GetById;

namespace College.WebApi.Controllers;

public class StudentController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllStudentsQuery()));
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetByIdStudentQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateStudentCommand(createRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStudentRequestDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateStudentCommand(updateRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStudentRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteStudentCommand(deleteRequest)));
    }
}