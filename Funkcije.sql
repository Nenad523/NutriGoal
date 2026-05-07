USE NutriGoal;
GO

CREATE FUNCTION fn_IzracunajBMR
(
	@Pol CHAR(1),
	@Visina DECIMAL(5,2),
	@Tezina DECIMAL(5,2),
	@Godine INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
	DECLARE @BMR DECIMAL(10,2)
		
		IF @Pol = 'M'
			SET @BMR = (10 * @Tezina) + 
					   (6.25 * @Visina) - 
					   (5 * @Godine) + 5
		ELSE
			SET @BMR = (10 * @Tezina) + 
					   (6.25 * @Visina) - 
					   (5 * @Godine) - 161

	RETURN @BMR
END

CREATE FUNCTION fn_NutritivneVrijednostiRecepta
(
    @ReceptId INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        ISNULL(SUM(S.KalorijeNa100g * RS.KolicinaG / 100), 0) AS Kalorije,
        ISNULL(SUM(S.ProteinNa100g * RS.KolicinaG / 100), 0) AS Proteini,
        ISNULL(SUM(S.UHNa100g * RS.KolicinaG / 100), 0) AS UgljeniHidrati,
        ISNULL(SUM(S.MastiNa100g * RS.KolicinaG / 100), 0) AS Masti
    FROM ReceptSastojci RS
    JOIN Sastojci S ON RS.SastojakId = S.Id
    WHERE RS.ReceptId = @ReceptId
)
GO