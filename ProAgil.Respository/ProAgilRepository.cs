using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Respository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;

        public ProAgilRepository(ProAgilContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //GERAIS
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
         public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        //EVENTO

        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes= false)
        {
            IQueryable<Evento> query = _context.Eventos
            .Include(c => c.Lotes)
            .Include(c => c.RedesSociais);

            if(includePalestrantes)
            {
                query = query.Include(p => p.PalestranteEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderBy(c => c.Id);
            return await query.ToArrayAsync();
        }

        public async Task<Evento> GetEventoAsyncById(int eventoId, bool includePalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
            .Include(c => c.Lotes)
            .Include(c => c.RedesSociais);

            if(includePalestrantes)
            {
                query = query.Include(pe => pe.PalestranteEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderBy(c => c.Id).Where(c => c.Id == eventoId);
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
            .Include(c => c.Lotes)
            .Include(c => c.RedesSociais);

            if(includePalestrantes)
            {
                query = query.Include(p => p.PalestranteEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderByDescending(c => c.DataEvento).Where(c => c.Tema.ToLower().Contains(tema.ToLower()));
            return await query.ToArrayAsync();
        }

        //PALESTRANTE

        public async Task<Palestrante[]> GetAllPalestrantesAsync(bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes.Include(p => p.RedesSociais);

            if(includeEventos)
            {
                query = query.Include(pe => pe.PalestranteEventos).ThenInclude(p => p.Evento);
            }

            query = query.AsNoTracking().OrderBy(c => c.Nome);
            
            return await query.AsNoTracking().ToArrayAsync();
        }

        public async Task<Palestrante> GetPalestranteAsyncById(int palestranteId, bool includeEventos = false)
        {
             IQueryable<Palestrante> query = _context.Palestrantes.Include(p => p.RedesSociais);

            if(includeEventos)
            {
                query = query.Include(pe => pe.PalestranteEventos).ThenInclude(p => p.Evento);
            }

            query = query.OrderBy(c => c.Nome).Where(p => p.Id == palestranteId);
            
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetPalestrantesAsyncByName(string name, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes.Include(p => p.RedesSociais);
            if(includeEventos)
            {
                query = query.Include(pe => pe.PalestranteEventos).ThenInclude(p => p.Evento);
            }

            query = query.AsNoTracking().Where(p => p.Nome.ToLower().Contains(name));
            
            return await query.ToArrayAsync();
        }
    }    
}
