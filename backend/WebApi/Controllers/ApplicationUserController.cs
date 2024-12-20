﻿using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin,BasicUser")]
[Route("api/[controller]/[action]")]
public class ApplicationUserController : ControllerBase
{
    private readonly IApplicationUserService _service;

    public ApplicationUserController(IApplicationUserService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ApplicationUserDTO>>> GetAllUsers(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return StatusCode(StatusCodes.Status499ClientClosedRequest, "Request was cancelled by client");

        return Ok(await _service.GetUsersAsync(cancellationToken));
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<ApplicationUserDTO>> GetUserDetailsByUserName(string userName,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return StatusCode(StatusCodes.Status499ClientClosedRequest, "Request was cancelled by client");

        return Ok(await _service.GetUserDetailsByUserNameAsync(userName));
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<ApplicationUserDTO>> GetUserDetailsByEmail(string email,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return StatusCode(StatusCodes.Status499ClientClosedRequest, "Request was cancelled by client");

        return Ok(await _service.GetUserDetailsByEmailAsync(email));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ApplicationUserDTO>> GetUserDetailsByUserId(string userId,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return StatusCode(StatusCodes.Status499ClientClosedRequest, "Request was cancelled by client");

        return Ok(await _service.GetUserDetailsByUserIdAsync(userId));
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfileInformation(string userId,
        [FromBody] UpdateProfileInformationModel model,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return StatusCode(StatusCodes.Status499ClientClosedRequest, "Request was cancelled by client");

        await _service.UpdateProfileInformationAsync(userId, model.FirstName!, model.LastName!,
            model.ProfilePictureUrl!);

        return Ok();
    }
}