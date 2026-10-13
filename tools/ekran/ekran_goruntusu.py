"""Uygulamayı tarayıcıda gezip ekran görüntüleri alır (CI'da kullanılır)."""
import sys
from playwright.sync_api import sync_playwright

ADRES, SIFRE, KLASOR = sys.argv[1], sys.argv[2], sys.argv[3]
SAYFALAR = [
    ("02-ana-sayfa", "/"),
    ("03-personel-listesi", "/personeller"),
    ("04-personel-karti", "/personeller/1"),
    ("05-aylik-puantaj", "/puantaj?personel=1"),
    ("06-aylik-puantaj-2", "/puantaj?personel=3"),
    ("07-hareketler", "/hareketler"),
    ("08-izinler", "/izinler"),
    ("09-vardiyalar", "/vardiyalar"),
    ("10-haftalik-plan", "/calisma-plani"),
    ("11-resmi-tatiller", "/resmi-tatiller"),
    ("12-bolumler", "/tanimlar/bolum"),
]

with sync_playwright() as p:
    b = p.chromium.launch()
    s = b.new_page(viewport={"width": 1440, "height": 900}, locale="tr-TR")
    s.goto(ADRES + "/hesap/giris")
    s.wait_for_load_state("networkidle")
    s.screenshot(path=f"{KLASOR}/01-giris.png")
    s.fill("#kullaniciAdi", "admin")
    s.fill("#sifre", SIFRE)
    s.click("button[type=submit]")
    s.wait_for_url(ADRES + "/")
    for ad, yol in SAYFALAR:
        s.goto(ADRES + yol)
        s.wait_for_load_state("networkidle")
        s.wait_for_timeout(1200)
        s.screenshot(path=f"{KLASOR}/{ad}.png", full_page=True)
        print("ok", ad)
    b.close()
