namespace KongTutorialBackend;

public class DataStore
{
    private readonly Dictionary<int, TodoItem> _items = new();

    public IEnumerable<TodoItem> Items => _items.Values;

    public TodoItem? Get(int id)
    {
        return _items.GetValueOrDefault(id);
    }

    public int Add(TodoItem item)
    {
        if (item.Id == 0)
        {
            if (_items.Count == 0) 
            {
                item.Id = 1;
            }
            else
            {
                item.Id = _items.Keys.Max() + 1;
            }
        }
        
        _items.Add(item.Id, item);
        return item.Id;
    }

    public void Remove(int id)
    {
        _items.Remove(id);
    }

    public bool TryUpdate(TodoItem item)
    {
        if (!_items.ContainsKey(item.Id)) return false;
        
        _items[item.Id] = item;
        return true;
    }

    public void Complete(int id)
    {
        if (_items.TryGetValue(id, out var item))
        {
            item.Done = true;
        }
    }
}