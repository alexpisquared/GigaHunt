SELECT     CAST(AVG(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float)) AS datetime) AS AvgTime, EMail.Company, COUNT(*) AS TtlSent
FROM        EHist INNER JOIN 
            EMail ON EHist.EMailID = EMail.ID
WHERE     (CONVERT(varCHAR, EHist.SentOn, 108) <> '00:00:00')
GROUP BY EMail.Company
HAVING     (COUNT(*) > 0)
ORDER BY AvgTime, EMail.Company