use QStatsRls
go
-- SELECT     COUNT(*) AS Cnt FROM        EHist WHERE     (EHist.SentOn IS NOT NULL)
SELECT      EMail.Company, COUNT(*) AS TtlSent
, format(24*AVG(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float)), 'N1') AS [Avg Sent On]
, format(24*( Max(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float))              -      Min(CAST(CAST(CONVERT(varCHAR, EHist.SentOn, 108) AS datetime) AS float))            ), 'N1') AS [Work Day (hr)]

, format(AVG(DATEDIFF(second, EHist.SentOn, EHist.EmailedAt)) / 3600.0, 'N2') AS [dT Avg (hr)]
, format(Max(DATEDIFF(second, EHist.SentOn, EHist.EmailedAt)) / 3600.0  - Min(DATEDIFF(second, EHist.SentOn, EHist.EmailedAt)) / 3600.0, 'N2') AS [dT range (hr)]
, Min(EHist.EmailedAt) as FrstTime
, Max(EHist.EmailedAt) as LastTime

FROM        EHist INNER JOIN 
            EMail ON EHist.EMailID = EMail.ID
WHERE     (CONVERT(varCHAR, EHist.SentOn, 108) <> '00:00:00')
GROUP BY EMail.Company
HAVING     (COUNT(*) > 10)
ORDER BY 1, 2 desc


SELECT     ID, AddedAt, FName, Company, PermBanReason, Notes
FROM        EMail
WHERE     (ID LIKE '%br_%') AND (NOT EXISTS
                      (SELECT     ID
                       FROM        EHist
                       WHERE     (EMail.ID = EMailID)))
ORDER BY ID


SELECT     COUNT(*) AS Cnt, RecivedOrSent, LetterSubject, MIN(EmailedAt) AS Expr1, MAX(EmailedAt) AS Expr2
FROM        EHist
GROUP BY LetterSubject, RecivedOrSent
ORDER BY Cnt DESC, LetterSubject