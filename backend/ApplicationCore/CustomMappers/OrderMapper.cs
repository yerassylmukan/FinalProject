﻿using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace ApplicationCore.CustomMappers;

public static class OrderMapper
{
    public static OrderDTO MapToDTO(this Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            UserId = order.UserId,
            IsConfirmed = order.IsConfirmed,
            ShippingMethod = order.ShippingMethod,
            ShippingDetails = order.ShippingDetails,
            Items = order.Items.Select(oi => oi.MapToDTO()).ToList()
        };
    }
}