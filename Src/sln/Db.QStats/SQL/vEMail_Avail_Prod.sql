USE [QStatsRls] -- Use this file - NOT the sql inline - for keeping track of logic (Nov 2019).
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[BadEmails] () -- last edit: Sep 29`19
RETURNS TABLE  
AS  
RETURN   
(  
 SELECT ID FROM EMail WHERE        
  (Company IN ('2marketsearch', '2x', '407ETR', 'adriatic', 'aei', 'airport', 'alerts', 'amigainformatics', 'avivacanada', 'bell', 'bellnet', 'bulletproof', 
  'ca', 'canadapost', 'canadarunningseries', 'carbonite', 'chatrwireless', 'cloud', 'commmunity neighbors', 'cooler', 
	'dock', 'docusign', 'e-mail', 'eteaminc', 'example', 'facebookappmail', 'garmin', 'github', 'goodtimesrunning', 
  'idctechnologies', 'imax', 'indeed', 'indeedemail', 'intel', 'invalidemail', 'iRun', 
	'jazz', 'kijiji', 'lexisnexis', 'linkedin', 'news', 'nityo', 'nokia', 'nymi', 'quantumworld', 
	'ramac', 'resource-logistics', 'richmondhilltoyota', 'runningroom', 'shatny', 'sleepcountry', 'stackoverflow', 'torontopolice', 
  'twitter', 'ukr', 'umca', 'vbuzzer', 'wietzestoyota'
  
  --, 'teksystems'							-- temp until Shadi finds me smthg. (Jul2019)
  --, 'prodigy', 'prodigylabs'	-- temp until Sep maybe
  --, 'randstad'								-- temp until David Kalmats is out of roles (Aug2019)
  --, 'litstaffing'							-- temp until Aug 19 (already sent them from GMAIL, will remind later)
  --, 'mobilelive'							-- temp until ready to deal with small shops

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
select getdate() Date ,   Count(*) BadEmlCount  from [BadEmails]() 
go
/*
Date				BadEmlCount
2019-08-14	1965 
2019-09-29	2308
*/
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
   IF 
     ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE RecivedOrSent = 'S' AND EMailID = @EmailID) IS NULL) OR 
     ((SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE RecivedOrSent = 'S' AND EMailID = @EmailID) < dbo.CurrentCampaignStart())
    
    -- (SELECT MAX(EmailedAt) AS LastSent FROM dbo.EHist WHERE RecivedOrSent = 'S' AND EMailID = @EmailID) > dbo.CurrentCampaignStart()
    return 1;
     
  return 0;  
END;  
GO  

ALTER VIEW [dbo].[vEMail_Avail_Prod]
AS
  SELECT TOP (100) PERCENT
    ID, FName, LName, Company, Phone, PermBanReason, Notes, AddedAt, DoNotNotifyOnAvailableForCampaignID AS DoNotNotifyForCampaignID,
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
  ORDER BY AddedAt DESC -- DOES NOT WORK !!! WHY ??? <= no worries: sort in C#!!! (Nov 2019)
GO

SELECT 
  ( SELECT Count(*) FROM EMail                                                  ) AS [All Emails     = ] ,
  ( SELECT Count(*) FROM EMail WHERE ID     IN ( SELECT * FROM dbo.BadEmails() )) AS [BadEmails()    + ] ,
  ( SELECT Count(*) FROM EMail WHERE ID NOT IN ( SELECT * FROM dbo.BadEmails() )) AS [Not BadEmails()  ] ,
  ((SELECT Count(*) FROM EMail WHERE ID NOT IN ( SELECT * FROM dbo.BadEmails() ))  -
   (select count(*) from [dbo].[vEMail_Avail_Prod]                              )) AS [Sent already    ] , 
  ( select count(*) from [dbo].[vEMail_Avail_Prod]                              )  AS [Remaining to be Sent], 
  dbo.CurrentCampaignStart()                                                     AS 'Last Campaign Start' ,
  dbo.CurrentCampaignID()                                                        AS 'Last Campaign ID' ,
  dbo.IsNotifiedForCurCmpgn('apigida@nymi.com')                                  AS 'notified already - apigida@nymi.com',
  dbo.IsNotifiedForCurCmpgn('nadine.pigida@outlook.com')                         AS 'notified already - nadine.pigida@ou'
GO


select top(7) *             from [dbo].[vEMail_Avail_Prod] ORDER BY LastSentAt --DESC -- where ReSendAfter IS NOT NULL --AND ReSendAfter < GETDATE() -- PermBanReason IS NOT NULL  -- //Company = 'umca'



--INSERT INTO EMail                                                (ID, FName, LName, Company, Phone, PermBanReason, DoNotNotifyOnAvailableForCampaignID, DoNotNotifyOnOffMarketForCampaignID,                          Notes, NotifyPriority, ReSendAfter, AddedAt, ModifiedAt)
SELECT     REPLACE(REPLACE(ID, '@bullhorn.com', ''), '=', '@') AS ID, FName, LName, Company, Phone, PermBanReason, DoNotNotifyOnAvailableForCampaignID, DoNotNotifyOnOffMarketForCampaignID, 'from ''' + ID + '''' AS Notes, NotifyPriority, ReSendAfter, AddedAt, ModifiedAt
FROM        EMail AS e1
WHERE     (ID LIKE '%=%@bullhorn.com%') AND (NOT EXISTS
                      (SELECT     ID, FName, LName, Company, Phone, PermBanReason, DoNotNotifyOnAvailableForCampaignID, DoNotNotifyOnOffMarketForCampaignID, Notes, NotifyPriority, ReSendAfter, AddedAt, ModifiedAt
                       FROM        EMail AS EMail_1
                       WHERE     (ID = REPLACE(REPLACE(e1.ID, '@bullhorn.com', ''), '=', '@'))))
