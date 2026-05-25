using DevSpot.Data;
using DevSpot.Models;
using DevSpot.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Repositories;

public class CompanyRepository : IRepository<Company>
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task AddAsync(Company entity)
    {
        await _context.Companies.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company entity)
    {
        _context.Companies.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
        {
            throw new KeyNotFoundException();
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
    }
}