/****** Object:  Table [dbo].[records]    Script Date: 8/27/2020 8:40:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[records](
	[id] [uniqueidentifier] NOT NULL,
	[messageObject] [nvarchar](max) NULL,
	[createdAt] [datetime] NULL,
	[status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[records] ADD  DEFAULT (newid()) FOR [id]
GO

ALTER TABLE [dbo].[records] ADD  DEFAULT (getdate()) FOR [createdAt]
GO

ALTER TABLE [dbo].[records] ADD  DEFAULT ((1)) FOR [status]
GO


