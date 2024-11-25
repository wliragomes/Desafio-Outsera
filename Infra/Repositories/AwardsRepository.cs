using Domain.Entities;
using Domain.Interfaces;
using Infra.Context;

namespace Infra.Repositories
{
    public class AwardsRepository : IAwardsRepository
    {
        private readonly ApplicationDbContext _context;

        public AwardsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Award> awards)
        {
            await _context.Awards.AddRangeAsync(awards); 
            await _context.SaveChangesAsync();
        }
    }
}
