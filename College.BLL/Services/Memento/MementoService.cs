using College.BLL.Services.Memento.Interfaces;

namespace College.BLL.Services.Memento;

public class MementoService<T> : IMementoService<T>
{
    public T? State { get; set; }

    public void SetMemento(INarrowMemento? memento)
    {
        if (memento is IWideMemento<T> m)
        {
            State = m.GetState();
        }
    }

    public INarrowMemento CreateMemento(string key)
    {
        var memento = new Memento<T>();
        if (State is not null)
        {
            memento.SetState(State, key);
        }
        return memento;
    }
}