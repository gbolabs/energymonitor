DROP TABLE [dbo].[Measures]
GO
DROP TABLE [dbo].[Counters]
GO
DROP TABLE [dbo].[Objects]
GO
DROP TABLE [dbo].[LastMeasureByCounter]
GO


CREATE TABLE [dbo].[Counters](
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()),
	[Name] [nvarchar](20) NULL,
	[Description] [nvarchar](50) NULL,
	[Unit] [nchar](3) NOT NULL,
	[IsCumulative] [bit] NOT NULL,
	[IsAbsolute] [bit] NOT NULL,
	[Exposant] [tinyint] NOT NULL,
 CONSTRAINT [PK_Counters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Measures]    Script Date: 05/07/2022 09:12:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Measures](
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()),
	[SamplingDate] [datetime] NOT NULL,
	[CounterId] [uniqueidentifier] NOT NULL,
	[ObjectId] [uniqueidentifier] NOT NULL,
	[Value] [int] NOT NULL,
 CONSTRAINT [PK_Measures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Objects]    Script Date: 05/07/2022 09:12:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Objects](
	[Id] [uniqueidentifier] NOT NULL DEFAULT(NEWID()),
	[Name] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](50) NULL,
 CONSTRAINT [PK_Objects] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Measures]  WITH CHECK ADD  CONSTRAINT [FK_Measures_Counters] FOREIGN KEY([CounterId])
REFERENCES [dbo].[Counters] ([Id])
GO
ALTER TABLE [dbo].[Measures] CHECK CONSTRAINT [FK_Measures_Counters]
GO
ALTER TABLE [dbo].[Measures]  WITH CHECK ADD  CONSTRAINT [FK_Measures_Objects] FOREIGN KEY([ObjectId])
REFERENCES [dbo].[Objects] ([Id])
GO
ALTER TABLE [dbo].[Measures] CHECK CONSTRAINT [FK_Measures_Objects]
GO
ALTER DATABASE [adbpr114emp001] SET  READ_WRITE 
GO


CREATE TABLE [dbo].[LastMeasureByCounter](
	[CounterId] [uniqueidentifier] NOT NULL,
	[MeasureId] [uniqueidentifier] NOT NULL,
	[ObjectId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_LastMeasureByCounter] PRIMARY KEY CLUSTERED 
(
	[CounterId] ASC,
	[ObjectId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO