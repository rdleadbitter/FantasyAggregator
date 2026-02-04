-- schema.sql
DROP DATABASE IF EXISTS FantasyAggregator;
CREATE DATABASE FantasyAggregator;
USE FantasyAggregator;

-- Users: people using the aggregator
CREATE TABLE Users (
  UserId INT AUTO_INCREMENT PRIMARY KEY,
  Username VARCHAR(50) NOT NULL UNIQUE,
  Email VARCHAR(255) NOT NULL UNIQUE,
  CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Platforms: different fantasy apps/sites (ESPN, Yahoo, Sleeper, etc.)
CREATE TABLE Platforms (
  PlatformId INT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(100) NOT NULL UNIQUE,
  Url VARCHAR(255),
  CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Teams: aggregator stores teams a user tracks on different platforms
CREATE TABLE Teams (
  TeamId INT AUTO_INCREMENT PRIMARY KEY,
  UserId INT NOT NULL,
  PlatformId INT NOT NULL,
  PlatformTeamId VARCHAR(100) NOT NULL, -- id on remote platform
  TeamName VARCHAR(150) NOT NULL,
  LeagueName VARCHAR(150),
  CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_teams_user FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
  CONSTRAINT fk_teams_platform FOREIGN KEY (PlatformId) REFERENCES Platforms(PlatformId) ON DELETE CASCADE,
  UNIQUE(UserId, PlatformId, PlatformTeamId)
);

-- Players: canonical players in aggregator DB
CREATE TABLE Players (
  PlayerId INT AUTO_INCREMENT PRIMARY KEY,
  FullName VARCHAR(150) NOT NULL,
  Position VARCHAR(20),
  TeamAbbrev VARCHAR(10),
  Active BOOLEAN DEFAULT TRUE
);

-- TeamPlayers: many-to-many teams <-> players with roster slot
CREATE TABLE TeamPlayers (
  TeamPlayerId INT AUTO_INCREMENT PRIMARY KEY,
  TeamId INT NOT NULL,
  PlayerId INT NOT NULL,
  RosterSlot VARCHAR(50),
  AcquiredOn DATE,
  CONSTRAINT fk_tp_team FOREIGN KEY (TeamId) REFERENCES Teams(TeamId) ON DELETE CASCADE,
  CONSTRAINT fk_tp_player FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId) ON DELETE CASCADE,
  UNIQUE(TeamId, PlayerId)
);

-- Optional: index for performance
CREATE INDEX idx_players_name ON Players(FullName);
