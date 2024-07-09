-----------------------------------------------------------------
-- View of user and profile details
-----------------------------------------------------------------
CREATE VIEW vw_UserProfile
AS
	SELECT pr.UserId, pr.UserName, pr.IsActive, pr.StaffNo, pr.StaffName, pr.Email, ur.RoleId, r.RoleName
	FROM UserProfile pr
	INNER JOIN webpages_UsersInRoles ur ON ur.UserId = pr.UserId
	INNER JOIN webpages_Roles r ON r.RoleId = ur.RoleId
;
GO

-----------------------------------------------------------------
-- View of record, part, rack and building of record details 
-----------------------------------------------------------------
CREATE VIEW vw_RecordPartRackBuilding
AS
	SELECT r.RecordId, r.RecordDateTime, r.RecordQty, r.RecordStatus, r.RecordBy, r.RecordRemark, 
	r.PartId, p.PartCode, p.PartName, p.PartDesc,
	r.RackId, c.RackCode, c.RackName,
	b.BuildingId, b.BuildingName
	FROM Record r
	INNER JOIN Part p ON p.PartId = r.PartId
	INNER JOIN Rack c ON c.RackId = r.RackId
	INNER JOIN Building b ON b.BuildingId = c.BuildingId
;
GO

-----------------------------------------------------------------
-- View of part, rack and building of record details 
-----------------------------------------------------------------
CREATE VIEW vw_PartRackBuilding
AS
	SELECT pr.PartId, p.PartCode, p.PartName, p.PartDesc, p.PartMinQty, pr.PartQty,
	pr.RackId, c.RackCode, c.RackName,
	b.BuildingId, b.BuildingName
	FROM PartRack pr
	INNER JOIN Part p ON p.PartId = pr.PartId
	INNER JOIN Rack c ON c.RackId = pr.RackId
	INNER JOIN Building b ON b.BuildingId = c.BuildingId
;
GO

-----------------------------------------------------------------
-- DALUser
-----------------------------------------------------------------
-- Get UserProfile detail
CREATE PROCEDURE usp_GetUserProfile
AS
BEGIN
	SELECT UserId, UserName, RoleId, RoleName, IsActive, StaffNo, StaffName, Email 
	FROM vw_UserProfile
END;
GO

-- Get UserProfile detail by UserId
CREATE PROCEDURE usp_GetUserProfileById
(
	@UserId int
)
AS
BEGIN
	SELECT UserId, UserName, RoleId, RoleName, IsActive, StaffNo, StaffName, Email
	FROM vw_UserProfile
	WHERE UserId = @UserId
END;
GO

-- Check if staff no exist
CREATE PROCEDURE usp_ChkExistStaffNo
(
	@StaffNo nvarchar(12)
)
AS
BEGIN
	IF EXISTS (
			SELECT 1
    		FROM UserProfile
    		WHERE StaffNo = @StaffNo
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Check if email exist
CREATE PROCEDURE usp_ChkExistEmail
(
	@Email nvarchar(100)
)
AS
BEGIN
	IF EXISTS (
			SELECT 1
    		FROM UserProfile
    		WHERE Email = @Email
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Update user 
CREATE PROCEDURE usp_UpdUser
(
	@UserId int,
	@StaffName nvarchar(80),
	@StaffNo nvarchar(12),
	@Email nvarchar(100),
	@IsActive bit
)
AS
BEGIN
	UPDATE UserProfile SET StaffName = @StaffName, StaffNo = @StaffNo, Email = @Email, IsActive = @IsActive
	WHERE UserId = @UserId
END;
GO

-- Get Membership by UserId
CREATE PROCEDURE usp_GetMembershipById
(
	@UserId int
)
AS
BEGIN
	SELECT UserId, CreateDate, ConfirmationToken, IsConfirmed, LastPasswordFailureDate, PasswordFailuresSinceLastSuccess, 
	Password, PasswordChangedDate, PasswordSalt, PasswordVerificationToken, PasswordVerificationTokenExpirationDate
	FROM webpages_Membership
	WHERE UserId = @UserId
END;
GO

-- Get user role list
CREATE PROCEDURE usp_GetRole
AS
BEGIN
	SELECT RoleId, RoleName FROM webpages_Roles
END;
GO

-- Get username by reset token
CREATE PROCEDURE usp_GetUsernameByResetToken
(
	@PasswordVerificationToken nvarchar(128)
)
AS
BEGIN
	SELECT u.UserName
	FROM UserProfile u
	INNER JOIN webpages_Membership m ON m.UserId = u.UserId
	WHERE m.PasswordVerificationToken = @PasswordVerificationToken
END;
GO

-- Get email by user id
CREATE PROCEDURE usp_GetEmailByUserId
(
	@UserId int
)
AS
BEGIN
	SELECT Email FROM UserProfile
	WHERE UserId = @UserId
END;
GO

-----------------------------------------------------------------
-- DALSession
-----------------------------------------------------------------
-- Get session info by UserId
CREATE PROCEDURE usp_GetSessionById
(
	@UserId int
)
AS
BEGIN
	SELECT UserId, SessionId, LastSessionTime 
	FROM Session 
	WHERE UserId = @UserId
END;
GO

-- Register/update session info by UserId
CREATE PROCEDURE usp_SetSession
(
	@UserId int,
	@SessionId nvarchar(30),
	@LastSessionTime datetime
)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM Session WHERE UserId = @UserId)
		UPDATE Session
		SET SessionId = @SessionId, LastSessionTime = @LastSessionTime
		WHERE UserId = @UserId
	ELSE
		INSERT INTO Session (UserId, SessionId, LastSessionTime) VALUES (@UserId, @SessionId, @LastSessionTime)
END;
GO

-- Update session time when logout
CREATE PROCEDURE usp_UpdSessionLogout
(
	@SessionId nvarchar(30),
	@LastSessionTime datetime
)
AS
BEGIN
	UPDATE Session
	SET LastSessionTime = @LastSessionTime
	WHERE SessionId = @SessionId
END;
GO

-- Select no of valid session
CREATE PROCEDURE usp_GetValidSession
(
	@LastSessionTime datetime
)
AS
BEGIN
	SELECT COUNT(*) FROM Session
	WHERE LastSessionTime > @LastSessionTime
END;
GO

-----------------------------------------------------------------
-- DALEvent
-----------------------------------------------------------------
-- Backup database
CREATE PROCEDURE usp_SQLDBBackup
(
	@db NVARCHAR(100),
    @path NVARCHAR(255)
)
AS
BEGIN
	BACKUP DATABASE @db
	TO DISK = @path
END;
GO

-----------------------------------------------------------------
-- DALLog
-----------------------------------------------------------------
-- Register log
CREATE PROCEDURE usp_SetLog
(
	@LogTime datetime,
	@LogType int,
	@LogDesc nvarchar(4000),
	@UserName nvarchar(20)
)
AS
BEGIN
	INSERT INTO LOG (LogTime, LogType, LogDesc, UserName) VALUES (@LogTime, @LogType, @LogDesc, @Username)
END;
GO

-- Get log
CREATE PROCEDURE usp_GetLog
(
	@LogTypeId int,
	@StartDateTime datetime,
	@EndDateTime datetime
)
AS
BEGIN
	IF (@LogTypeId = 0)
		SELECT * FROM Log WHERE LogTime BETWEEN @StartDateTime AND @EndDateTime ORDER BY LogTime ASC
	ELSE
		SELECT * FROM Log WHERE LogTime BETWEEN @StartDateTime AND @EndDateTime AND LogType = @LogTypeId ORDER BY LogTime ASC
END;
GO

-- Delete log
CREATE PROCEDURE usp_DelLog
(
    @CutOfDate datetime
)
AS
BEGIN
    DELETE Log WHERE LogTime < @CutOfDate
END;
GO

-----------------------------------------------------------------
-- DALMisc
-----------------------------------------------------------------
-- Get Misc
CREATE PROCEDURE usp_GetMisc
AS
BEGIN
	SELECT UseEmail, EmailSmtp, EmailPort, EmailProtocol, EmailUsername, EmailPassword, RetentionPeriod, AttachmentSize, IdleTime, TokenResetTime, DefaultEmail
	FROM Misc
END;
GO

-- Update Misc
CREATE PROCEDURE usp_UpdMisc
(
	@UseEmail bit,
	@EmailSmtp nvarchar(50),
	@EmailPort nvarchar(5),
	@EmailProtocol int,
	@EmailUsername nvarchar(50),
	@EmailPassword nvarchar(100),
	@RetentionPeriod int,
	@AttachmentSize int,
	@IdleTime int,
	@TokenResetTime int,
	@DefaultEmail nvarchar(50)
)
AS  
BEGIN 
	UPDATE Misc SET UseEmail = @UseEmail, EmailSmtp = @EmailSmtp, EmailPort = @EmailPort, EmailProtocol = @EmailProtocol, EmailUsername = @EmailUsername, 
	EmailPassword = @EmailPassword,	RetentionPeriod = @RetentionPeriod, AttachmentSize = @AttachmentSize, IdleTime = @IdleTime, TokenResetTime = @TokenResetTime, 
	DefaultEmail = @DefaultEmail
END;
GO

-----------------------------------------------------------------
-- DALBuilding
-----------------------------------------------------------------
-- Get building
CREATE PROCEDURE usp_GetBuilding
AS
BEGIN
	SELECT BuildingId, BuildingName
	FROM Building
	ORDER BY BuildingName
END;
GO

-- Get building by id
CREATE PROCEDURE usp_GetBuildingById
(
	@BuildingId int
)
AS
BEGIN
	SELECT BuildingId, BuildingName
	FROM Building 
	WHERE BuildingId = @BuildingId
END;
GO

-- Register building
CREATE PROCEDURE usp_SetBuilding
(
	@BuildingName nvarchar(50)
)
AS
BEGIN
	INSERT INTO Building VALUES (@BuildingName)
	SELECT BuildingId FROM Building WHERE BuildingName = @BuildingName
END;
GO

-- Update building
CREATE PROCEDURE usp_UpdBuilding
(
	@BuildingId int,
	@BuildingName nvarchar(50)
)
AS
BEGIN
	UPDATE Building SET BuildingName = @BuildingName
	WHERE BuildingId = @BuildingId
END;
GO

-- Delete building
CREATE PROCEDURE usp_DelBuilding
(
	@BuildingId int
)
AS
BEGIN
	DELETE Building WHERE BuildingId = @BuildingId
END;
GO

-- Check if building exist
CREATE PROCEDURE usp_ChkExistBuilding
(
	@BuildingName nvarchar(50)
)
AS
BEGIN
	IF EXISTS (
			SELECT 1
    		FROM Building
    		WHERE BuildingName = @BuildingName
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Get building id by building name
CREATE PROCEDURE usp_GetBuildingIdByName
(
	@BuildingName nvarchar(50)
)
AS
BEGIN
	SELECT BuildingId FROM Building
	WHERE BuildingName = @BuildingName
END;
GO

-- Register building and get inserted building id 
CREATE PROCEDURE usp_SetBuildingWithOutputId
(
	@BuildingName nvarchar(50)
)
AS
BEGIN
	INSERT INTO Building 
	OUTPUT INSERTED.BuildingId
	VALUES (@BuildingName)
	SELECT BuildingId FROM Building WHERE BuildingName = @BuildingName
END;
GO

-----------------------------------------------------------------
-- DALRack
-----------------------------------------------------------------
-- Get rack
CREATE PROCEDURE usp_GetRack
AS
BEGIN
	SELECT c.RackId, c.RackName, c.RackCode, c.BuildingId, b.BuildingName 
	FROM Rack c
	INNER JOIN Building b ON b.BuildingId = c.BuildingId
END;
GO

-- Register rack
CREATE PROCEDURE usp_SetRack
(
	@RackName nvarchar(50),
	@RackCode nvarchar(20),
	@BuildingId int
)
AS
BEGIN
	INSERT INTO Rack VALUES (@RackName, @RackCode, @BuildingId)
END;
GO

-- Update rack
CREATE PROCEDURE usp_UpdRack
(
	@RackId int,
	@RackName nvarchar(50),
	@RackCode nvarchar(20)
)
AS
BEGIN
	UPDATE Rack SET RackName = @RackName , RackCode = @RackCode
	WHERE RackId = @RackId
END;
GO

-- Delete rack
CREATE PROCEDURE usp_DelRack
(
	@RackId int
)
AS
BEGIN
	DELETE Rack WHERE RackId = @RackId
END;
GO

-- Get rack building by id
CREATE PROCEDURE usp_GetRackBuildingById
(
	@RackId int
)
AS
BEGIN
	SELECT c.RackId, c.RackName, c.RackCode, c.BuildingId, b.BuildingName 
	FROM Rack c
	INNER JOIN Building b ON b.BuildingId = c.BuildingId
	WHERE c.RackId = @RackId
END;
GO

-- Check if rack exist under same building
CREATE PROCEDURE usp_ChkExistRack
(
	@RackName nvarchar(50),
	@RackCode nvarchar(20),
	@BuildingId int
)
AS
BEGIN
	IF EXISTS (
			SELECT RackName, RackCode
    		FROM Rack
    		WHERE RackName = @RackName AND RackCode = @RackCode AND BuildingId = @BuildingId
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Check if rack code exist
CREATE PROCEDURE usp_ChkExistRackCode
(
	@RackCode nvarchar(20)
)
AS
BEGIN
	IF EXISTS (
			SELECT RackCode
    		FROM Rack
    		WHERE RackCode = @RackCode
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Get rack id by rack code
CREATE PROCEDURE usp_GetRackId
(
	@RackCode nvarchar(20)
)
AS
BEGIN
	SELECT RackId FROM Rack WHERE RackCode = @RackCode
END;
GO

-- Get rack by building id
CREATE PROCEDURE usp_GetRackByBuildingId
(
	@BuildingId int
)
AS
BEGIN
	SELECT c.RackId, c.RackName, c.RackCode, c.BuildingId, b.BuildingName 
	FROM Rack c
	INNER JOIN Building b ON b.BuildingId = c.BuildingId
	WHERE c.BuildingId = @BuildingId
END;
GO

-- Check if rack name and code exist
CREATE PROCEDURE usp_ChkExistRackNameCode
(
	@RackName nvarchar(50),
	@RackCode nvarchar(20)
)
AS
BEGIN
	IF EXISTS (
			SELECT RackCode
    		FROM Rack
    		WHERE RackName = @RackName AND RackCode = @RackCode 
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Register rack and get inserted rack id 
CREATE PROCEDURE usp_SetRackWithOutputId
(
	@RackName nvarchar(50),
	@RackCode nvarchar(20),
	@BuildingId int
)
AS
BEGIN
	INSERT INTO Rack 
	OUTPUT INSERTED.RackId
	VALUES (@RackName, @RackCode, @BuildingId)
END;
GO

-- Check if rack name exist
CREATE PROCEDURE usp_ChkExistRackName
(
	@RackName nvarchar(50)
)
AS
BEGIN
	IF EXISTS (
			SELECT RackName
    		FROM Rack
    		WHERE RackName = @RackName
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-----------------------------------------------------------------
-- DALPart
-----------------------------------------------------------------
-- Get part
CREATE PROCEDURE usp_GetPart
AS
BEGIN
	SELECT *
	FROM Part 
	ORDER BY PartCode
END;
GO

-- Get part by id
CREATE PROCEDURE usp_GetPartById
(
	@PartId int
)
AS
BEGIN
	SELECT *
	FROM Part 
	WHERE PartId = @PartId
END;
GO

-- Register part
CREATE PROCEDURE usp_SetPart
(
	@PartCode nvarchar(30),
	@PartName nvarchar(50),
	@PartDesc nvarchar(200),
	@PartFileName nvarchar(100),
	@PartGUIDFileName nvarchar(100),
	@PartMinQty INT
)
AS
BEGIN
	INSERT INTO Part (PartCode, PartName, PartDesc, PartFileName, PartGUIDFileName, PartMinQty)
	VALUES (@PartCode, @PartName, @PartDesc, @PartFileName, @PartGUIDFileName, @PartMinQty)
END;
GO

-- Update part
CREATE PROCEDURE usp_UpdPart
(
	@PartId int,
	@PartCode nvarchar(30),
	@PartName nvarchar(50),
	@PartDesc nvarchar(200),
	@PartFileName nvarchar(100),
	@PartGUIDFileName nvarchar(100),
	@PartMinQty INT
)
AS
BEGIN
	UPDATE Part SET PartCode = @PartCode, PartName = @PartName, PartDesc = @PartDesc, PartFileName = @PartFileName, PartGUIDFileName = @PartGUIDFileName, PartMinQty = @PartMinQty
	WHERE PartId = @PartId
END;
GO

-- Delete part
CREATE PROCEDURE usp_DelPart
(
	@PartId int
)
AS
BEGIN
	DELETE Part WHERE PartId = @PartId
END;
GO

-- Check if part code exist
CREATE PROCEDURE usp_ChkExistPartCode
(
	@PartCode nvarchar(30)
)
AS
BEGIN
	IF EXISTS (
			SELECT 1
    		FROM Part
    		WHERE PartCode = @PartCode
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Get part id by part code
CREATE PROCEDURE usp_GetPartId
(
	@PartCode nvarchar(30)
)
AS
BEGIN
	SELECT PartId FROM Part WHERE PartCode = @PartCode
END;
GO

-- Register part and get inserted part id 
CREATE PROCEDURE usp_SetPartWithOutputId
(
	@PartCode nvarchar(30),
	@PartName nvarchar(50),
	@PartDesc nvarchar(200),
	@PartFileName nvarchar(100),
	@PartGUIDFileName nvarchar(100),
	@PartMinQty INT
)
AS
BEGIN
	INSERT INTO Part (PartCode, PartName, PartDesc, PartFileName, PartGUIDFileName, PartMinQty)
	OUTPUT INSERTED.PartId
	VALUES (@PartCode, @PartName, @PartDesc, @PartFileName, @PartGUIDFileName, @PartMinQty)
END;
GO

-- Get part file name by part id
CREATE PROCEDURE usp_GetPartFileNameByPartId
(
	@PartId int
)
AS
BEGIN
	SELECT PartFileName FROM Part WHERE PartId = @PartId
END;
GO

-- Get part GUID file name by part id
CREATE PROCEDURE usp_GetPartGUIDFileNameByPartId
(
	@PartId int
)
AS
BEGIN
	SELECT PartGUIDFileName FROM Part WHERE PartId = @PartId
END;
GO

-- Get part with balance quantity
CREATE PROCEDURE usp_GetPartNBalQtyList
AS
BEGIN
	SELECT p.PartId, p.PartCode, p.PartName, p.PartDesc, p.PartFileName, p.PartGUIDFileName, p.PartMinQty,
	SUM(pr.PartQty) AS BalanceQty
	FROM PART p
	LEFT JOIN PartRack pr ON p.PartId = pr.PartId
	GROUP BY p.PartId, p.PartCode, p.PartName, p.PartDesc, p.PartFileName, p.PartGUIDFileName, p.PartMinQty
END;
GO

-- Get part with balance quantity by id
CREATE PROCEDURE usp_GetPartNBalQtyById
(
	@PartId int
)
AS
BEGIN
	SELECT p.PartId, p.PartCode, p.PartName, p.PartDesc, p.PartFileName, p.PartGUIDFileName, p.PartMinQty,
	SUM(pr.PartQty) AS BalanceQty
	FROM PART p
	LEFT JOIN PartRack pr ON p.PartId = pr.PartId
	WHERE p.PartId = @PartId
	GROUP BY p.PartId, p.PartCode, p.PartName, p.PartDesc, p.PartFileName, p.PartGUIDFileName, p.PartMinQty
END;
GO

-----------------------------------------------------------------
-- DALPartRack
-----------------------------------------------------------------
-- Get part rack
CREATE PROCEDURE usp_GetPartRackList
AS
BEGIN
	SELECT pr.RackId, r.RackCode, r.RackName, pr.PartId, p.PartCode, p.PartName, pr.PartQty
	FROM PartRack pr
	INNER JOIN Rack r ON r.RackId = pr.RackId
	INNER JOIN Part p ON p.PartId = pr.PartId
END;
GO

-- Check if part rack exist
CREATE PROCEDURE usp_ChkExistPartRack
(
	@RackId int,
	@PartId int
)
AS
BEGIN
	IF EXISTS (
			SELECT 1
    		FROM PartRack
    		WHERE RackId = @RackId AND PartId = @PartId 
		)
   		SELECT 1
	ELSE
   		SELECT 0
END;
GO

-- Register part rack
CREATE PROCEDURE usp_SetPartRack
(
	@RackId int,
	@PartId int,
	@PartQty int
)
AS
BEGIN
	INSERT INTO PartRack (RackId, PartId, PartQty)
	VALUES (@RackId, @PartId, @PartQty)
END;
GO

-- Delete part rack
CREATE PROCEDURE usp_DelPartRack
(
	@RackId int,
	@PartId int
)
AS
BEGIN
	DELETE PartRack WHERE RackId = @RackId AND PartId = @PartId
END;
GO

-- Update part rack
CREATE PROCEDURE usp_UpdPartRack
(
	@RackId int,
	@PartId int,
	@PartQty int
)
AS
BEGIN
	UPDATE PartRack SET RackId = @RackId , PartId = @PartId, PartQty = @PartQty
	WHERE RackId = @RackId AND PartId = @PartId
END;
GO

-- Get part rack by rack id and part id
CREATE PROCEDURE usp_GetPartByRackIdPartId
(
	@RackId int,
	@PartId int
)
AS
BEGIN
	SELECT p.PartId, p.PartCode, pr.PartQty, p.PartGUIDFileName
	FROM PartRack pr
	INNER JOIN Part p ON pr.PartId = p.PartId
	WHERE pr.RackId = @RackId AND pr.PartId = @PartId
END;
GO

-- Get part quantity by rack id and part id
CREATE PROCEDURE usp_GetPartQtyByRackIdPartId
(
	@RackId int,
	@PartId int
)
AS
BEGIN
	SELECT PartQty FROM PartRack 
	WHERE RackId = @RackId AND PartId = @PartId
END;
GO

-- Get part list by rack id
CREATE PROCEDURE usp_GetPartRackByRackId
(
	@RackId int
)
AS
BEGIN
	SELECT p.PartId, p.PartCode, p.PartName, p.PartDesc
	FROM PartRack pr
	INNER JOIN Part p ON pr.PartId = p.PartId
	WHERE RackId = @RackId
END;
GO

-- Report - Get inventory list
CREATE PROCEDURE usp_GetInventoryList
AS  
BEGIN
	SELECT PartId, PartCode, PartName, PartDesc, PartMinQty, PartQty,
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_PartRackBuilding
END;
GO

-- Report - Get inventory by building id
CREATE PROCEDURE usp_GetInventoryByBuildingId
(
	@BuildingId int
)
AS  
BEGIN
	SELECT PartId, PartCode, PartName, PartDesc, PartMinQty, PartQty,
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_PartRackBuilding
	WHERE BuildingId = @BuildingId
END;
GO

-- Report - Get inventory by building id, rack id
CREATE PROCEDURE usp_GetInventoryByBuildingIdRackId
(
	@BuildingId int,
	@RackId int
)
AS  
BEGIN
	SELECT PartId, PartCode, PartName, PartDesc, PartMinQty, PartQty,
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_PartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId
END;
GO

-- Report - Get inventory by building id, rack id, part id
CREATE PROCEDURE usp_GetInventoryByBuildingIdRackIdPartId
(
	@BuildingId int,
	@RackId int,
	@PartId int
)
AS  
BEGIN
	SELECT PartId, PartCode, PartName, PartDesc, PartMinQty, PartQty,
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_PartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND PartId = @PartId
END;
GO

-- Report - Get inventory by part id
CREATE PROCEDURE usp_GetInventoryByPartId
(
	@PartId int
)
AS  
BEGIN
	SELECT PartId, PartCode, PartName, PartDesc, PartMinQty, PartQty,
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_PartRackBuilding
	WHERE PartId = @PartId
END;
GO

-- Update part rack rack id, part id part qty
CREATE PROCEDURE usp_UpdPartRackByRackIdPartIdPartQty
(
	@NewRackId int,
	@RackId int,
	@PartId int,
	@PartQty int
)
AS
BEGIN
	UPDATE PartRack SET RackId = @NewRackId , PartId = @PartId, PartQty = @PartQty
	WHERE RackId = @RackId AND PartId = @PartId
END;
GO

-- Update part rack by part qty
CREATE PROCEDURE usp_UpdPartRackByPartQty
(
	@RackId int,
	@PartId int,
	@PartQty int
)
AS
BEGIN
	UPDATE PartRack SET PartQty = @PartQty
	WHERE RackId = @RackId AND PartId = @PartId
END;
GO

-----------------------------------------------------------------
-- DALRecord
-----------------------------------------------------------------
-- Get record
CREATE PROCEDURE usp_GetRecord
AS
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, PartId, PartCode, RackId, RackCode
	FROM vw_RecordPartRackBuilding
	ORDER BY RecordDateTime DESC
END;
GO

-- Get record by id
CREATE PROCEDURE usp_GetRecordById
(
	@RecordId int
)
AS
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, PartId, PartCode, RackId, RackCode
	FROM vw_RecordPartRackBuilding
	WHERE RecordId = @RecordId
END;
GO

-- Register Record 
CREATE PROCEDURE usp_SetRecord
(
	@RecordDateTime datetime,
	@RecordQty int,
	@RecordStatus int,
	@RecordBy nvarchar(20),
	@RecordRemark nvarchar(200),
	@PartId int,
	@RackId int
)
AS
BEGIN
	INSERT INTO Record (RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, PartId, RackId)
	VALUES (@RecordDateTime, @RecordQty, @RecordStatus, @RecordBy, @RecordRemark, @PartId, @RackId)
END;
GO

-- Delete record
CREATE PROCEDURE usp_DelRecord
(
	@CutOfDate datetime
)
AS
BEGIN
	DELETE FROM Record WHERE RecordDateTime < @CutOfDate
END;
GO

-- Get record by rack id
CREATE PROCEDURE usp_GetRecordByRackId
(
	@RackId int
)
AS
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName
	FROM vw_RecordPartRackBuilding
	WHERE RackId = @RackId
END;
GO

-- Get record by part id
CREATE PROCEDURE usp_GetRecordByPartId
(
	@PartId int
)
AS
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName
	FROM vw_RecordPartRackBuilding
	WHERE PartId = @PartId
END;
GO

-- Report - Get record by date from, date to, status
CREATE PROCEDURE usp_GetRecordFromDtToDtStatus
(
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus = @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id date from, date to, status
CREATE PROCEDURE usp_GetRecordByBuildingIdFromDtToDtStatus
(
	@BuildingId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus = @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id, rack id, date from, date to, status
CREATE PROCEDURE usp_GetRecordByBuildingIdRackIdFromDtToDtStatus
(
	@BuildingId int,
	@RackId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus = @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id, rack id, part id, date from, date to, status
CREATE PROCEDURE usp_GetRecordByBuildingIdRackIdPartIdFromDtToDtStatus
(
	@BuildingId int,
	@RackId int,
	@PartId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND PartId = @PartId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus = @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by part id, date from, date to, status
CREATE PROCEDURE usp_GetRecordByPartIdFromDtToDtStatus
(
	@PartId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE PartId = @PartId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus = @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by date from, date to
CREATE PROCEDURE usp_GetRecordFromDtToDt
(
	@FromDT datetime,
	@ToDT datetime
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id, date from, date to
CREATE PROCEDURE usp_GetRecordByBuildingIdFromDtToDt
(
	@BuildingId int,
	@FromDT datetime,
	@ToDT datetime
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT
	ORDER BY RecordDateTime ASC
END;
GO


-- Report - Get record by building id, rack id, date from, date to
CREATE PROCEDURE usp_GetRecordByBuildingIdRackIdFromDtToDt
(
	@BuildingId int,
	@RackId int,
	@FromDT datetime,
	@ToDT datetime
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id, rack id, part id, date from, date to
CREATE PROCEDURE usp_GetRecordByBuildingIdRackIdPartIdFromDtToDt
(
	@BuildingId int,
	@RackId int,
	@PartId int,
	@FromDT datetime,
	@ToDT datetime
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND PartId = @PartId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by part id, date from, date to
CREATE PROCEDURE usp_GetRecordByPartIdFromDtToDt
(
	@PartId int,
	@FromDT datetime,
	@ToDT datetime
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE PartId = @PartId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by date from, date to, not equal status
CREATE PROCEDURE usp_GetRecordFromDtToDtNotEqualStatus
(
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus != @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id date from, date to, not equal status
CREATE PROCEDURE usp_GetRecordByBuildingIdFromDtToDtNotEqualStatus
(
	@BuildingId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus != @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id, rack id, date from, date to, not equal status
CREATE PROCEDURE usp_GetRecordByBuildingIdRackIdFromDtToDtNotEqualStatus
(
	@BuildingId int,
	@RackId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus != @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by building id, rack id, part id, date from, date to, not equal status
CREATE PROCEDURE usp_GetRecordByBuildingIdRackIdPartIdFromDtToDtNotEqualStatus
(
	@BuildingId int,
	@RackId int,
	@PartId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE BuildingId = @BuildingId AND RackId = @RackId AND PartId = @PartId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus != @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-- Report - Get record by part id, date from, date to, not equal status
CREATE PROCEDURE usp_GetRecordByPartIdFromDtToDtNotEqualStatus
(
	@PartId int,
	@FromDT datetime,
	@ToDT datetime,
	@RecordStatus int
)
AS  
BEGIN
	SELECT RecordId, RecordDateTime, RecordQty, RecordStatus, RecordBy, RecordRemark, 
	PartId, PartCode, PartName, PartDesc, 
	RackId, RackCode, RackName,
	BuildingId, BuildingName
	FROM vw_RecordPartRackBuilding
	WHERE PartId = @PartId AND RecordDateTime >= @FromDT AND RecordDateTime <= @ToDT AND RecordStatus != @RecordStatus
	ORDER BY RecordDateTime ASC
END;
GO

-----------------------------------------------------------------
-- Server/DALUser
-----------------------------------------------------------------
-- Get active user for admin account
CREATE PROCEDURE usp_GetActUserAdminAcc
(
	@RoleId int
)
AS
BEGIN
	SELECT '' AS No, u.UserId, u.UserName, u.StaffName, u.Email, u.IsActive, m.Password, r.RoleId
	FROM UserProfile u
	INNER JOIN webpages_Membership m ON m.UserId = u.UserId
	INNER JOIN webpages_UsersInRoles r ON r.UserId = u.UserId
	WHERE RoleId = @RoleId AND u.IsActive = 1
END;
GO

-----------------------------------------------------------------
-- Server/DALPartRack
-----------------------------------------------------------------

-- Get part rack below min qty
CREATE PROCEDURE usp_GetPartRackBelowMinQtyList
AS
BEGIN
	SELECT p.PartCode, p.PartName, p.PartDesc, p.PartMinQty, pr.PartQty
	FROM PartRack pr
	INNER JOIN Part p ON pr.PartId = p.PartId
	WHERE pr.PartQty <= p.PartMinQty
END;
GO
