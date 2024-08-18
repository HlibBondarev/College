namespace College.BLL.Services.Memento.Interfaces;

public interface IMementoService<T>
{
    T? State { get; set; }
    INarrowMemento CreateMemento(string key);
    void SetMemento(INarrowMemento? memento);
}
