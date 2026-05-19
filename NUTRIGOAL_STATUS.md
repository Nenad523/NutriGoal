# NutriGoal — Status projekta

## Što je NutriGoal?

NutriGoal je ASP.NET MVC web aplikacija za personalizovanu ishranu. Korisnik unosi svoje podatke (visina, težina, pol, godine, nivo aktivnosti i cilj) i na osnovu toga dobija preporučene recepte prilagođene njegovom cilju i alergijama.

---

## Tehnički stack

- **Backend:** ASP.NET MVC (.NET Framework 4.8)
- **Baza podataka:** SQL Server (MSSQL)
- **ORM:** Entity Framework 6 (Database First)
- **Frontend:** Bootstrap + custom CSS (inspirisan Lovable dizajnom)
- **Autentifikacija:** Session-based
- **Lozinke:** BCrypt hashiranje (BCrypt.Net-Next)
- **IDE:** Visual Studio 2022
- **Version control:** Git / GitHub

---

## Struktura projekta

```
NutriGoal/
├── Controllers/
│   ├── HomeController.cs
│   ├── KorisnikController.cs
│   └── ReceptController.cs
├── Models/
│   ├── NutriGoalModel.edmx (Entity Framework)
│   ├── RegisterViewModel.cs
│   ├── LoginViewModel.cs
│   ├── ProfilViewModel.cs
│   ├── ReceptViewModel.cs
│   └── DodajReceptViewModel.cs
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── Home/
│   │   └── Index.cshtml (nije još kreiran)
│   ├── Korisnik/
│   │   ├── Register.cshtml
│   │   ├── Login.cshtml
│   │   └── Profil.cshtml
│   └── Recept/
│       └── Index.cshtml (nije još kreiran)
└── Content/ (CSS, Bootstrap)
```

---

## Baza podataka — 16 tabela

| # | Tabela | Opis |
|---|---|---|
| 1 | Korisnici | Email, PasswordHash, Uloga (Admin/Korisnik) |
| 2 | KorisnickiProfil | Ime, Prezime, Pol, Visina, Tezina, NivoAktivnosti, CiljId, BMR, TDEE, PreporucenoKalorija |
| 3 | Ciljevi | Mršavljenje, Izgradnja mišića, Održavanje + edukativni tekst + Faktor |
| 4 | Kategorije | Doručak, Ručak, Večera, Snack |
| 5 | Alergije | Gluten, Laktoza, Orasi, Jaja, Riba, Soja, Kikiriki, Školjke |
| 6 | Sastojci | Naziv, KalorijeNa100g, ProteinNa100g, UHNa100g, MastiNa100g |
| 7 | Recepti | Naziv, Opis, Postupak, Fotografija, KategorijaId, VrijemePripreme, Kalorije, Proteini, UgljeniHidrati, Masti, ProsjecnaOcjena |
| 8 | ReceptSastojci | ReceptId, SastojakId, KolicinaG |
| 9 | ReceptCiljevi | ReceptId, CiljId (M:N veza) |
| 10 | SastojakAlergija | SastojakId, AlergijaId (M:N veza) |
| 11 | KorisnikAlergije | KorisnikId, AlergijaId (M:N veza) |
| 12 | Komentari | KorisnikId, ReceptId, Tekst, Odobren |
| 13 | Ocjene | KorisnikId, ReceptId, Vrijednost (1-5) |
| 14 | Favoriti | KorisnikId, ReceptId, DatumDodavanja |
| 15 | PlanIshrane | KorisnikId, DatumOd, DatumDo |
| 16 | StavkePlana | PlanId, ReceptId, DanSedmice, ObrokTip |

### Napomena o EF mapiranju:
- `KorisnikAlergije` → EF mapira kao `Korisnici.Alergije` (many-to-many)
- `SastojakAlergija` → EF mapira kao `Sastojci.Alergije` (many-to-many)
- `ReceptCiljevi` → EF mapira kao `Recepti.Ciljevi` (many-to-many)

---

## SQL objekti

### Funkcije:
- `fn_IzracunajBMR(@Pol, @Visina, @Tezina, @Godine)` — Mifflin-St Jeor formula, vraća BMR
- `fn_UkupnoKalorijaRecepta(@ReceptId)` — vraća ukupno kalorija recepta
- `fn_NutritivneVrijednostiRecepta(@ReceptId)` — Table-valued function, vraća Kalorije/Proteini/UH/Masti

### Trigeri:
- `tr_IzracunajBMR_TDEE` — AFTER INSERT/UPDATE na KorisnickiProfil, automatski računa BMR, TDEE, PreporucenoKalorija koristeći fn_IzracunajBMR
- `tr_AzurirajNutritivneVrijednosti` — AFTER INSERT/UPDATE/DELETE na ReceptSastojci, automatski ažurira Kalorije/Proteini/UH/Masti u Recepti tabeli koristeći fn_NutritivneVrijednostiRecepta

### Stored Procedure:
- `sp_PreporuciRecepte(@KorisnikId)` — vraća recepte koji odgovaraju cilju korisnika i ne sadrže njegove alergene
- `sp_PretragaRecepata(@Naziv, @KategorijaId, @CiljId, @MinKalorije, @MaxKalorije, @MaxVrijeme, @Sortiranje)` — detaljna pretraga recepata sa opcionalnim filterima

---

## Implementirane funkcionalnosti

### KorisnikController:
- ✅ GET/POST Register — registracija sa BCrypt hashiranjem
- ✅ GET/POST Login — prijava sa BCrypt verifikacijom, session
- ✅ GET LogOut — brisanje sesije
- ✅ GET/POST Profil — kreiranje i izmjena profila (cilj, mjere, alergije)

### ReceptController:
- ✅ GET Index(bool sviRecepti) — lista recepata, personalizovana za prijavljenog korisnika
- ✅ POST DodajUFavorite(int receptId)
- ✅ POST UkloniIzFavorita(int receptId)
- ✅ GET Admin(string pretraga) — admin panel sa pretragom

### Session varijable koje se čuvaju pri prijavi:
```csharp
Session["KorisnikId"] = int
Session["KorisnikEmail"] = string
Session["KorisnikUloga"] = "Admin" ili "Korisnik"
Session["KorisnikIme"] = string (ime ili email ako nema profila)
```

---

## Što još nije implementirano

### ReceptController — nedostaje:
- ❌ GET Detalji(int id) — detalji jednog recepta
- ❌ GET/POST Dodaj — admin dodavanje recepta
- ❌ GET/POST Izmijeni(int id) — admin izmjena recepta
- ❌ POST Obrisi(int id) — admin brisanje recepta
- ❌ POST Ocijeni(int receptId, int vrijednost) — ocjenjivanje
- ❌ POST Komentiraj(int receptId, string tekst) — komentarisanje

### Views — nedostaje:
- ❌ Home/Index.cshtml — početna stranica
- ❌ Recept/Index.cshtml — lista recepata
- ❌ Recept/Detalji.cshtml — detalji recepta
- ❌ Recept/Dodaj.cshtml — admin forma
- ❌ Recept/Izmijeni.cshtml — admin forma
- ❌ Recept/Admin.cshtml — admin panel

### Ostalo:
- ❌ Brza pretraga (search bar u navigaciji)
- ❌ Stranica favorita (/Korisnik/Favoriti)
- ❌ Sedmični planer (opciono)

---

## Dizajn

Aplikacija koristi dizajn inspirisan Lovable.dev prototipom.

### Boje:
```css
Pozadina: #F5F0E8 (krem)
Primarna: #1C3D2E (tamno zelena)
Tekst: #1a1a1a
Sekundarni tekst: #6b7280
Bijela: #ffffff
Akcent (alergeni): narandžasta/amber
```

### Komponente:
- Kartice: bijela pozadina, border-radius 12-16px, box-shadow
- Dugmad: border-radius 50px (pill shape), tamno zelena
- Inputi: krem pozadina (#F5F0E8), bez bordera, border-radius 10px
- Navigacija: sticky, backdrop-blur, krem pozadina

---

## Cilj projekta

Ispuniti sve kriterijume za završni projekat iz predmeta Napredne baze podataka:

- ✅ MVC web aplikacija u .NET Framework-u
- ✅ Više nivoa korisnika (Admin/Korisnik)
- ✅ Brza i detaljna pretraga
- ✅ Responsive design
- ✅ 10+ tabela u bazi (imamo 16)
- ✅ 2 trigera
- ✅ 2 stored procedure
- ✅ 2 funkcije (imamo 3)
- ⬜ PDF sa AI konverzacijama (na kraju)
- ✅ GIT

---

## Prioriteti za završetak

### MORA biti gotovo:
1. Login i Register View (dizajn)
2. Home Page
3. Lista recepata (Index View)
4. Detalji recepta
5. Admin panel (CRUD recepti)
6. Brza pretraga

### Ako ostane vremena:
1. Sedmični planer
2. Dashboard sa preporukama
3. Stranica favorita

---

## Napomene za razvoj

### Pozivanje stored procedure iz C#:
```csharp
// Kroz EF direktno (procedure su dodane u EDMX):
var recepti = db.sp_PreporuciRecepte(korisnikId).ToList();

// Mapiranje u ViewModel:
var model = recepti.Select(r => new ReceptViewModel {
    Id = r.Id,
    Naziv = r.Naziv,
    // ...
}).ToList();
```

### Provjera uloge korisnika:
```csharp
// Admin provjera:
if (Session["KorisnikUloga"] == null || 
    Session["KorisnikUloga"].ToString() != "Admin")
{
    return RedirectToAction("Index", "Home");
}

// Prijavljeni korisnik:
if (Session["KorisnikId"] == null)
{
    return RedirectToAction("Login", "Korisnik");
}
```

### EF many-to-many navigacija:
```csharp
// Alergije korisnika:
var korisnik = db.Korisnici.Find(korisnikId);
var alergeniIds = korisnik.Alergije.Select(a => a.Id).ToList();

// Provjera alergena u receptu:
bool sadrziAlergen = recept.ReceptSastojci
    .Any(rs => rs.Sastojci.Alergije
        .Any(a => alergeniIds.Contains(a.Id)));
```
