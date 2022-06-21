using MinAPI.Models;

namespace MinAPI.Data
{
    public interface ICommandRepo
    {
       Task SaveChangesAsync();
       Task<Command?> GetCommandById(int id);
        Task<IEnumerable<Command>> GetAllCommands();
        Task<Command> CreateCommand(Command command);
        void DeleteCommand(Command command);
    }
}

