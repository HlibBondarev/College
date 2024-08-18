using College.BLL.Services.Memento.Interfaces;
using Newtonsoft.Json;

namespace College.BLL.Services.Memento;

public class Memento<T> : IWideMemento<T>
{
    public KeyValuePair<string, string> State { get; set; }

    public T? GetState()
    {
        return JsonConvert.DeserializeObject<T>(State.Value); ;
    }

    public void SetState(T value, string key)
    {
        State = new KeyValuePair<string, string>(string.Format("{0}_{1}", key, typeof(T).Name), JsonConvert.SerializeObject(value));
    }
}
