namespace KongTutorialBackend;

public interface IItemWithKey
{
    int Id { get; set; }
}

public class TodoItem : IItemWithKey
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public bool Done { get; set; }
}