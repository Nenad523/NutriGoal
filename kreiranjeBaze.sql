-- =========================
-- DATABASE
-- =========================
CREATE DATABASE NutriGoal;
GO

USE NutriGoal;
GO

-- =========================
-- 1. KORISNICI
-- =========================
CREATE TABLE Korisnici (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    DatumRegistracije DATETIME NOT NULL DEFAULT GETDATE(),
    Uloga NVARCHAR(20) NOT NULL DEFAULT 'Korisnik'
        CHECK (Uloga IN ('Admin', 'Korisnik'))
);

-- =========================
-- 2. CILJEVI
-- =========================
CREATE TABLE Ciljevi (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE,
    Opis NVARCHAR(500) NOT NULL,
    FokusTekst NVARCHAR(1000),
    Faktor DECIMAL(4,2) NOT NULL CHECK (Faktor > 0)
);

-- =========================
-- 3. KATEGORIJE
-- =========================
CREATE TABLE Kategorije (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv NVARCHAR(50) NOT NULL UNIQUE
);

-- =========================
-- 4. ALERGIJE
-- =========================
CREATE TABLE Alergije (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE,
    Opis NVARCHAR(500)
);

-- =========================
-- 5. SASTOJCI
-- =========================
CREATE TABLE Sastojci (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv NVARCHAR(255) NOT NULL UNIQUE,
    KalorijeNa100g DECIMAL(10,2) NOT NULL CHECK (KalorijeNa100g >= 0),
    ProteinNa100g DECIMAL(10,2) NOT NULL CHECK (ProteinNa100g >= 0),
    UHNa100g DECIMAL(10,2) NOT NULL CHECK (UHNa100g >= 0),
    MastiNa100g DECIMAL(10,2) NOT NULL CHECK (MastiNa100g >= 0)
);

-- =========================
-- 6. KORISNIČKI PROFIL
-- =========================
CREATE TABLE KorisnickiProfil (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KorisnikId INT NOT NULL UNIQUE,
    Ime NVARCHAR(100),
    Prezime NVARCHAR(100),
    DatumRodjenja DATE,
    Pol CHAR(1) CHECK (Pol IN ('M', 'Z')),
    Visina DECIMAL(5,2),
    Tezina DECIMAL(5,2),
    NivoAktivnosti INT CHECK (NivoAktivnosti BETWEEN 1 AND 4),
    CiljId INT NULL,
    BMR DECIMAL(10,2),
    TDEE DECIMAL(10,2),
    PreporucenoKalorija DECIMAL(10,2),
    DatumAzuriranja DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id),
    FOREIGN KEY (CiljId) REFERENCES Ciljevi(Id)
);

CREATE INDEX IX_KorisnickiProfil_KorisnikId ON KorisnickiProfil(KorisnikId);

-- =========================
-- 7. RECEPTI
-- =========================
CREATE TABLE Recepti (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv NVARCHAR(255) NOT NULL,
    Opis NVARCHAR(1000),
    Postupak NVARCHAR(MAX) NOT NULL,
    Fotografija NVARCHAR(500),
    KategorijaId INT NOT NULL,
    VrijemePripreme INT NOT NULL CHECK (VrijemePripreme > 0),

    Kalorije DECIMAL(10,2),
    Proteini DECIMAL(10,2),
    UgljeniHidrati DECIMAL(10,2),
    Masti DECIMAL(10,2),

    DatumKreiranja DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (KategorijaId) REFERENCES Kategorije(Id)
);

CREATE INDEX IX_Recepti_KategorijaId ON Recepti(KategorijaId);

-- =========================
-- 8. RECEPT - CILJEVI (M:N)
-- =========================
CREATE TABLE ReceptCiljevi (
    ReceptId INT NOT NULL,
    CiljId INT NOT NULL,

    PRIMARY KEY (ReceptId, CiljId),

    FOREIGN KEY (ReceptId) REFERENCES Recepti(Id),
    FOREIGN KEY (CiljId) REFERENCES Ciljevi(Id)
);

-- =========================
-- 9. RECEPT SASTOJCI
-- =========================
CREATE TABLE ReceptSastojci (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReceptId INT NOT NULL,
    SastojakId INT NOT NULL,
    KolicinaG DECIMAL(10,2) NOT NULL CHECK (KolicinaG > 0),

    CONSTRAINT UQ_Recept_Sastojak UNIQUE (ReceptId, SastojakId),

    FOREIGN KEY (ReceptId) REFERENCES Recepti(Id),
    FOREIGN KEY (SastojakId) REFERENCES Sastojci(Id)
);

CREATE INDEX IX_ReceptSastojci_ReceptId ON ReceptSastojci(ReceptId);
CREATE INDEX IX_ReceptSastojci_SastojakId ON ReceptSastojci(SastojakId);

-- =========================
-- 10. SASTOJAK ALERGIJA
-- =========================
CREATE TABLE SastojakAlergija (
    SastojakId INT NOT NULL,
    AlergijaId INT NOT NULL,

    PRIMARY KEY (SastojakId, AlergijaId),

    FOREIGN KEY (SastojakId) REFERENCES Sastojci(Id),
    FOREIGN KEY (AlergijaId) REFERENCES Alergije(Id)
);

-- =========================
-- 11. KORISNIK ALERGIJE
-- =========================
CREATE TABLE KorisnikAlergije (
    KorisnikId INT NOT NULL,
    AlergijaId INT NOT NULL,

    PRIMARY KEY (KorisnikId, AlergijaId),

    FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id),
    FOREIGN KEY (AlergijaId) REFERENCES Alergije(Id)
);

-- =========================
-- 12. KOMENTARI
-- =========================
CREATE TABLE Komentari (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KorisnikId INT NOT NULL,
    ReceptId INT NOT NULL,
    Tekst NVARCHAR(MAX) NOT NULL,
    DatumKreiranja DATETIME NOT NULL DEFAULT GETDATE(),
    Odobren BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id),
    FOREIGN KEY (ReceptId) REFERENCES Recepti(Id)
);

CREATE INDEX IX_Komentari_ReceptId ON Komentari(ReceptId);

-- =========================
-- 13. OCJENE
-- =========================
CREATE TABLE Ocjene (
    KorisnikId INT NOT NULL,
    ReceptId INT NOT NULL,
    Vrijednost INT NOT NULL CHECK (Vrijednost BETWEEN 1 AND 5),
    DatumOcjene DATETIME NOT NULL DEFAULT GETDATE(),

    PRIMARY KEY (KorisnikId, ReceptId),

    FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id),
    FOREIGN KEY (ReceptId) REFERENCES Recepti(Id)
);

CREATE INDEX IX_Ocjene_ReceptId ON Ocjene(ReceptId);

-- =========================
-- 14. FAVORITI
-- =========================
CREATE TABLE Favoriti (
    KorisnikId INT NOT NULL,
    ReceptId INT NOT NULL,
    DatumDodavanja DATETIME NOT NULL DEFAULT GETDATE(),

    PRIMARY KEY (KorisnikId, ReceptId),

    FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id),
    FOREIGN KEY (ReceptId) REFERENCES Recepti(Id)
);

-- =========================
-- 15. PLAN ISHRANE
-- =========================
CREATE TABLE PlanIshrane (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KorisnikId INT NOT NULL,
    DatumKreiranja DATETIME NOT NULL DEFAULT GETDATE(),
    DatumOd DATE NOT NULL,
    DatumDo DATE NOT NULL,

    FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id)
);

CREATE INDEX IX_PlanIshrane_KorisnikId ON PlanIshrane(KorisnikId);

-- =========================
-- 16. STAVKE PLANA
-- =========================
CREATE TABLE StavkePlana (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PlanId INT NOT NULL,
    ReceptId INT NOT NULL,
    DanSedmice INT NOT NULL CHECK (DanSedmice BETWEEN 1 AND 7),
    ObrokTip NVARCHAR(20) NOT NULL
        CHECK (ObrokTip IN (N'Dorucak', N'Rucak', N'Vecera', N'Snack')),

    CONSTRAINT UQ_Plan_Dan_Obrok UNIQUE (PlanId, DanSedmice, ObrokTip),

    FOREIGN KEY (PlanId) REFERENCES PlanIshrane(Id),
    FOREIGN KEY (ReceptId) REFERENCES Recepti(Id)
);

CREATE INDEX IX_StavkePlana_PlanId ON StavkePlana(PlanId);