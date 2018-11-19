using CasaDoCodigo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        private readonly ICategoriaRepository categoriaRepository;

        public ProdutoRepository(ApplicationContext contexto,
            ICategoriaRepository categoriaRepository) : base(contexto)
        {
            this.categoriaRepository = categoriaRepository;
        }

        public IList<Produto> GetProdutos()
        {
            return dbSet
                .Include(c => c.Categoria)
                .ToList();
        }

        public async Task<IList<Produto>> GetProdutos(string filtro)
        {
            IList<Produto> produtos = new List<Produto>();

            produtos = await dbSet
                    .Include(c => c.Categoria)
                    .Where(p => p.Nome.ToUpper().Contains(filtro.ToUpper()) || 
                           p.Categoria.Nome.ToUpper().Contains(filtro.ToUpper()))
                    .ToListAsync();

            if (produtos.Count <= 0)
            {
                produtos = await dbSet
                    .Include(c => c.Categoria)
                    .ToListAsync();
            }

            return produtos;
        }

        public void SaveProdutos(List<Livro> livros)
        {
            int changes = 0;

            foreach (var livro in livros)
            {
                if (!dbSet.Where(p => p.Codigo.Equals(livro.Codigo, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    var categoria = ObterCategoria(livro.Categoria);
                    dbSet.Add(new Produto(livro.Codigo, livro.Nome, livro.Preco, categoria));
                    changes++;
                }
            }

            if (changes > 0)
                contexto.SaveChanges();
        }

        private Categoria ObterCategoria(string nome)
        {
            if (string.IsNullOrEmpty(nome))
                return null;

            categoriaRepository.SalveCategoria(nome).Wait();
            return categoriaRepository.GetCategoria(nome);
        }
    }

    public class Livro
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public decimal Preco { get; set; }
    }
}
