USE NutriGoal;
GO

CREATE TRIGGER tr_IzracunajBMR_TDEE
ON KorisnickiProfil
AFTER INSERT, UPDATE
AS
BEGIN
	
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

	DECLARE @KorisnikId INT
	DECLARE @Pol CHAR(1)
	DECLARE @Visina DECIMAL(5,2)
	DECLARE @Tezina DECIMAL(5,2)
	DECLARE @Godine INT
	DECLARE @NivoAktivnosti INT
	DECLARE @CiljId INT
	DECLARE @BMR DECIMAL(10,2)
	DECLARE @TDEE DECIMAL(10,2)
	DECLARE @FaktorAktivnosti DECIMAL(5,3)
	DECLARE @FaktorCilja DECIMAL(4,2)
	DECLARE @PreporucenoKalorija DECIMAL(10,2)

	SELECT 
		@KorisnikId = KorisnikId,
		@Pol = Pol,
		@Visina = Visina,
		@Tezina = Tezina,
		@Godine = DATEDIFF(YEAR, DatumRodjenja, GETDATE()),
		@NivoAktivnosti = NivoAktivnosti,
		@CiljId = CiljId
	from inserted

	IF @Pol is NULL OR @Visina is NULL OR @Tezina is NULL OR @Godine is NULL
		RETURN
	
	SET @BMR = dbo.fn_IzracunajBMR(@Pol, @Visina, @Tezina, @Godine)

	SET @FaktorAktivnosti = CASE @NivoAktivnosti
		WHEN 1 THEN 1.200
		WHEN 2 THEN 1.375
		WHEN 3 THEN 1.550
		WHEN 4 THEN 1.725
		ELSE 1.200
	END

	SET @TDEE = @BMR * @FaktorAktivnosti
	
	SET @FaktorCilja = 1.00

	IF @CiljId IS NOT NULL
		SELECT 
			@FaktorCilja = Faktor
		FROM Ciljevi 
		WHERE Id = @CiljId

	SET @PreporucenoKalorija = @TDEE * @FaktorCilja

	UPDATE KorisnickiProfil
		SET BMR=@BMR, TDEE=@TDEE, PreporucenoKalorija=@PreporucenoKalorija
	WHERE KorisnikId=@KorisnikId

END

CREATE TRIGGER tr_AzurirajNutritivneVrijednosti
ON ReceptSastojci
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	
	DECLARE @ReceptId INT
	DECLARE @Kalorije DECIMAL(10,2)
	DECLARE @Proteini DECIMAL(10,2)
	DECLARE @UgljeniHidrati DECIMAL(10,2)
	DECLARE @Masti DECIMAL(10,2)

	SELECT 
		@ReceptId = COALESCE(I.ReceptId, D.ReceptId)
	FROM inserted I FULL OUTER JOIN deleted D
		ON I.ReceptId = D.ReceptId

	SELECT 
		@Kalorije = Kalorije,
		@Proteini = Proteini,
		@UgljeniHidrati = UgljeniHidrati,
		@Masti = Masti
	FROM dbo.fn_NutritivneVrijednostiRecepta(@ReceptId)

	UPDATE Recepti SET
		Kalorije = @Kalorije,
		Proteini = @Proteini,
		UgljeniHidrati = @UgljeniHidrati,
		Masti = @Masti
	WHERE id = @ReceptId
END
