DECLARE @Relabel TABLE
(
    ID INT NOT NULL,
    [Year] INT NOT NULL,
    SenderLaboratoryNumber NVARCHAR(255) NOT NULL,
    ProtectKey NVARCHAR(255) NULL,
    Cryobox NVARCHAR(255) NULL,
    BeforeYearlySequentialEntryNumber INT NOT NULL,
    AfterYearlySequentialEntryNumber INT NOT NULL,
    BeforeSNNumber NVARCHAR(50) NOT NULL,
    AfterSNNumber NVARCHAR(50) NOT NULL
);

;WITH EntriesToMove AS
(
    SELECT
        se.ID,
        parsed.LabNo,
        NewYearlySequentialEntryNumber =
            CASE
                WHEN UPPER(se.SenderLaboratoryNumber) LIKE 'IMMI %' AND parsed.LabNo BETWEEN 34 AND 98
                    THEN se.YearlySequentialEntryNumber - 2
                WHEN se.[Year] = 2023 AND se.YearlySequentialEntryNumber = 137
                    THEN 202
                WHEN se.[Year] = 2023 AND se.YearlySequentialEntryNumber = 138
                    THEN 203
            END
    FROM SentinelEntries se
    CROSS APPLY (VALUES (TRY_CAST(SUBSTRING(se.SenderLaboratoryNumber, 6, 10) AS INT))) parsed(LabNo)
    WHERE
        (
            se.[Year] = 2023
            AND UPPER(se.SenderLaboratoryNumber) LIKE 'IMMI %'
            AND parsed.LabNo BETWEEN 34 AND 98
        )
        OR
        (
            se.[Year] = 2023
            AND se.YearlySequentialEntryNumber IN (137, 138)
            AND UPPER(se.SenderLaboratoryNumber) NOT LIKE 'IMMI %'
        )
)
UPDATE se
SET se.YearlySequentialEntryNumber = etm.NewYearlySequentialEntryNumber
OUTPUT
    inserted.ID,
    inserted.[Year],
    inserted.SenderLaboratoryNumber,
    inserted.ProtectKey,
    CONCAT('SN-', inserted.CryoBoxNumber) AS Cryobox,
    deleted.YearlySequentialEntryNumber AS BeforeYearlySequentialEntryNumber,
    inserted.YearlySequentialEntryNumber AS AfterYearlySequentialEntryNumber,
    CONCAT('SN-', deleted.[Year], '-', RIGHT('0000' + CAST(deleted.YearlySequentialEntryNumber AS VARCHAR(4)), 4)) AS BeforeSNNumber,
    CONCAT('SN-', inserted.[Year], '-', RIGHT('0000' + CAST(inserted.YearlySequentialEntryNumber AS VARCHAR(4)), 4)) AS AfterSNNumber
INTO @Relabel
FROM SentinelEntries se
INNER JOIN EntriesToMove etm ON etm.ID = se.ID;

SELECT
    ID,
    [Year],
    SenderLaboratoryNumber,
    ProtectKey,
    Cryobox,
    BeforeYearlySequentialEntryNumber,
    AfterYearlySequentialEntryNumber,
    BeforeSNNumber,
    AfterSNNumber
FROM @Relabel
ORDER BY BeforeYearlySequentialEntryNumber;