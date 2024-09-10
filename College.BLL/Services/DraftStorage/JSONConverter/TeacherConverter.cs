using College.BLL.DTO.Teachers.Drafts;
using Newtonsoft.Json.Linq;

namespace College.BLL.Services.DraftStorage.JSONConverter;

public class TeacherConverter : JsonCreationConverter<TeacherWithNameDto>
{
    protected override TeacherWithNameDto Create(Type objectType, JObject jObject)
    {
        ArgumentNullException.ThrowIfNull(objectType);
        ArgumentNullException.ThrowIfNull(jObject);

        if (jObject["degree"] != null)
        {
            return new TeacherWithDegreeDto();
        }
        else
        {
            return new TeacherWithNameDto();
        }
    }
}
