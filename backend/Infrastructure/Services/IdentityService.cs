﻿using System.Net;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenClaimsService _tokenClaimsService;
    private readonly IEmailSender _emailSender;

    public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ITokenClaimsService tokenClaimsService, IEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenClaimsService = tokenClaimsService;
        _emailSender = emailSender;
    }

    public async Task<string> CreateUserAsync(string email, string password, string firstName, string lastName, string profilePictureUrl)
    {
        var userExists = await _userManager.Users.AnyAsync(u => u.Email == email);
        if (userExists) throw new UserAlreadyExistsException(email);

        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureUrl = profilePictureUrl
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "BasicUser");

        await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

        var token = await _tokenClaimsService.GetTokenAsync(user.UserName);
        return token;
    }

    public async Task<string> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email) 
                   ?? throw new UserNotFoundException(email);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) throw new IncorrectPasswordException(email);

        return await _tokenClaimsService.GetTokenAsync(user.UserName);
    }

    public async Task<string> AddUserToRolesAsync(string email, IEnumerable<string> roles)
    {
        var user = await _userManager.FindByEmailAsync(email)
                    ?? throw new UserNotFoundException(email);

        foreach (var role in roles)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                throw new RoleDoesNotExistException(role);
            }
            
            await _userManager.AddToRoleAsync(user, role);
        }
        
        var newToken = await _tokenClaimsService.GetTokenAsync(user.UserName);
        
        return newToken;
    }
    
    public async Task SendPasswordResetTokenAsync(string email, string linkToResetPassword)
    {
        var user = await _userManager.FindByEmailAsync(email) 
                   ?? throw new UserNotFoundException(email);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = linkToResetPassword;

        await _emailSender.EmailSendAsync(
            email,
            "Password Reset Request",
            $"To reset your password, click the link: {resetLink}'\n'" +
                    $"Your token is {token}",
            CancellationToken.None
        );
    }

    public async Task ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email)
                   ?? throw new UserNotFoundException(email);
        
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Password reset failed: {errors}");
        }
    }
}