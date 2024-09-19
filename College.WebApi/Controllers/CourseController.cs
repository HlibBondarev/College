using Microsoft.AspNetCore.Mvc;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Create;
using College.BLL.MediatR.Course.Update;
using College.BLL.MediatR.Course.Delete;
using College.BLL.MediatR.Course.GetAll;
using College.BLL.MediatR.Course.GetById;
using Microsoft.AspNetCore.Authorization;

namespace College.WebApi.Controllers;

[Authorize]
public class CourseController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllCoursesQuery()));
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetByIdCourseQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateCourseCommand(createRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCourseRequestDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateCourseCommand(updateRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCourseRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteCourseCommand(deleteRequest)));
    }
}