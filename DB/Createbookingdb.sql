create database Golf_Booking 

use [Golf_Booking]
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
    role_id    	UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
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
    rro_id     	UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
    roles_id   	int NOT NULL,
    object_id  	varchar(200) NOT NULL,
    description	nvarchar(200) NULL,
    tstamp     	DATETIME NOT NULL,
    PRIMARY KEY(rro_id)
)
CREATE TABLE [dbo].[golf_course] (
  [course_id]     UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
  [course_name]   NVARCHAR (50) NOT NULL,
  PRIMARY KEY(course_id)
);
CREATE TABLE [dbo].[order] (
  [order_id]   UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
  [user_id]  int NOT NULL,
  [course_id]  int NOT NULL,
  [booking_name] nvarchar(50) not null,
  [phone]   varchar(50) not null,
  [order_status]      VARCHAR (20) NOT NULL,	
  [bookling_status]       varchar(20) not null,
  [settle_status]       varchar(20) not null,
  [member_number]  int not null,
  [appointment_time]   DATETIME      NOT NULL,  
  [deposit] int null,
  [pay_balance] int null,
  [total] int not null,
  [tax] int null,
  [tstamp]             DATETIME      NOT NULL,
  [description]    nvarchar(200)  null,
  [agency] nvarchar(50) null,
  PRIMARY KEY(order_id)
);
GO



