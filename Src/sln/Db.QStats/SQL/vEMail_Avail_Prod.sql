USE [QStatsRls] -- Use this file - NOT THE SQL INLINE - for keeping track of logic (2019-11). Revisited on 2023-11.
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
ALTER FUNCTION [dbo].[BadEmails] () RETURNS TABLE AS                   -- last edit: 2023-11-23
  RETURN (  
	 SELECT ID FROM EMail WHERE        
	  (Company IN ('2marketsearch', '2x', '407ETR', 'adriatic', 'aei', 'airport', 'alerts', 'amigainformatics', 'avivacanada', 'bell', 'bellnet', 'bulletproof', 
	  'ca', 'canadapost', 'canadarunningseries', 'carbonite', 'chatrwireless', 'cloud', 'commmunity neighbors', 'cooler', 
		'dock', 'docusign', 'e-mail', 'eteaminc', 'example', 'facebookappmail', 'garmin', 'github', 'goodtimesrunning', 
	  'idctechnologies', 'imax', 'indeed', 'indeedemail', 'intel', 'invalidemail', 'iRun', 
		'jazz', 'kijiji', 'lexisnexis', 'linkedin', 'news', 'nityo', 'nokia', 'nymi', 'quantumworld', 
		'ramac', 'resource-logistics', 'richmondhilltoyota', 'runningroom', 'shatny', 'sleepcountry', 'stackoverflow', 'torontopolice', 
	  'twitter', 'ukr', 'umca', 'vbuzzer', 'wietzestoyota', 'here'
	  , 'botsford'   -- temp: remove after a talk or after Dec 3, 2023
	  , 'botsfordit' -- temp: remove after a talk or after Dec 3, 2023
	  )) OR
	  (ID LIKE '%unsubscribe%') OR
	  (ID LIKE '%no-reply%') OR
	  (ID LIKE '%noreply%') OR
	  (ID LIKE '%UnKnwn%') OR
	  (ID LIKE '%@alquemy%') OR
	  (ID LIKE '%@amazon%') OR
	  (ID LIKE '%@cmail%') OR
	  (ID LIKE '%@e.%') OR
	  (ID LIKE '%@e-mail%') OR
	  (ID LIKE '%@email%') OR
	  (ID LIKE '%@mail%') OR
	  (ID LIKE '%@mg.%') OR
	  (ID LIKE '%@reply%') OR
	  (ID LIKE '%''%') OR
	  (ID LIKE '%dobrin%') OR
	  (ID LIKE '%karpov%') OR
	  (ID LIKE '%+%') OR
	  (ID LIKE '%*%') OR
	  (ID LIKE '%/%') OR
	  len(isnull(PermBanReason,'')) > 1  --!!! PermBanReason IS NOT NULL == this is bad: makes '' banned!!! Sep 2019.
);  
go
select getdate() Date,   Count(*) BadEmlCount  from [BadEmails]() 
go
-- Date		  BadEmlCount
-- 2019-08-14	 1,965 
-- 2019-09-29	 2,308
-- 2023-11-23	10,632
-- 2023-11-24   10,653

ALTER FUNCTION CurrentCampaignStart () --IF OBJECT_ID (N'CurrentCampaignStart', N'FN') IS NOT NULL    DROP FUNCTION CurrentCampaignStart;  GO
RETURNS datetime --WITH EXECUTE AS CALLER  
AS  
BEGIN  
     RETURN (SELECT MAX(CampaignStart) FROM dbo.Campaign); 
END;  
GO  

ALTER FUNCTION CurrentCampaignID () --IF OBJECT_ID (N'CurrentCampaignID', N'FN') IS NOT NULL    DROP FUNCTION CurrentCampaignID;  GO
RETURNS int --WITH EXECUTE AS CALLER  
AS  
BEGIN  
     RETURN (SELECT Id FROM dbo.Campaign WHERE CampaignStart = dbo.CurrentCampaignStart()); 
END;  
GO  

ALTER FUNCTION dbo.IsNotifiedForCurCmpgn (@EmailID varchar(256))  
RETURNS BIT
AS  
BEGIN  
   IF ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE RecivedOrSent = 'S' AND EMailID = @EmailID) IS NULL) OR 
      ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE RecivedOrSent = 'S' AND EMailID = @EmailID) < dbo.CurrentCampaignStart())
    -- (SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE RecivedOrSent = 'S' AND EMailID = @EmailID) > dbo.CurrentCampaignStart()
    return 1;
     
  return 0;  
END;  
GO  

ALTER VIEW [dbo].[vEMail_Avail_Prod]
AS
  SELECT -- TOP (100) PERCENT
    ISNULL(ROW_NUMBER() OVER (ORDER BY AddedAt desc), 0)                                                 AS RowNumberForEfId,
    NotifyPriority, ID, FName, LName, Company, Phone, PermBanReason, Notes, AddedAt, DoNotNotifyOnAvailableForCampaignID AS DoNotNotifyForCampaignID,
    (SELECT MAX(CampaignStart) FROM dbo.Campaign)                                                                             AS CurrentCampaignStart,
    (SELECT Id FROM dbo.Campaign WHERE (CampaignStart = (SELECT MAX(CampaignStart) FROM dbo.Campaign AS Campaign_6)))         AS LastCampaignID,
    (SELECT COUNT(*)       FROM dbo.EHist WHERE (RecivedOrSent = 'S') AND (LetterBody NOT LIKE 'std%') AND (EMailID = em.ID)) AS MyReplies,
    (SELECT MAX(EmailedAt) FROM dbo.EHist WHERE (RecivedOrSent = 'S') AND (LetterBody NOT LIKE 'std%') AND (EMailID = em.ID)) AS LastSentAt,
    (SELECT MAX(EmailedAt) FROM dbo.EHist WHERE (RecivedOrSent<> 'S') AND (LetterBody NOT LIKE 'std%') AND (EMailID = em.ID)) AS LastRepliedAt,
    (SELECT COUNT(*)       FROM dbo.EHist WHERE (RecivedOrSent = 'S')                                  AND (EMailID = em.ID)) AS TtlSends,
    (SELECT COUNT(*)       FROM dbo.EHist WHERE (RecivedOrSent = 'R')                                  AND (EMailID = em.ID)) AS TtlRcvds
  FROM dbo.EMail AS em
  WHERE 
    ID NOT IN (SELECT * FROM BadEmails()) AND 
    (dbo.CurrentCampaignID() <> ISNULL(DoNotNotifyOnAvailableForCampaignID, -1)) AND -- not banned for this campaign
    ( 
      --dbo.IsNotifiedForCurCmpgn(em.ID) <> 1 -- performance issues !!! //todo:
      ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE (RecivedOrSent = 'S') AND (EMailID = em.ID)) IS NULL) OR 
      ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE (RecivedOrSent = 'S') AND (EMailID = em.ID)) < dbo.CurrentCampaignStart())
    )
     -- OR ReSendAfter IS NOT NULL AND ReSendAfter < GETDATE()
  -- ORDER BY NotifyPriority, AddedAt DESC -- !!! DOES NOT WORK !!! WHY ??? <= MUST sort in C#!!! 
GO

CREATE OR ALTER VIEW [dbo].[vEMailId_Avail_Prod]
AS
  SELECT ID FROM dbo.EMail AS em
  WHERE 
    ID NOT IN (SELECT * FROM BadEmails()) AND 
    (dbo.CurrentCampaignID() <> ISNULL(DoNotNotifyOnAvailableForCampaignID, -1)) AND -- not banned for this campaign
    (((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE (RecivedOrSent = 'S') AND (EMailID = em.ID)) IS NULL) OR 
     ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE (RecivedOrSent = 'S') AND (EMailID = em.ID)) < dbo.CurrentCampaignStart()))
GO

-- Broadcast Status/Progress:                     2023-11-23
SELECT getdate() Date, 
  ( SELECT Count(*) FROM EMail                                                  )  AS [AllEmails =],
  ( SELECT Count(*) FROM EMail WHERE ID     IN ( SELECT * FROM dbo.BadEmails() ))  AS [BadEmails +],
  ( SELECT Count(*) FROM EMail WHERE ID NOT IN ( SELECT * FROM dbo.BadEmails() ))  AS [!Bad ones -],
  ((SELECT Count(*) FROM EMail WHERE ID NOT IN ( SELECT * FROM dbo.BadEmails() ))  -
   (select count(*) from [dbo].[vEMail_Avail_Prod]                              )) AS [Sent      =], 
  ( select count(*) from [dbo].[vEMail_Avail_Prod]                              )  AS [Left], 
  dbo.CurrentCampaignStart()                                                     AS 'Last Campaign Start' ,
  dbo.CurrentCampaignID()                                                        AS 'Last Campaign ID' ,
  dbo.IsNotifiedForCurCmpgn('apigida@nymi.com')                                  AS 'notified already - apigida@nymi.com',
  dbo.IsNotifiedForCurCmpgn('nadine.pigida@outlook.com')                         AS 'notified already - nadine.pigida@ou'
GO
--Date                    AllEmails = BadEmails + !Bad ones - Sent   =    Left        Last Campaign Start     Last Campaign ID notified already - apigida@nymi.com notified already - nadine.pigida@ou
------------------------- ----------- ----------- ----------- ----------- ----------- ----------------------- ---------------- ----------------------------------- -----------------------------------
--2023-11-23 11:15:11.527 13421       10632       2789        21          2768        2023-11-11 00:00:00.000 12               1                                   1


--CREATE NONCLUSTERED INDEX EmailerViewAcceleratorIndex ON dbo.EHist (EMailID, RecivedOrSent) -- solves 8 min cold start for the vEMail_Avail_Prod view!!!

--INSERT INTO EMail                                              (ID, FName, LName, Company, Phone, PermBanReason, DoNotNotifyOnAvailableForCampaignID, DoNotNotifyOnOffMarketForCampaignID,                          Notes, NotifyPriority, ReSendAfter, AddedAt, ModifiedAt)
SELECT     REPLACE(REPLACE(ID, '@bullhorn.com', ''), '=', '@') AS ID, FName, LName, Company, Phone, PermBanReason, DoNotNotifyOnAvailableForCampaignID, DoNotNotifyOnOffMarketForCampaignID, 'from ''' + ID + '''' AS Notes, NotifyPriority, ReSendAfter, AddedAt, ModifiedAt
FROM        EMail AS e1
WHERE     (ID LIKE '%=%@bullhorn.com%') AND (NOT EXISTS
                      (SELECT     ID, FName, LName, Company, Phone, PermBanReason, DoNotNotifyOnAvailableForCampaignID, DoNotNotifyOnOffMarketForCampaignID, Notes, NotifyPriority, ReSendAfter, AddedAt, ModifiedAt
                       FROM        EMail AS EMail_1
                       WHERE     (ID = REPLACE(REPLACE(e1.ID, '@bullhorn.com', ''), '=', '@'))))

-- run this periodically to update priorities and expedite email broadcast:
UPDATE EMail SET NotifyPriority = isnull((
	SELECT     1000 * DATEDIFF(day, MAX(EmailedAt), GETDATE()) / COUNT(*) AS Priority
	FROM        EHist
	WHERE     (RecivedOrSent = 'R') AND (EMailID = EMail.ID)
	GROUP BY EMailID
), 130000)
WHERE    EMail.PermBanReason is null and (Notes NOT LIKE '#TopPriority#%')
-- */

-- review latest sends:
SELECT TOP (16) EHist.*, EMail.FName, EMail.Notes, EMail.NotifyPriority FROM EHist INNER JOIN EMail ON EHist.EMailID = EMail.ID where EMail.PermBanReason is null ORDER BY EHist.EmailedAt DESC
SELECT TOP (16) *, (select count(*) from EHist where EHist.EMailID = EMail.ID ) as aa FROM EMail where EMail.PermBanReason is null ORDER BY aa
