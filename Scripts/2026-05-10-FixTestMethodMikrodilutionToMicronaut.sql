-- Update prepared on 2026-05-10
-- Fix issue #218: sentinel entries with mislabeled AntimicrobialSensitivityTest testing method
-- from Microdilution (1, displayed as "Mikrodilution") to Micronaut (3)

-- Verification query (run before UPDATE)
SELECT *
FROM AntimicrobialSensitivityTest
WHERE TestingMethod = 1 -- Microdilution (displayed as "Mikrodilution")
  AND SentinelEntryId IN (
      SELECT Id
      FROM SentinelEntries
      WHERE (Id BETWEEN 1304 AND 1598)
         OR (Id BETWEEN 1683 AND 1693)
         OR (Id BETWEEN 1795 AND 1801)
         OR Id = 1942
         OR (Id BETWEEN 2788 AND 2805)
  );

-- ~NNN rows affected (verify with SELECT above before running)
UPDATE AntimicrobialSensitivityTest
SET TestingMethod = 3 -- Micronaut
WHERE TestingMethod = 1 -- Microdilution (displayed as "Mikrodilution")
  AND SentinelEntryId IN (
      SELECT Id
      FROM SentinelEntries
      WHERE (Id BETWEEN 1304 AND 1598)
         OR (Id BETWEEN 1683 AND 1693)
         OR (Id BETWEEN 1795 AND 1801)
         OR Id = 1942
         OR (Id BETWEEN 2788 AND 2805)
  );
