USE NutriGoal;
GO

CREATE PROCEDURE sp_PreporuciRecepte
	@KorisnikId INT
AS
BEGIN
	
	DECLARE @CiljId INT

	SELECT
		@CiljId = CiljId
	FROM KorisnickiProfil
	WHERE KorisnikId = @KorisnikId

	SELECT R.*
	FROM Recepti R
		 JOIN ReceptCiljevi RC ON R.Id = RC.ReceptId
	WHERE RC.CiljId = @CiljId
		  AND R.Id NOT IN (
			 SELECT RS.ReceptId
			 FROM ReceptSastojci RS
			 JOIN SastojakAlergija SA ON RS.SastojakId = SA.SastojakId
			 JOIN KorisnikAlergije KA ON SA.AlergijaId = KA.AlergijaId
			 WHERE KA.KorisnikId = @KorisnikId
		  )
END

CREATE PROCEDURE sp_PretragaRecepata
    @Naziv NVARCHAR(255) = NULL,
    @KategorijaId INT = NULL,
    @CiljId INT = NULL,
    @MinKalorije DECIMAL(10,2) = NULL,
    @MaxKalorije DECIMAL(10,2) = NULL,
    @MaxVrijeme INT = NULL,
	@Sortiranje NVARCHAR(500) = 'Naziv' -- sorting po nazivu je default
AS
BEGIN
	
	SELECT R.*
	FROM Recepti R
	LEFT JOIN ReceptCiljevi RC ON R.Id = RC.ReceptId
	WHERE
		(@Naziv IS NULL OR R.Naziv LIKE '%' + @Naziv + '%')
	AND (@KategorijaId IS NULL OR R.KategorijaId = @KategorijaId)
	AND (@CiljId IS NULL OR RC.CiljId = @CiljId)
	AND (@MinKalorije IS NULL OR R.Kalorije >= @MinKalorije)
	AND (@MaxKalorije IS NULL OR R.Kalorije <= @MaxKalorije)
	AND (@MaxVrijeme IS NULL OR R.VrijemePripreme <= @MaxVrijeme)
	ORDER BY
		CASE WHEN @Sortiranje = 'Naziv' 
			THEN R.Naziv 
		END ASC,
		CASE WHEN @Sortiranje = 'Kalorije' 
			THEN R.Kalorije 
		END ASC,
		CASE WHEN @Sortiranje = 'Vrijeme' 
			THEN R.VrijemePripreme 
		END ASC	
END
GO

USE NutriGoal;
GO

-- Test 1 — bez filtera, sve recepte:
EXEC sp_PretragaRecepata

-- Test 2 — po nazivu:
EXEC sp_PretragaRecepata @Naziv = 'salata'

-- Test 3 — po kategoriji i max kalorijama:
EXEC sp_PretragaRecepata @KategorijaId = 1, @MaxKalorije = 500

-- Test 4 — sortiranje po kalorijama:
EXEC sp_PretragaRecepata @Sortiranje = 'Kalorije'