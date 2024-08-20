namespace College.BLL.Services.Memento.Interfaces;

public interface IWideMemento : INarrowMemento
{
    KeyValuePair<string, string?> State { get; set; }
}
