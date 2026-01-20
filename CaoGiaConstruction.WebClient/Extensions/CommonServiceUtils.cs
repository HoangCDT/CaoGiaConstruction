using Microsoft.EntityFrameworkCore;

namespace CaoGiaConstruction.WebClient.Extensions
{
    public static class CommonServiceUtils
    {
        public static async Task<bool> CheckExistCodeAsync<T>(this DbSet<T> dbSet, string code, Guid id) where T : class
        {
            code = code.Trim();
            var entity = await dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => EF.Property<string>(x, "Code") == code && EF.Property<Guid>(x, "Id") != id);
            return entity != null;
        }
    }
}
