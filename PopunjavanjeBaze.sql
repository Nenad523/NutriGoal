USE NutriGoal;
GO

INSERT INTO Sastojci (Naziv, KalorijeNa100g, ProteinNa100g, UHNa100g, MastiNa100g)
VALUES
(N'Pileća prsa', 165.00, 31.00, 0.00, 3.60),
(N'Govedina', 250.00, 26.00, 0.00, 17.00),
(N'Losos', 208.00, 20.00, 0.00, 13.00),
(N'Jaja', 155.00, 13.00, 1.10, 11.00),
(N'Mlijeko', 42.00, 3.40, 5.00, 1.00),
(N'Jogurt', 59.00, 3.50, 3.60, 3.30),
(N'Sir', 402.00, 25.00, 1.30, 33.00),
(N'Riža', 130.00, 2.70, 28.00, 0.30),
(N'Tjestenina', 131.00, 5.00, 25.00, 1.10),
(N'Krompir', 77.00, 2.00, 17.00, 0.10),
(N'Brokoli', 34.00, 2.80, 7.00, 0.40),
(N'Spanać', 23.00, 2.90, 3.60, 0.40),
(N'Paradajz', 18.00, 0.90, 3.90, 0.20),
(N'Krastavac', 16.00, 0.70, 3.60, 0.10),
(N'Avokado', 160.00, 2.00, 9.00, 15.00),
(N'Maslinovo ulje', 884.00, 0.00, 0.00, 100.00),
(N'Ovsena kaša', 389.00, 17.00, 66.00, 7.00),
(N'Banana', 89.00, 1.10, 23.00, 0.30),
(N'Jabuka', 52.00, 0.30, 14.00, 0.20),
(N'Orasi', 654.00, 15.00, 14.00, 65.00);

INSERT INTO Kategorije (Naziv)
VALUES 
(N'Doručak'),
(N'Ručak'),
(N'Večera'),
(N'Snack');

INSERT INTO Recepti (Naziv, Opis, Postupak, KategorijaId, VrijemePripreme, DatumKreiranja)
VALUES
(N'Ovsena kaša sa bananom', 
 N'Zdrav i hranjiv doručak bogat vlaknima.', 
 N'1. Skuhaj ovsenu kašu u mlijeku. 2. Dodaj narezanu bananu. 3. Po želji dodaj med.',
 1, 10, GETDATE()),

(N'Kajgana sa povrćem', 
 N'Proteinski doručak sa svježim povrćem.', 
 N'1. Umuti jaja. 2. Dodaj narezani paradajz i spanać. 3. Prži na maslinovom ulju.',
 1, 15, GETDATE()),

(N'Pileća salata', 
 N'Lagana i hranjiva salata sa piletinom.', 
 N'1. Skuhaj pileća prsa. 2. Nareži na trakice. 3. Dodaj paradajz, krastavac i maslinovo ulje.',
 2, 20, GETDATE()),

(N'Losos sa brokolijem', 
 N'Omega-3 bogat obrok sa povrćem.', 
 N'1. Začini losos. 2. Peči u rerni 20 minuta. 3. Skuhaj brokoli na pari.',
 2, 30, GETDATE()),

(N'Tjestenina sa piletinom', 
 N'Klasična tjestenina sa pilećim prsima.', 
 N'1. Skuhaj tjesteninu. 2. Prži piletinu. 3. Pomiješaj i dodaj začine.',
 2, 25, GETDATE()),

(N'Grilovana govedina sa krompirom', 
 N'Proteinski bogat obrok za izgradnju mišića.', 
 N'1. Začini govedinu. 2. Griliraj 10 minuta. 3. Skuhaj krompir.',
 2, 35, GETDATE()),

(N'Pileća supa', 
 N'Topla i hranjiva supa sa povrćem.', 
 N'1. Skuhaj piletinu. 2. Dodaj povrće. 3. Kuhaj 30 minuta.',
 2, 45, GETDATE()),

(N'Riža sa povrćem', 
 N'Lagani obrok bogat ugljenim hidratima.', 
 N'1. Skuhaj rižu. 2. Prži povrće. 3. Pomiješaj zajedno.',
 2, 25, GETDATE()),

(N'Pečeni losos sa rižom', 
 N'Uravnotežen obrok sa proteinima i ugljenim hidratima.', 
 N'1. Začini losos. 2. Peči 20 minuta. 3. Skuhaj rižu.',
 3, 30, GETDATE()),

(N'Pileća prsa sa brokolijem', 
 N'Dijetalni obrok bogat proteinima.', 
 N'1. Začini piletinu. 2. Peči 25 minuta. 3. Skuhaj brokoli.',
 3, 30, GETDATE()),

(N'Grčka salata', 
 N'Osvježavajuća mediteranska salata.', 
 N'1. Nareži paradajz i krastavac. 2. Dodaj sir. 3. Začini maslinovim uljem.',
 3, 10, GETDATE()),

(N'Spanać sa jajetom', 
 N'Lagana večera bogata željezom.', 
 N'1. Prži spanać. 2. Dodaj jaje. 3. Začini po ukusu.',
 3, 15, GETDATE()),

(N'Jogurt sa orasima', 
 N'Brzi snack bogat proteinima.', 
 N'1. Sipaj jogurt u činiju. 2. Dodaj orahe. 3. Po želji dodaj med.',
 4, 5, GETDATE()),

(N'Banana shake', 
 N'Energetski napitak za prije treninga.', 
 N'1. Stavi bananu u blender. 2. Dodaj mlijeko. 3. Miksaj 1 minutu.',
 4, 5, GETDATE()),

(N'Avokado tost', 
 N'Zdravi snack bogat zdravim mastima.', 
 N'1. Namaži avokado na tost. 2. Dodaj paradajz. 3. Začini maslinovim uljem.',
 4, 10, GETDATE());

INSERT INTO Ciljevi (Naziv, Opis, FokusTekst, Faktor)
VALUES
(
    N'Mršavljenje',
    N'Kalorijski deficit je ključ uspjeha. Tijelo troši više nego što unosi, što dovodi do gubitka masnog tkiva.',
    N'Povećaj unos proteina da sačuvaš mišiće. Smanji jednostavne ugljene hidrate i fokusiraj se na hranu koja te siti duže.',
    0.80
),
(
    N'Izgradnja mišića',
    N'Mišići rastu kada imaš kalorijski suficit i dovoljno proteina za oporavak nakon treninga.',
    N'Visok unos proteina je ključan za rast mišića. Ugljeni hidrati ti daju energiju za treninge.',
    1.20
),
(
    N'Održavanje',
    N'Balans je ključ — unosiš onoliko koliko trošiš. Fokus na raznovrsnu i uravnoteženu ishranu.',
    N'Fokusiraj se na raznovrstan unos svih makronutrijenata u umjerenim količinama.',
    1.00
);

INSERT INTO Alergije (Naziv, Opis)
VALUES
(N'Gluten', N'Prisutan u pšenici, raži, ječmu i zobi'),
(N'Laktoza', N'Prisutna u mlijeku i mliječnim proizvodima'),
(N'Orasi', N'Uključuje bademe, lješnjake, orahe i sl.'),
(N'Jaja', N'Prisutna u jajima i proizvodima koji ih sadrže'),
(N'Riba', N'Uključuje sve vrste ribe'),
(N'Soja', N'Prisutna u soji i proizvodima od soje'),
(N'Kikiriki', N'Prisutan u kikirikiju i proizvodima od kikirikija'),
(N'Školjke', N'Uključuje škampe, rakove, jastoge i sl.');

-- Mršavljenje (CiljId = 1)
INSERT INTO ReceptCiljevi (ReceptId, CiljId) VALUES
(2, 1),  -- Ovsena kaša sa bananom
(3, 1),  -- Kajgana sa povrćem
(4, 1),  -- Pileća salata
(5, 1),  -- Losos sa brokolijem
(11, 1), -- Pileća prsa sa brokolijem
(12, 1), -- Grčka salata
(13, 1); -- Spanać sa jajetom

-- Izgradnja mišića (CiljId = 2)
INSERT INTO ReceptCiljevi (ReceptId, CiljId) VALUES
(3, 2),  -- Kajgana sa povrćem
(6, 2),  -- Tjestenina sa piletinom
(7, 2),  -- Grilovana govedina sa krompirom
(8, 2),  -- Pileća supa
(9, 2),  -- Riža sa povrćem
(10, 2), -- Pečeni losos sa rižom
(15, 2); -- Banana shake

-- Održavanje (CiljId = 3)
INSERT INTO ReceptCiljevi (ReceptId, CiljId) VALUES
(2, 3),  -- Ovsena kaša sa bananom
(4, 3),  -- Pileća salata
(5, 3),  -- Losos sa brokolijem
(10, 3), -- Pečeni losos sa rižom
(14, 3), -- Jogurt sa orasima
(16, 3); -- Avokado tost

INSERT INTO ReceptCiljevi (ReceptId, CiljId) VALUES
(3, 2),  -- Kajgana sa povrćem - Izgradnja
(10, 2), -- Pečeni losos sa rižom - Izgradnja
(15, 2), -- Banana shake - Izgradnja
(2, 3),  -- Ovsena kaša - Održavanje
(10, 3), -- Pečeni losos - Održavanje
(13, 1); -- Spanać sa jajetom - Mršavljenje

INSERT INTO ReceptSastojci (ReceptId, SastojakId, KolicinaG) VALUES
-- Ovsena kaša sa bananom (ReceptId = 2)
(2, 17, 100.00),  -- Ovsena kaša 100g
(2, 18, 120.00),  -- Banana 120g
(2, 5, 200.00),   -- Mlijeko 200g

-- Kajgana sa povrćem (ReceptId = 3)
(3, 4, 150.00),   -- Jaja 150g
(3, 13, 100.00),  -- Paradajz 100g
(3, 12, 50.00),   -- Spanać 50g
(3, 16, 10.00),   -- Maslinovo ulje 10g

-- Pileća salata (ReceptId = 4)
(4, 1, 200.00),   -- Pileća prsa 200g
(4, 13, 100.00),  -- Paradajz 100g
(4, 14, 100.00),  -- Krastavac 100g
(4, 16, 15.00),   -- Maslinovo ulje 15g

-- Losos sa brokolijem (ReceptId = 5)
(5, 3, 200.00),   -- Losos 200g
(5, 11, 150.00),  -- Brokoli 150g
(5, 16, 10.00),   -- Maslinovo ulje 10g

-- Tjestenina sa piletinom (ReceptId = 6)
(6, 9, 200.00),   -- Tjestenina 200g
(6, 1, 150.00),   -- Pileća prsa 150g
(6, 16, 10.00),   -- Maslinovo ulje 10g

-- Grilovana govedina sa krompirom (ReceptId = 7)
(7, 2, 250.00),   -- Govedina 250g
(7, 10, 200.00),  -- Krompir 200g
(7, 16, 10.00),   -- Maslinovo ulje 10g

-- Pileća supa (ReceptId = 8)
(8, 1, 300.00),   -- Pileća prsa 300g
(8, 11, 100.00),  -- Brokoli 100g
(8, 14, 100.00),  -- Krastavac 100g

-- Riža sa povrćem (ReceptId = 9)
(9, 8, 200.00),   -- Riža 200g
(9, 11, 100.00),  -- Brokoli 100g
(9, 13, 100.00),  -- Paradajz 100g
(9, 16, 10.00),   -- Maslinovo ulje 10g

-- Pečeni losos sa rižom (ReceptId = 10)
(10, 3, 200.00),  -- Losos 200g
(10, 8, 150.00),  -- Riža 150g
(10, 16, 10.00),  -- Maslinovo ulje 10g

-- Pileća prsa sa brokolijem (ReceptId = 11)
(11, 1, 250.00),  -- Pileća prsa 250g
(11, 11, 200.00), -- Brokoli 200g
(11, 16, 10.00),  -- Maslinovo ulje 10g

-- Grčka salata (ReceptId = 12)
(12, 13, 150.00), -- Paradajz 150g
(12, 14, 150.00), -- Krastavac 150g
(12, 7, 100.00),  -- Sir 100g
(12, 16, 15.00),  -- Maslinovo ulje 15g

-- Spanać sa jajetom (ReceptId = 13)
(13, 12, 200.00), -- Spanać 200g
(13, 4, 100.00),  -- Jaja 100g
(13, 16, 10.00),  -- Maslinovo ulje 10g

-- Jogurt sa orasima (ReceptId = 14)
(14, 6, 200.00),  -- Jogurt 200g
(14, 20, 30.00),  -- Orasi 30g

-- Banana shake (ReceptId = 15)
(15, 18, 150.00), -- Banana 150g
(15, 5, 250.00),  -- Mlijeko 250g

-- Avokado tost (ReceptId = 16)
(16, 15, 100.00), -- Avokado 100g
(16, 13, 50.00),  -- Paradajz 50g
(16, 16, 10.00);  -- Maslinovo ulje 10g

INSERT INTO SastojakAlergija (SastojakId, AlergijaId) VALUES
-- Jaja (SastojakId = 4) → alergen Jaja (AlergijaId = 4)
(4, 4),

-- Mlijeko (SastojakId = 5) → alergen Laktoza (AlergijaId = 2)
(5, 2),

-- Jogurt (SastojakId = 6) → alergen Laktoza (AlergijaId = 2)
(6, 2),

-- Sir (SastojakId = 7) → alergen Laktoza (AlergijaId = 2)
(7, 2),

-- Tjestenina (SastojakId = 9) → alergen Gluten (AlergijaId = 1)
(9, 1),

-- Losos (SastojakId = 3) → alergen Riba (AlergijaId = 5)
(3, 5),

-- Orasi (SastojakId = 20) → alergen Orasi (AlergijaId = 3)
(20, 3),

-- Ovsena kaša (SastojakId = 17) → alergen Gluten (AlergijaId = 1)
(17, 1);
