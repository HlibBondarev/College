﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using College.BLL.Interfaces;
using College.BLL.Services.JwtAuthentication.Models;
using College.BLL.Services.JwtAuthentication.Settings;
using College.DAL.Entities.JwtAuthentication;
using College.DAL.Repositories.Interfaces.Base;

namespace College.BLL.Services.JwtAuthentication;

public class UserService : IUserService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtConfig _jwt;
    private readonly AuthenticationConfig _authentication;
    private readonly IValidator<RegisterModel> _registerModelValidator;
    private readonly IValidator<TokenRequestModel> _tokenRequestModelValidator;
    private readonly IValidator<AddRoleModel> _addRoleModelValidator;

    public UserService(IRepositoryWrapper repositoryWrapper,
                       UserManager<ApplicationUser> userManager,
                       IOptions<JwtConfig> jwt,
                       IOptions<AuthenticationConfig> authentication,
                       IValidator<RegisterModel> registerModelValidator,
                       IValidator<TokenRequestModel> tokenRequestModelValidator,
                       IValidator<AddRoleModel> addRoleModelValidator)
    {
        _repositoryWrapper = repositoryWrapper;
        _userManager = userManager;
        _jwt = jwt.Value;
        _authentication = authentication.Value;
        _registerModelValidator = registerModelValidator;
        _tokenRequestModelValidator = tokenRequestModelValidator;
        _addRoleModelValidator = addRoleModelValidator;
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        await _registerModelValidator.ValidateAndThrowAsync(model);

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName!,
            LastName = model.LastName!
        };

        var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email!);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            await _repositoryWrapper.SaveChangesAsync();

            return $"User Registered with username {user.UserName}";
        }
        else
        {
            return $"Email {user.Email} is already registered.";
        }
    }

    public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
    {
        await _tokenRequestModelValidator.ValidateAndThrowAsync(model);

        var authenticationModel = new AuthenticationModel();
        var user = await _repositoryWrapper.UsersRepository.GetSingleOrDefaultAsync(
                        predicate: u => u.Email == model.Email,
                        include: qu => qu.Include(u => u.RefreshTokens));

        if (user == null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"No Accounts Registered with {model.Email}.";
            return authenticationModel;
        }
        if (await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationModel.Email = user.Email!;
            authenticationModel.UserName = user.UserName!;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = rolesList.ToList();

            if (user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(a => a.IsActive);
                authenticationModel.RefreshToken = activeRefreshToken!.Token!;
                authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = CreateRefreshToken();
                authenticationModel.RefreshToken = refreshToken.Token!;
                authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
                user.RefreshTokens.Add(refreshToken);
                _repositoryWrapper.UsersRepository.Update(user);
                await _repositoryWrapper.SaveChangesAsync();
            }

            return authenticationModel;
        }
        authenticationModel.IsAuthenticated = false;
        authenticationModel.Message = $"Incorrect Credentials for user {user.Email}.";
        return authenticationModel;
    }

    public async Task<string> AddRoleAsync(AddRoleModel model)
    {
        await _addRoleModelValidator.ValidateAndThrowAsync(model);

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            return $"No Accounts Registered with {model.Email}.";
        }
        if (await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var roleExists = _authentication.Roles.Exists(x => x.Equals(model.Role, StringComparison.OrdinalIgnoreCase));

            if (roleExists)
            {
                var validRole = _authentication.Roles.Find(x => x.Equals(model.Role, StringComparison.OrdinalIgnoreCase));
                await _userManager.AddToRoleAsync(user, validRole!);
                return $"Added {model.Role} role to user {model.Email}.";
            }
            return $"Role {model.Role} not found.";
        }
        return $"Incorrect Credentials for user {user.Email}.";
    }

    public async Task<AuthenticationModel> RefreshTokenAsync(string token)
    {
        var authenticationModel = new AuthenticationModel();
        var user = await _repositoryWrapper.UsersRepository.GetSingleOrDefaultAsync(
                        predicate: u => u.RefreshTokens.Any(t => t.Token == token),
                        include: qu => qu.Include(u => u.RefreshTokens));

        if (user == null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Token did not match any users.";
            return authenticationModel;
        }

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Token Not Active.";
            return authenticationModel;
        }

        //Revoke Current Refresh Token
        refreshToken.Revoked = DateTime.UtcNow;

        //Generate new Refresh Token and save to Database
        var newRefreshToken = CreateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        _repositoryWrapper.Update(user);
        await _repositoryWrapper.SaveChangesAsync();
        authenticationModel.RefreshToken = newRefreshToken.Token!;
        authenticationModel.RefreshTokenExpiration = newRefreshToken.Expires;

        //Generates new jwt
        authenticationModel.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
        authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authenticationModel.Email = user.Email!;
        authenticationModel.UserName = user.UserName!;
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        authenticationModel.Roles = rolesList.ToList();
        return authenticationModel;
    }

    public async Task<ApplicationUser?> GetById(string id)
    {
        return await _repositoryWrapper.UsersRepository.GetSingleOrDefaultAsync(
                      predicate: u => u.Id == id,
                      include: qu => qu.Include(u => u.RefreshTokens));
    }

    public async Task<bool> RevokeToken(string token)
    {
        var user = await _repositoryWrapper.UsersRepository.GetSingleOrDefaultAsync(
                        predicate: u => u.RefreshTokens.Any(t => t.Token == token),
                        include: qu => qu.Include(u => u.RefreshTokens));

        // return false if no user found with token
        if (user == null) return false;

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        // return false if token is not active
        if (!refreshToken.IsActive) return false;

        // revoke token and save
        refreshToken.Revoked = DateTime.UtcNow;
        _repositoryWrapper.Update(user);
        await _repositoryWrapper.SaveChangesAsync();

        return true;
    }

    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("uid", user.Id)
        }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    private static RefreshToken CreateRefreshToken()
    {
        var randomNumber = RandomNumberGenerator.GetBytes(32);
        
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(10),
            Created = DateTime.UtcNow
        };
    }
}
