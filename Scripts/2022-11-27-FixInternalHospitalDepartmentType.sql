-- Update performed on 2022-11-27 between 14:30 and 14.40 UTC

-- 150 rows affected, after adding InternalHospitalDepartmentType all entries with 
-- HospitalDepartment=Internal (1) and HospitalDepartmentType=NormalUnit (2) are
-- updated to have InternalHospitalDepartmentType=Other (10000) as Unknows (0) would be invalid
UPDATE SentinelEntries SET InternalHospitalDepartmentType=10000 WHERE HospitalDepartment=1 AND HospitalDepartmentType=2 AND InternalHospitalDepartmentType=0
