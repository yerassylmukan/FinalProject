﻿using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace ApplicationCore.CustomMappers;

public static class CatalogItemMapper
{
    public static CatalogItemDTO MapToDTO(this CatalogItem catalogItem)
    {
        return new CatalogItemDTO
        {
            Id = catalogItem.Id,
            Name = catalogItem.Name,
            Description = catalogItem.Description,
            Price = catalogItem.Price,
            PictureUrl = catalogItem.PictureUrl,
            StockQuantity = catalogItem.StockQuantity,
            CatalogItemTypeName = catalogItem.CatalogType.Type,
            CatalogItemBrandName = catalogItem.CatalogBrand.Brand,
            CatalogTypeId = catalogItem.CatalogTypeId,
            CatalogBrandId = catalogItem.CatalogBrandId,
            Reviews = catalogItem.Reviews.Select(r => r.MapToDTO()).ToList()
        };
    }
}