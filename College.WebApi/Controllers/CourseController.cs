using Microsoft.AspNetCore.Mvc;
using College.BLL.DTO.Courses;
using College.BLL.MediatR.Course.Create;

namespace College.WebApi.Controllers;

public class CourseController : BaseApiController
{
    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    return HandleResult(await Mediator.Send(new GetAllTeachersQuery()));
    //}

    //[HttpGet("{id:Guid}")]
    //public async Task<IActionResult> GetById([FromRoute] Guid id)
    //{
    //    return HandleResult(await Mediator.Send(new GetByIdTeacherQuery(id)));
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateCourseCommand(createRequest)));
    }

    //[HttpPut]
    //public async Task<IActionResult> Update([FromBody] UpdateTeacherRequestDto updateRequest)
    //{
    //    return HandleResult(await Mediator.Send(new UpdateTeacherCommand(updateRequest)));
    //}

    //[HttpDelete]
    //public async Task<IActionResult> Delete([FromBody] DeleteTeacherRequestDto deleteRequest)
    //{
    //    return HandleResult(await Mediator.Send(new DeleteTeacherCommand(deleteRequest)));
    //}
}