using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StructuredAttributeTutorial.Services;

public interface IDocumentStore
{
    string CreateDocument();
    IEnumerable<string> GetDocumentIds();
}

public class DocumentStore : IDocumentStore
{
    private int nextDocumentId = 0;
    private readonly List<string> documents = new();
    
    public string CreateDocument()
    {
        string id = (++nextDocumentId).ToString();
        documents.Add(id);
        return id;
    }

    public IEnumerable<string> GetDocumentIds()
    {
        return new ReadOnlyCollection<string>(documents);
    }
}