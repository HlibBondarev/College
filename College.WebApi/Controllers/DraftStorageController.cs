﻿using College.BLL.Common;
using College.BLL.Services.DraftStorage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace College.WebApi.Controllers;

/// <summary>Abstract controller with operations for storing the entity draft in cache.</summary>
/// <typeparam name="T">T is the entity draft type that should be stored in the cache.</typeparam>
public class DraftStorageController<T> : ControllerBase
{
    private readonly IDraftStorageService<T> draftStorageService;

    /// <summary>Initializes a new instance of the <see cref="DraftStorageController{T}" /> class.</summary>
    /// <param name="mementoService">The draft storage service.</param>
    protected DraftStorageController(IDraftStorageService<T> draftStorageService)
    {
        this.draftStorageService = draftStorageService;
    }

    /// <summary>Stores the entity draft.</summary>
    /// <param name="draftDto">The entity draft dto for type T.</param>
    /// <returns>
    /// Information about the stored entity of type T in the cache.
    /// </returns>
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> StoreDraft([FromBody] T draftDto)
    {
        if (!ModelState.IsValid)
        {
            return this.BadRequest(ModelState);
        }

        await draftStorageService.CreateAsync(GettingUserProperties.GetUserId(User), draftDto).ConfigureAwait(false);

        return Ok($"{typeof(T).Name} is stored");
    }

    /// <summary>Restores the entity draft.</summary>
    /// <returns> The entity draft dto of type T.</returns>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> RestoreDraft()
    {
        var memento = await draftStorageService.RestoreAsync(GettingUserProperties.GetUserId(User)).ConfigureAwait(false);

        return Ok(memento);
    }

    /// <summary>Removes the entity draft from the cache.</summary>
    /// <returns> Information about removing an entity of type T from the cache.</returns>
    [HttpDelete]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> RemoveDraft()
    {
        var userId = GettingUserProperties.GetUserId(User);
        await draftStorageService.RemoveAsync(userId).ConfigureAwait(false);

        return NoContent();
    }
}
