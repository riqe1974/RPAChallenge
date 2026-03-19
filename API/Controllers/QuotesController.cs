using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteRepository _quoteRepository;

    public QuotesController(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quote>>> GetAll()
    {
        var quotes = await _quoteRepository.GetAllAsync();
        return Ok(quotes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Quote>> GetById(int id)
    {
        var quote = await _quoteRepository.GetByIdAsync(id);
        if (quote == null)
            return NotFound();
        return Ok(quote);
    }

    [HttpGet("byauthor")]
    public async Task<ActionResult<IEnumerable<Quote>>> GetByAuthor([FromQuery] string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            return BadRequest("Author parameter is required.");
        var quotes = await _quoteRepository.GetByAuthorAsync(author);
        return Ok(quotes);
    }

    [HttpPost]
    public async Task<ActionResult<Quote>> Create(Quote quote)
    {
        // Aqui poderíamos adicionar validações
        quote.CreatedAt = DateTime.UtcNow;
        await _quoteRepository.AddAsync(quote);
        return CreatedAtAction(nameof(GetById), new { id = quote.Id }, quote);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Quote quote)
    {
        if (id != quote.Id)
            return BadRequest();

        var existing = await _quoteRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _quoteRepository.UpdateAsync(quote);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _quoteRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _quoteRepository.DeleteAsync(id);
        return NoContent();
    }
}