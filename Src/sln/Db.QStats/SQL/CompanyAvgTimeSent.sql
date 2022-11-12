use QStatsRls
go

SELECT      EMail.Company, COUNT(*) AS TtlSent
, format(24*AVG(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float)), 'N1') AS [Avg Sent On]
, format(24*( Max(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float))              -      Min(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float))            ), 'N1') AS [Work Day (hr)]

, format(AVG(DATEDIFF(second, EHist.SentOn, EHist.EmailedAt)) / 3600.0, 'N2') AS [dT (hr)]
, format(Max(DATEDIFF(second, EHist.SentOn, EHist.EmailedAt)) / 3600.0  - Min(DATEDIFF(second, EHist.SentOn, EHist.EmailedAt)) / 3600.0, 'N2') AS [dT range (hr)]

FROM        EHist INNER JOIN 
            EMail ON EHist.EMailID = EMail.ID
WHERE     (CONVERT(varCHAR, EHist.SentOn, 108) <> '00:00:00')
GROUP BY EMail.Company
HAVING     (COUNT(*) > 4)
ORDER BY 3, EMail.Company
