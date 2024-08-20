using College.BLL.Services.Memento.Interfaces;

namespace College.BLL.Services.Memento;

public class Memento : IWideMemento
{
    public KeyValuePair<string, string?> State { get; set; }
}
