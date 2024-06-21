-- Update performed on 2023-06-04 ~ 10:30 UTC

-- 8 sentinel entries affected according to this query 
-- data change was reviewed by laboratory
SELECT Id, SenderLaboratoryNumber, Year, YearlySequentialEntryNumber FROM SentinelEntries WHERE ID in (SELECT SentinelEntryId From AntimicrobialSensitivityTest WHERE ID IN (11808, 7584, 7576, 7512, 6851, 3747, 2942, 2926))
