using System.Threading.Tasks;

namespace backend.Services.Interfaces
{
    public interface ITranslatorService
    {
        public Task<string> Prevedi(string ss);
    }
}