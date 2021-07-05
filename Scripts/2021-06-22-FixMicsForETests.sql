-- Update performed on 2021-22-06 between 21:00 and 21.20 UTC

-- Fluconazole test with now invalid steps
SELECT * from AntimicrobialSensitivityTest where TestingMethod = 4 and AntifungalAgent = 400 and MinimumInhibitoryConcentration not in (0.016,0.023,0.032,0.047,0.064,0.094,0.125,0.19,0.25,0.38,0.5,0.75,1,1.5,2,3,4,6,8,12,16,24,32,48,64,96,128,192,256,256.001)

-- 3 rows updated
UPDATE AntimicrobialSensitivityTest set MinimumInhibitoryConcentration=0.125 where MinimumInhibitoryConcentration=0.12 and TestingMethod=4 and AntifungalAgent=400


-- Anidulafungin test with now invalid steps
SELECT * from AntimicrobialSensitivityTest where TestingMethod = 4 and AntifungalAgent = 200 and MinimumInhibitoryConcentration not in (0.002,0.003,0.004,0.006,0.008,0.012,0.016,0.023,0.032,0.047,0.064,0.094,0.125,0.19,0.25,0.38,0.5,0.75,1,1.5,2,3,4,6,8,12,16,24,32,32.001)

-- 85 rows updated
UPDATE AntimicrobialSensitivityTest set MinimumInhibitoryConcentration=0.016 where MinimumInhibitoryConcentration=0.015 and TestingMethod=4 and AntifungalAgent=200

-- 9 rows updated
UPDATE AntimicrobialSensitivityTest set MinimumInhibitoryConcentration=0.032 where MinimumInhibitoryConcentration=0.03 and TestingMethod=4 and AntifungalAgent=200  and ClinicalBreakpointId<278

-- 2 rows affected, update of resistance from 0 -> 2 (SID 140 and 136)
UPDATE AntimicrobialSensitivityTest set MinimumInhibitoryConcentration=0.032, Resistance = 2  where MinimumInhibitoryConcentration=0.03 and TestingMethod=4 and AntifungalAgent=200 and ClinicalBreakpointId=278
