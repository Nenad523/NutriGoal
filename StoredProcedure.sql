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
