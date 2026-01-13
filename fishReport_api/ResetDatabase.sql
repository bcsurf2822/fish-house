-- Reset FishHouse Database
-- Run this script to clear all tables and migration history

-- Drop foreign key constraints first (by dropping junction table)
IF OBJECT_ID(N'[dbo].[FishMarketInventory]', N'U') IS NOT NULL
    DROP TABLE [dbo].[FishMarketInventory];

-- Drop main tables
IF OBJECT_ID(N'[dbo].[Species]', N'U') IS NOT NULL
    DROP TABLE [dbo].[Species];

IF OBJECT_ID(N'[dbo].[FishMarkets]', N'U') IS NOT NULL
    DROP TABLE [dbo].[FishMarkets];

-- Drop migration history
IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
    DROP TABLE [dbo].[__EFMigrationsHistory];

-- Verify all tables are dropped
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
