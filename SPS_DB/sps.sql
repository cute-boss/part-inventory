CREATE TABLE UserProfile
(
	UserId int IDENTITY(1,1) NOT NULL,
	UserName nvarchar(20) NOT NULL,
	StaffName nvarchar(80) NOT NULL,
	StaffNo nvarchar(12) NOT NULL,
	Email nvarchar(100) NOT NULL,
	IsActive bit NOT NULL DEFAULT (1),
	CONSTRAINT PK_UserProfile PRIMARY KEY (UserId),
	CONSTRAINT UQ_UserProfile_UserName UNIQUE (UserName),
	CONSTRAINT UQ_UserProfile_StaffNo UNIQUE (StaffNo),
);
GO

SET IDENTITY_INSERT UserProfile ON 
INSERT UserProfile (UserId, UserName, StaffName, StaffNo, Email, IsActive) VALUES (1, N'admin', 'ADMIN', '000000', 'ADMIN@SPS.COM', 1)
SET IDENTITY_INSERT UserProfile OFF
;
GO

CREATE TABLE webpages_Membership
(
	UserId int NOT NULL,
	CreateDate datetime NULL,
	ConfirmationToken nvarchar(128) NULL,
	IsConfirmed bit NULL DEFAULT (0),
	LastPasswordFailureDate datetime NULL,
	PasswordFailuresSinceLastSuccess int NOT NULL DEFAULT (0),
	Password nvarchar(128) NOT NULL,
	PasswordChangedDate datetime NULL,
	PasswordSalt nvarchar(128) NOT NULL,
	PasswordVerificationToken nvarchar(128) NULL,
	PasswordVerificationTokenExpirationDate datetime NULL,
	CONSTRAINT PK_webpages_Membership PRIMARY KEY (UserId),
	CONSTRAINT FK_webpages_Membership_UserId FOREIGN KEY (UserId) 
		REFERENCES UserProfile(UserId) ON DELETE CASCADE,
);
GO

INSERT webpages_Membership (UserId, CreateDate, ConfirmationToken, IsConfirmed, LastPasswordFailureDate, PasswordFailuresSinceLastSuccess, Password, PasswordChangedDate, PasswordSalt, PasswordVerificationToken, PasswordVerificationTokenExpirationDate) VALUES (1, GETUTCDATE(), NULL, 1, NULL, 0, N'AMm6gLNGhvv+9qN4kh8T7I+XE2RhuIosI5kBfUXxE137VQIkMTnC4v6u4VoD3y8zHQ==', GETUTCDATE(), N'', NULL, NULL);
GO

CREATE TABLE webpages_Roles
(
	RoleId int IDENTITY(1,1) NOT NULL,
	RoleName nvarchar(20) NOT NULL,
	CONSTRAINT PK_webpages_Roles PRIMARY KEY (RoleId),
	CONSTRAINT UQ_webpages_Roles_RoleName UNIQUE (RoleName),
);
GO

SET IDENTITY_INSERT webpages_Roles ON 
INSERT webpages_Roles (RoleId, RoleName) VALUES (1, N'ADMIN')
INSERT webpages_Roles (RoleId, RoleName) VALUES (2, N'USER')
SET IDENTITY_INSERT webpages_Roles OFF
;
GO

CREATE TABLE webpages_UsersInRoles
(
	UserId int NOT NULL,
	RoleId int NOT NULL,
	CONSTRAINT PK_webpages_UsersInRoles PRIMARY KEY (UserId, RoleId),
	CONSTRAINT FK_webpages_UsersInRoles_UserId FOREIGN KEY (UserId) 
		REFERENCES UserProfile(UserId) ON DELETE CASCADE,
	CONSTRAINT FK_webpages_UsersInRoles_RoleId FOREIGN KEY (RoleId) 
		REFERENCES webpages_Roles(RoleId),
);
GO

INSERT webpages_UsersInRoles (UserId, RoleId) VALUES (1, 1);
GO

CREATE TABLE Session
(
	UserId int NOT NULL,
	SessionId nvarchar(30) NOT NULL,
	LastSessionTime datetime NOT NULL,
	CONSTRAINT PK_Session PRIMARY KEY (UserId),
	CONSTRAINT FK_Session_UserId FOREIGN KEY (UserId) 
		REFERENCES UserProfile(UserId) ON DELETE CASCADE,
)
;
GO

CREATE TABLE Log
(
	LogId int IDENTITY(1,1) NOT NULL,
	LogTime DateTime NOT NULL,
	LogType int NOT NULL,
	LogDesc nvarchar(4000) NOT NULL,
	UserName nvarchar(20) NOT NULL,
);
GO

CREATE TABLE Misc
(
	UseEmail bit NOT NULL,
	EmailSmtp nvarchar(50) NOT NULL,
	EmailPort nvarchar(5) NOT NULL,
	EmailProtocol int NOT NULL,
	EmailUsername nvarchar(50) NOT NULL,
	EmailPassword nvarchar(100) NOT NULL,
	RetentionPeriod int NOT NULL,
	AttachmentSize int NOT NULL,
	IdleTime int NOT NULL,
	TokenResetTime int NOT NULL,
	DefaultEmail nvarchar(50) NOT NULL,
);
GO
INSERT Misc (UseEmail, EmailSmtp , EmailPort, EmailProtocol, EmailUsername, EmailPassword, RetentionPeriod, AttachmentSize, IdleTime, TokenResetTime, DefaultEmail) VALUES (0, '', '', '' ,'', '', 7, 3, 10, 15, 'psnm-noreply@my.panasonic.com');
GO

CREATE TABLE Building
(
	BuildingId int IDENTITY(1,1) NOT NULL,
	BuildingName nvarchar(50) NOT NULL,
	CONSTRAINT PK_BuildingId PRIMARY KEY (BuildingId),
	CONSTRAINT UQ_BuildingName UNIQUE (BuildingName),
);
GO

CREATE TABLE Rack
(
	RackId int IDENTITY(1,1) NOT NULL,
	RackName nvarchar(50) NOT NULL,
	RackCode nvarchar(20) NOT NULL,
	BuildingId int NOT NULL,
	CONSTRAINT PK_RackId PRIMARY KEY (RackId),
	CONSTRAINT FK_Rack_BuildingId FOREIGN KEY (BuildingId)
		REFERENCES Building(BuildingId) ON DELETE CASCADE,
	CONSTRAINT UQ_RackName UNIQUE (RackName),
	CONSTRAINT UQ_RackCode UNIQUE (RackCode),
);
GO

CREATE TABLE Part
(
	PartId int IDENTITY(1,1) NOT NULL,
	PartCode nvarchar(30) NOT NULL,
	PartName nvarchar(50) NOT NULL,
	PartDesc nvarchar(200) NOT NULL,
	PartFileName nvarchar(100) NOT NULL,
	PartGUIDFileName nvarchar(100) NOT NULL,
	PartMinQty int DEFAULT 0,
	CONSTRAINT PK_PartId PRIMARY KEY (PartId),
	CONSTRAINT UQ_PartCode UNIQUE (PartCode),
);
GO

CREATE TABLE PartRack
(
	RackId int NOT NULL,
	PartId int NOT NULL,
	PartQty int NOT NULL
	CONSTRAINT UQ_PartRack_PartRack_Id UNIQUE ([RackId], [PartId]),
	CONSTRAINT FK_PartRack_RackId FOREIGN KEY ([RackId]) 
		REFERENCES Rack ([RackId]),
	CONSTRAINT FK_PartRack_PartId FOREIGN KEY ([PartId]) 
		REFERENCES Part ([PartId]) ON DELETE CASCADE,
);
GO

CREATE TABLE Record
(
	RecordId int IDENTITY(1,1) NOT NULL,
	RecordDateTime DateTime,
	RecordQty int NOT NULL,
	RecordStatus int NOT NULL,
	RecordBy nvarchar(20) NOT NULL,
	RecordRemark nvarchar(200) NOT NULL,
	PartId int NOT NULL,
	RackId int NOT NULL
	CONSTRAINT PK_Record PRIMARY KEY (RecordId),
	CONSTRAINT FK_Record_PartId FOREIGN KEY (PartId)
		REFERENCES Part(PartId) ON DELETE CASCADE,
	CONSTRAINT FK_Record_RackId FOREIGN KEY (RackId)
		REFERENCES Rack(RackId) ON DELETE CASCADE,
);
GO
