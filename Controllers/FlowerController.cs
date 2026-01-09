using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.DTO;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/flowers")]
public class FlowerController : ControllerBase
{
    private readonly IFlowerService _flowerService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<FlowerController> _logger;

    public FlowerController(
        IFlowerService flowerService, 
        IAuthorizationService authorizationService,
        ILogger<FlowerController> logger)
    {
        _flowerService = flowerService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all flowers with pagination and filtering
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<FlowerResponseDto>>> GetFiltered([FromQuery] FlowerFilterDto filter)
    {
        var result = await _flowerService.GetFilteredAsync(filter);
        
        var response = new PagedResponseDto<FlowerResponseDto>
        {
            Items = result.Items.Select(f => new FlowerResponseDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                Quantity = f.Quantity,
                Color = f.Color,
                Season = f.Season,
                ImageUrl = f.ImageUrl,
                CategoryId = f.CategoryId,
                CategoryName = f.Category?.Name,
                IsAvailable = f.IsAvailable,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }),
            Total = result.Total,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(response);
    }

    /// <summary>
    /// Get all flowers (without pagination)
    /// </summary>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlowerResponseDto>>> GetAll()
    {
        var flowers = await _flowerService.GetAllAsync();
        var response = flowers.Select(f => new FlowerResponseDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Price = f.Price,
            Quantity = f.Quantity,
            Color = f.Color,
            Season = f.Season,
            ImageUrl = f.ImageUrl,
            CategoryId = f.CategoryId,
            CategoryName = f.Category?.Name,
            IsAvailable = f.IsAvailable,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        });

        return Ok(response);
    }

    /// <summary>
    /// Get flower by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlowerResponseDto>> Get(int id)
    {
        var flower = await _flowerService.GetByIdAsync(id);

        if (flower == null)
        {
            return NotFound();
        }

        var response = new FlowerResponseDto
        {
            Id = flower.Id,
            Name = flower.Name,
            Description = flower.Description,
            Price = flower.Price,
            Quantity = flower.Quantity,
            Color = flower.Color,
            Season = flower.Season,
            ImageUrl = flower.ImageUrl,
            CategoryId = flower.CategoryId,
            CategoryName = flower.Category?.Name,
            IsAvailable = flower.IsAvailable,
            CreatedAt = flower.CreatedAt,
            UpdatedAt = flower.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new flower
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FlowerResponseDto>> Post(
        [FromBody] FlowerCreateDto dto,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        [FromServices] IIdempotencyService idempotencyService)
    {
        if (!_authorizationService.CanCreate(User))
        {
            return Forbid();
        }

        // Check idempotency
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var cachedResponse = await idempotencyService.GetResponseAsync(idempotencyKey);
            if (cachedResponse != null)
            {
                var cachedDto = JsonSerializer.Deserialize<FlowerResponseDto>(cachedResponse);
                if (cachedDto != null)
                {
                    return Ok(cachedDto);
                }
            }
        }

        var flower = await _flowerService.CreateAsync(dto);
        
        var response = new FlowerResponseDto
        {
            Id = flower.Id,
            Name = flower.Name,
            Description = flower.Description,
            Price = flower.Price,
            Quantity = flower.Quantity,
            Color = flower.Color,
            Season = flower.Season,
            ImageUrl = flower.ImageUrl,
            CategoryId = flower.CategoryId,
            CategoryName = flower.Category?.Name,
            IsAvailable = flower.IsAvailable,
            CreatedAt = flower.CreatedAt,
            UpdatedAt = flower.UpdatedAt
        };

        // Store idempotency response
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var responseJson = JsonSerializer.Serialize(response);
            await idempotencyService.StoreResponseAsync(idempotencyKey, responseJson, TimeSpan.FromHours(24));
        }

        return CreatedAtAction(nameof(Get), new { id = flower.Id }, response);
    }

    /// <summary>
    /// Update an existing flower
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] FlowerUpdateDto dto)
    {
        if (!_authorizationService.CanUpdate(User))
        {
            return Forbid();
        }

        var updated = await _flowerService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a flower
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

        var deleted = await _flowerService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlowerResponseDto>>> GetByCategory(int categoryId)
    {
        var flowers = await _flowerService.GetByCategoryIdAsync(categoryId);
        var response = flowers.Select(f => new FlowerResponseDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Price = f.Price,
            Quantity = f.Quantity,
            Color = f.Color,
            Season = f.Season,
            ImageUrl = f.ImageUrl,
            CategoryId = f.CategoryId,
            CategoryName = f.Category?.Name,
            IsAvailable = f.IsAvailable,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        });

        return Ok(response);
    }

    [HttpGet("color/{color}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlowerResponseDto>>> GetByColor(string color)
    {
        var flowers = await _flowerService.GetByColorAsync(color);
        var response = flowers.Select(f => new FlowerResponseDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Price = f.Price,
            Quantity = f.Quantity,
            Color = f.Color,
            Season = f.Season,
            ImageUrl = f.ImageUrl,
            CategoryId = f.CategoryId,
            CategoryName = f.Category?.Name,
            IsAvailable = f.IsAvailable,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        });

        return Ok(response);
    }
}
