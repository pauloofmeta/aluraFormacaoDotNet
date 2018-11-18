using CasaDoCodigo.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface ICategoriaRepository
    {
        Task SalveCategoria(string nome);
        Categoria GetCategoria(string nome);
    }

    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ApplicationContext contexto) : base(contexto)
        {
        }

        public Categoria GetCategoria(string nome)
        {
            return dbSet
                .Where(c => c.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();
        }

        public async Task SalveCategoria(string nome)
        {
            if (!dbSet.Where(c => c.Nome.Equals(nome)).Any())
            {
                dbSet.Add(new Categoria(nome));
                await contexto.SaveChangesAsync();
            }
        }
    }
}
