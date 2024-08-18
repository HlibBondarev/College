using College.BLL.Services.Memento.Interfaces;

namespace College.BLL.Services.Memento;

public class Storage<T> : IStorage<T>
{
    private readonly Dictionary<string, string> _items = new Dictionary<string, string>();
    private readonly INarrowMemento _memento;

    public Storage(INarrowMemento memento)
    {
        _memento = memento;
    }

    public INarrowMemento? this[string key]
    {
        get
        {
            var itemKey = string.Format("{0}_{1}", key, typeof(T).Name);
            if (_items.TryGetValue(itemKey, out string? stateValue))
            {
                _memento.State = new KeyValuePair<string, string>(key, stateValue);
                return _memento;
            }
            return null;
        }
        set
        {
            var itemKey = string.Format("{0}_{1}", key, typeof(T).Name);
            if (_items.TryGetValue(itemKey, out string? stateValue))
            {
                _items[itemKey] = value!.State.Value;
            }
            else
            {
                _items.Add(value!.State.Key, value!.State.Value);
            }
        }
    }

    public void DeleteMemento(INarrowMemento? memento)
    {
        if (memento is null) return;

        if (_items.ContainsKey(memento.State.Key))
        {
            _items.Remove(memento.State.Key);
        }
    }
}
