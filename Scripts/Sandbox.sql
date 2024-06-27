DELETE FROM ClinicalBreakpoints

SELECT * FROM ClinicalBreakpoints

SELECT * FROM SentinelEntries

-- Lab number in box
SELECT 'SN-' + CAST(Year AS VARCHAR(4)) + '-' + RIGHT('0000'+CAST(YearlySequentialEntryNumber AS VARCHAR(4)),4) FROm SentinelEntries where CryoBoxNumber=1

-- Cryo Box Slot Availability
Declare @T Table (CryoBoxNumber INT, CryoBoxSlot INT)
Insert @T Exec GenerateBoxNumberCompartmentNumberSequence
Select CryoBoxNumber as 'Cryo Box', count(*) as 'freie Plätze' from @T group by CryoBoxNumber

-- Cryo Box Missing Slot Finder
CREATE OR ALTER PROCEDURE GenerateBoxNumberCompartmentNumberSequence 
AS
BEGIN
  DECLARE @boxSize INT
  DECLARE @lastBoxSize INT
  DECLARE @lastBoxNumber INT
  DECLARE @currentSlotNumber INT
  DECLARE @lastSlotNumber INT
  DECLARE @currentBoxNumber INT
  DECLARE @allPossibleBoxes table(CryoBoxNumber INT, CryoBoxSlot INT)
  
  SELECT @lastBoxNumber = MAX(CryoBoxNumber) FROM SentinelEntries
  SELECT @lastSlotNumber = MAX(CryoBoxSlot) FROM SentinelEntries WHERE CryoBoxNumber=@lastBoxNumber
  SELECT @boxSize = MAX(CryoBoxSlot) FROM SentinelEntries
  
  SET @currentSlotNumber = 1;
  SET @currentBoxNumber = 1;
  WHILE @currentBoxNumber <= @lastBoxNumber
  BEGIN
    INSERT @allPossibleBoxes VALUES (@currentBoxNumber, @currentSlotNumber);

    IF @currentBoxNumber = @lastBoxNumber AND @currentSlotNumber = @lastSlotNumber
    BEGIN
      BREAK;  -- Reached final box slot
    END
    
    SET @currentSlotNumber = @currentSlotNumber + 1;

    IF @currentSlotNumber > @boxSize
    BEGIN
      SET @currentSlotNumber = 1;
      SET @currentBoxNumber = @currentBoxNumber + 1;
    END;
  END;
  select * from @allPossibleBoxes
  EXCEPT
  SELECT CryoBoxNumber,CryoBoxSlot FROM SentinelEntries;
END;