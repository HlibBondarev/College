namespace College.BLL.Services.Memento.Interfaces;

public interface INarrowMemento
{
    KeyValuePair<string, string> State { get; set; }
}