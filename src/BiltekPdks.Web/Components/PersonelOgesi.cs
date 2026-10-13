using BiltekPdks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Web.Components;

public sealed record PersonelOgesi(int Id, string Ad, string KartNo)
{
    /// <summary>Seçim listeleri için personeller; varsayılan olarak yalnızca çalışanlar.</summary>
    public static Task<List<PersonelOgesi>> ListeleAsync(PdksDbContext db, bool ayrilanlarDahil = false)
    {
        var bugun = DateOnly.FromDateTime(DateTime.Today);
        var q = db.Personeller.AsNoTracking();
        if (!ayrilanlarDahil)
            q = q.Where(p => p.IstenCikisTarihi == null || p.IstenCikisTarihi >= bugun);
        return q.OrderBy(p => p.Ad).ThenBy(p => p.Soyad)
            .Select(p => new PersonelOgesi(p.Id, p.Ad + " " + p.Soyad, p.KartNo))
            .ToListAsync();
    }
}
