﻿namespace ApplicationCore.Entities.CatalogAggregate;

public class CatalogType : BaseEntity
{
    public string Type { get; private set; }

    public ICollection<CatalogItem> CatalogItems { get; private set; } = new List<CatalogItem>();

    public CatalogType(string type)
    {
        Type = type;
    }
}