namespace College.BLL.Services.Memento.Interfaces;

public interface IWideMemento<T> : INarrowMemento
{
    T? GetState();
    void SetState(T value, string key);
}
