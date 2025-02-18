namespace rotating_work_schedule.Controllers;
using Microsoft.AspNetCore.Mvc;
using rotating_work_schedule.Models;
using YourNamespace.Repositories;

[ApiController]
[Route("api/companies")]
public class CompanyController(ICompanyRepository _repository) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companies = await _repository.GetAllAsync();
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await _repository.GetByIdAsync(id);
        if (company == null) return NotFound();
        return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Company company)
    {
        await _repository.AddAsync(company);
        return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Company company)
    {
        if (id != company.Id) return BadRequest();
        await _repository.UpdateAsync(company);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}