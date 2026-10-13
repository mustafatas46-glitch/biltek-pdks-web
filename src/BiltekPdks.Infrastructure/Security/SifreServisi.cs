using BiltekPdks.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BiltekPdks.Infrastructure.Security;

/// <summary>PBKDF2 tabanlı şifre hash'leme (ASP.NET Core Identity ile aynı algoritma).</summary>
public class SifreServisi
{
    private readonly PasswordHasher<Kullanici> _hasher = new();

    public string Hashle(Kullanici kullanici, string sifre) => _hasher.HashPassword(kullanici, sifre);

    public bool Dogrula(Kullanici kullanici, string sifre) =>
        !string.IsNullOrEmpty(kullanici.SifreHash) &&
        _hasher.VerifyHashedPassword(kullanici, kullanici.SifreHash, sifre) != PasswordVerificationResult.Failed;

    /// <summary>Asgari şifre kuralı: en az 8 karakter, harf ve rakam.</summary>
    public static string? KuralHatasi(string? sifre)
    {
        if (string.IsNullOrWhiteSpace(sifre) || sifre.Length < 8) return "Şifre en az 8 karakter olmalı.";
        if (!sifre.Any(char.IsLetter) || !sifre.Any(char.IsDigit)) return "Şifre harf ve rakam içermeli.";
        return null;
    }
}
