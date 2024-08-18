namespace College.BLL.Services.Memento.Interfaces;

public interface IStorage<T>
{
    INarrowMemento? this[string key] { get; set; }
    void DeleteMemento(INarrowMemento? memento);
}