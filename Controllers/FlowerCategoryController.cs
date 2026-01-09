using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/categories")]
public class FlowerCategoryController : ControllerBase
{
    private readonly IFlowerCategoryService _categoryService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<FlowerCategoryController> _logger;

    public FlowerCategoryController(
        IFlowerCategoryService categoryService, 
        IAuthorizationService authorizationService,
        ILogger<FlowerCategoryController> logger)
    {
        _categoryService = categoryService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all flower categories
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlowerCategory>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get flower category by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlowerCategory>> Get(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    /// <summary>
    /// Create a new flower category
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FlowerCategory>> Post([FromBody] FlowerCategory category)
    {
        if (!_authorizationService.CanCreate(User))
        {
            return Forbid();
        }

        var created = await _categoryService.CreateAsync(category);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing flower category
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] FlowerCategory category)
    {
        if (!_authorizationService.CanUpdate(User))
        {
            return Forbid();
        }

        if (id != category.Id)
        {
            return BadRequest(new { message = "Flower category ID mismatch" });
        }

        var updated = await _categoryService.UpdateAsync(id, category);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a flower category
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!_authorizationService.CanDelete(User))
        {
            return Forbid();
        }

        var deleted = await _categoryService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
