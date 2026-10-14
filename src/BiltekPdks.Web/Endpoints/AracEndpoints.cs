using BiltekPdks.Domain.Entities;
using BiltekPdks.Infrastructure.Aktarim;
using BiltekPdks.Infrastructure.Data;
using BiltekPdks.Infrastructure.Firmalar;
using BiltekPdks.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BiltekPdks.Web.Endpoints;

public static class AracEndpoints
{
    public static void MapAracEndpoints(this IEndpointRouteBuilder app)
    {
        // Araçlar → Data Yedekle: veritabanının tutarlı bir kopyasını indirir (yalnızca yönetici).
        app.MapGet("/araclar/yedek", async (HttpContext http, IFirmaVeritabanlari veritabanlari) =>
        {
            // Bölüm kısıtlı kullanıcı tüm veritabanını indiremez.
            if (http.User.HasClaim(c => c.Type == Talepler.Bolum)) return Results.Forbid();
            // Ayrı veritabanı modunda yalnızca oturumdaki firmanın veritabanı yedeklenir.
            await using var db = await veritabanlari.Fabrika(http.User).CreateDbContextAsync(http.RequestAborted);
            if (!db.Database.IsSqlite())
                return Results.Text("Sunucu (MySQL) kurulumunda yedek, veritabanı sunucusunun kendi yedekleme aracıyla alınır.", statusCode: 400);

            var gecici = Path.Combine(Path.GetTempPath(), $"pdks-yedek-{Guid.NewGuid():N}.db");
            try
            {
                // VACUUM INTO, program çalışırken bile tutarlı bir kopya üretir.
                var hedef = new SqliteConnectionStringBuilder { DataSource = gecici }.DataSource.Replace("'", "''");
                await db.Database.ExecuteSqlRawAsync($"VACUUM INTO '{hedef}'", http.RequestAborted);
                var icerik = await File.ReadAllBytesAsync(gecici, http.RequestAborted);
                return Results.File(icerik, "application/octet-stream", $"pdks-yedek-{DateTime.Now:yyyy-MM-dd-HHmm}.db");
            }
            finally
            {
                try { File.Delete(gecici); } catch (IOException) { }
            }
        }).RequireAuthorization(Yetkiler.Yedek);

        // Excel'den veri alma için boş örnek dosya: doğru başlıklarla doldurulup geri yüklenir.
        app.MapGet("/araclar/aktarim-ornegi.xlsx", (AktarimTuru tur) =>
        {
            var alanlar = ExcelAktarimi.Alanlar(tur);
            if (alanlar.Count == 0) return Results.NotFound();
            using var kitap = new ClosedXML.Excel.XLWorkbook();
            var s = kitap.Worksheets.Add(ExcelAktarimi.TurAdi(tur));
            for (var i = 0; i < alanlar.Count; i++)
            {
                var c = s.Cell(1, i + 1);
                c.Value = alanlar[i].Etiket;
                c.Style.Font.Bold = true;
                if (alanlar[i].Zorunlu) c.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightYellow;
            }
            s.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            kitap.SaveAs(ms);
            return Results.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ornek-{tur.ToString().ToLowerInvariant()}.xlsx");
        }).RequireAuthorization();
    }
}
