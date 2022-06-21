using Microsoft.EntityFrameworkCore;
using MinAPI.Models;

namespace MinAPI.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;
        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Command?> GetCommandById(int id)
        {
            return await _context.Commands.FindAsync(id);
        }
        public async Task<IEnumerable<Command>> GetAllCommands()
        {
            return await _context.Commands.ToListAsync();
        }
        public async Task CreateCommand(Command command)
        {
            if(command == null)
            {
                throw new System. ArgumentNullException(nameof(command));
            }

            await _context.AddAsync(command);
        }
        public void DeleteCommand(Command command)
        {
            if(command == null)
            {
                throw new System.ArgumentNullException(nameof(command));
            }

            _context.Commands.Remove(command);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        Task<Command> ICommandRepo.CreateCommand(Command command)
        {
            throw new NotImplementedException();
        }
    }
}