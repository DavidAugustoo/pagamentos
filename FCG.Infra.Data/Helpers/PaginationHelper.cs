using FCG.Domain.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PagedList<T>> CreateAsync<T>
            (IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take((pageSize)).ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}

/*
   Essa função recebe uma consulta (IQueryable<T>), um número de página e um tamanho de página, e retorna um objeto PagedList<T>, 
   que contém os itens da página solicitada junto com informações sobre a paginação.
 */