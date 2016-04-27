/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_NewsletterSubscribers
	(
	Id int NOT NULL IDENTITY (1, 1),
	Email nvarchar(128) NOT NULL,
	Unsubscribed bit NOT NULL,
	IsOrganisation bit NOT NULL,
	Name nvarchar(128) NULL,
	Address nvarchar(256) NULL,
	CountryId int NOT NULL,
	UnsubscribeToken uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_NewsletterSubscribers SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_NewsletterSubscribers ADD CONSTRAINT
	DF_NewsletterSubscribers_Unsubscribed DEFAULT 0 FOR Unsubscribed
GO
ALTER TABLE dbo.Tmp_NewsletterSubscribers ADD CONSTRAINT
	DF_NewsletterSubscribers_UnsubscribeToken DEFAULT newId() FOR UnsubscribeToken
GO
SET IDENTITY_INSERT dbo.Tmp_NewsletterSubscribers OFF
GO
IF EXISTS(SELECT * FROM dbo.NewsletterSubscribers)
	 EXEC('INSERT INTO dbo.Tmp_NewsletterSubscribers (Email)
		SELECT Email FROM dbo.NewsletterSubscribers WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.NewsletterSubscribers
GO
EXECUTE sp_rename N'dbo.Tmp_NewsletterSubscribers', N'NewsletterSubscribers', 'OBJECT' 
GO

ALTER TABLE dbo.NewsletterSubscribers ADD CONSTRAINT
	PK_NewsletterSubscribers PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.NewsletterSubscribers ADD CONSTRAINT
	UniqueEmail UNIQUE NONCLUSTERED 
	(
	Email
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.NewsletterSubscribers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


BEGIN TRANSACTION
GO
CREATE TABLE dbo.Receipt
	(
	Id int NOT NULL IDENTITY (1, 1),
	NewletterSubscriberId int NOT NULL,
	DateReceived date NOT NULL,
	DateSent date NOT NULL,
	Amount money NOT NULL,
	TransferMethodId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Receipt ADD CONSTRAINT
	PK_Receipt PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Receipt ADD CONSTRAINT
	FK_Receipt_NewsletterSubscribers FOREIGN KEY
	(
	NewletterSubscriberId
	) REFERENCES dbo.NewsletterSubscribers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.Receipt SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Receipt ADD CONSTRAINT
	UniquePersonDate UNIQUE NONCLUSTERED 
	(
	NewletterSubscriberId,
	DateReceived
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Receipt ADD CONSTRAINT
	CK_Receipt_DateSentSequentialAfterReceived CHECK (DateSent >= DateReceived)
GO
ALTER TABLE dbo.Receipt SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


BEGIN TRANSACTION
GO
CREATE TABLE dbo.TransferMethod
	(
	Id int NOT NULL IDENTITY (1, 1),
	Description nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.TransferMethod ADD CONSTRAINT
	PK_TransferMethod PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.TransferMethod SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Receipt ADD CONSTRAINT
	FK_Receipt_TransferMethod FOREIGN KEY
	(
	TransferMethodId
	) REFERENCES dbo.TransferMethod
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TransferMethod ADD CONSTRAINT
	IX_TransferMethod_UniqueDescription UNIQUE NONCLUSTERED 
	(
	Description
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Receipt SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
INSERT INTO [Hearts4Kids].[dbo].[TransferMethod]
           ([Description])
     VALUES
           ('Direct Bank Transfer'),('GiveALittle'),('Goods In Kind')
GO

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Countries
	(
	Id int NOT NULL,
	Name nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Countries ADD CONSTRAINT
	PK_Countries PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Countries ADD CONSTRAINT
	IX_TransferMethod_UniqueName UNIQUE NONCLUSTERED 
	(
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	
ALTER TABLE dbo.NewsletterSubscribers ADD CONSTRAINT
	FK_NewsletterSubscribers_Countries FOREIGN KEY
	(
	CountryId
	) REFERENCES dbo.Countries
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.Countries SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


INSERT INTO [Hearts4Kids].[dbo].[Countries]
           ([Id],[Name])
     VALUES
           (0,'Unknown'),(1,'New Zealand'),(2,'Australia'),(3,'USA'),(4,'Fiji'),(100,'Other')
GO