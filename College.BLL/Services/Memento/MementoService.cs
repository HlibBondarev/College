using College.BLL.Services.Memento.Interfaces;
using Newtonsoft.Json;

namespace College.BLL.Services.Memento;

public class MementoService<T> : IMementoService<T>
{
    private readonly IWideMemento _wideMemento;

    public MementoService(IWideMemento wideMemento)
    {
        _wideMemento = wideMemento;
    }

    public T? State { get; set; }

    public void RestoreMemento(KeyValuePair<string, string?> memento)
    {
        State = JsonConvert.DeserializeObject<T>(memento.Value ?? string.Empty);
    }

    public IWideMemento CreateMemento(string key, T value)
    {
        var mementoKey = string.Format("{0}_{1}", key, typeof(T).Name);
        _wideMemento.State = new KeyValuePair<string, string?>(string.Format("{0}_{1}", key, typeof(T).Name), JsonConvert.SerializeObject(value));
        return _wideMemento;
    }

    public void DeleteMemento(string key)
    {
        var mementoKey = string.Format("{0}_{1}", key, typeof(T).Name);
        throw new NotImplementedException();
    }
}