﻿namespace WebApi.Models;

public class UpdateReviewModel
{
    public string? UserId { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
}