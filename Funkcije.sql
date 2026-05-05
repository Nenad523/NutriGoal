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

CREATE FUNCTION fn_UkupnoKalorijaRecepta
(
    @ReceptId INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Ukupno DECIMAL(10,2)

    SELECT @Ukupno = SUM(S.KalorijeNa100g * RS.KolicinaG / 100)
    FROM ReceptSastojci RS
    JOIN Sastojci S ON RS.SastojakId = S.Id
    WHERE RS.ReceptId = @ReceptId

    RETURN ISNULL(@Ukupno, 0)
END
GO

SELECT dbo.fn_UkupnoKalorijaRecepta(1)