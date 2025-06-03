using Microsoft.AspNetCore.Mvc;

namespace KongTutorialBackend;

[Route("api")]
public class TodoController : Controller
{
    private readonly DataStore _store;

    public TodoController(DataStore store)
    {
        _store = store;
    }

    [HttpGet]
    [Route("todos")]
    public IActionResult GetAll()
    {
        return Ok(_store.Items);
    }

    [HttpGet]
    [Route("todo/{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var item = _store.Get(id);

        if (item == null) return NotFound();

        return Ok(item);
    }

    [HttpPost]
    [Route("todo")]
    public IActionResult Create([FromBody] TodoItem item)
    {
        int id = _store.Add(item);
        
        return Ok(new
        {
            Id = id
        });
    }

    [HttpPut]
    [Route("todo")]
    public IActionResult Update([FromBody] TodoItem item)
    {
        bool exists = _store.TryUpdate(item);
        
        return exists ? Ok() : NotFound();
    }

    [HttpDelete]
    [Route("todo/{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        _store.Remove(id);

        return Ok();
    }
    
    [HttpPatch]
    [Route("todo/{id}")]
    public IActionResult Complete([FromRoute] int id)
    {
        _store.Complete(id);

        return Ok();
    }
}