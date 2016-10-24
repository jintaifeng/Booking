-- 1. 데이타베이스 
create database GolfBooking 

--2. 테이블 
use [GolfBooking]
CREATE TABLE [dbo].[user] (
    [user_id]             UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
	[login_name]          VARCHAR (50)         NOT NULL,
    [email]               VARCHAR (50)  NOT NULL,
    [password]            VARCHAR (50)  NOT NULL,
    [name]           NVARCHAR (50) NOT NULL,
	[role_id]    int not null,
    [phone]       VARCHAR (50) NOT NULL,	
	[status]      VARCHAR (20) NOT NULL,	
	[update_time] DATETIME      NOT NULL,
    [tstamp]              DATETIME      NOT NULL,
	PRIMARY KEY([user_id])
);
CREATE TABLE rbac_roles ( 
    role_id    	int identity(1,1) NOT NULL,
    role_name  	nvarchar(50) NOT NULL,
    description	nvarchar(200) NULL,
    tstamp     	DATETIME NOT NULL,
    PRIMARY KEY(role_id)
)
CREATE TABLE rbac_objects ( 
    object_id        	varchar(200) NOT NULL,
    object_name      	nvarchar(50) NOT NULL,
    object_url       	varchar(100) NULL,
    object_type      	varchar(100) NOT NULL,
    description      	nvarchar(200) NULL,
    status           	varchar(100) NOT NULL,
    tstamp           	DATETIME NOT NULL,
    icon             	varchar(100) NULL,
    sort             	int NULL,
    PRIMARY KEY(object_id)
)
CREATE TABLE rbac_role_object ( 
    rro_id     	INT  identity(1,1) NOT NULL,
    roles_id   	int NOT NULL,
    object_id  	varchar(200) NOT NULL,
    description	nvarchar(200) NULL,
    tstamp     	DATETIME NOT NULL,
    PRIMARY KEY(rro_id)
)
CREATE TABLE [dbo].[golf_course] (
  [course_id]     int identity(1,1) NOT NULL,
  [course_name]   NVARCHAR (50) NOT NULL,
  [commission]   INT NOT NULL,
  [status]     VARCHAR(10) NOT NULL,
  [description]   NVARCHAR (200) NULL,
  PRIMARY KEY(course_id)
);
CREATE TABLE [dbo].[booking_order] (
  [order_id]  int identity(100,1) NOT NULL,
  [user_id]  UNIQUEIDENTIFIER NOT NULL,
  [course_id]  int NOT NULL,
  [booking_username] nvarchar(50) not null,
  [phone]   varchar(50) not null,	
  [bookling_status]       varchar(20) not null,
  [settle_status]       varchar(20) not null,
  [member_number]  int not null,
  [appointment_date]   DATETIME      NOT NULL,  
  [booking_date]   DATETIME      NOT NULL,  
  [deposit] int null,
  [pay_balance] int null,
  [sub_total] int not null,
  [total] int not null,
  [commission] int null,
  [total_commission] int null,
  [description]    nvarchar(200)  null,
  [tstamp]             DATETIME      NOT NULL,
  [update_time]        DATETIME      NOT NULL,
  PRIMARY KEY(order_id)
);
CREATE TABLE [dbo].[user_code] (
	[user_id] UNIQUEIDENTIFIER not null,
	[code] varchar(50) not null,
	[tstamp]   DATETIME      NOT NULL
);

-- 3. 데이타 입력
INSERT INTO [GolfBooking].[dbo].[rbac_roles]
           ([role_name]
           ,[description]
           ,[tstamp])
     VALUES
           ('Admin','관리자',GETDATE()),
           ('Custom','일반사용자',GETDATE())
GO


--4. admin insert

INSERT INTO [dbo].[user]
			(
            [login_name]
           ,[email]
           ,[password]
           ,[name]
           ,[role_id]
           ,[phone]
           ,[status]
           ,[update_time]
           ,[tstamp])
     VALUES
           ('admin'
           ,'admin@outlook.com'
           ,'21232F297A57A5A743894A0E4A801FC3'
           ,'admin'
           ,1
           ,''
           ,'open'
           ,GETDATE()
           ,GETDATE())



