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

END
GO