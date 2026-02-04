-- seed.sql
USE FantasyAggregator;

-- Users
INSERT INTO Users (Username, Email) VALUES
('alice', 'alice@example.com'),
('bob', 'bob@example.com'),
('charlie', 'charlie@example.com');

-- Platforms
INSERT INTO Platforms (Name, Url) VALUES
('ESPN', 'https://espn.com'),
('Yahoo', 'https://sports.yahoo.com'),
('Sleeper', 'https://sleeper.app'),
('CBS', 'https://cbssports.com');

-- Teams (example: alice tracks two teams on ESPN and Yahoo)
INSERT INTO Teams (UserId, PlatformId, PlatformTeamId, TeamName, LeagueName) VALUES
(1, 1, 'espn_1001', 'Alice Avengers', 'Alpha League'),
(1, 2, 'yahoo_2001', 'Alice Allstars', 'Cedar League'),
(2, 3, 'sleeper_3001', 'Bob Bombers', 'Metro League'),
(3, 4, 'cbs_4001', 'Charlie Champs', 'Sunday League');

-- Players (60 players)
INSERT INTO Players (FullName, Position, TeamAbbrev, Active) VALUES
('Patrick Mahomes','QB','KC',TRUE),
('Josh Allen','QB','BUF',TRUE),
('Jalen Hurts','QB','PHI',TRUE),
('Joe Burrow','QB','CIN',TRUE),
('Justin Jefferson','WR','MIN',TRUE),
('Ja''Marr Chase','WR','CIN',TRUE),
('Tyreek Hill','WR','MIA',TRUE),
('Cooper Kupp','WR','LAR',TRUE),
('Christian McCaffrey','RB','SF',TRUE),
('Derrick Henry','RB','TEN',TRUE),
('Austin Ekeler','RB','LAC',TRUE),
('Saquon Barkley','RB','NYG',TRUE),
('Travis Kelce','TE','KC',TRUE),
('Mark Andrews','TE','BAL',TRUE),
('D.K. Metcalf','WR','SEA',TRUE),
('Amon-Ra St. Brown','WR','DET',TRUE),
('Nick Chubb','RB','CLE',TRUE),
('Jonathan Taylor','RB','IND',TRUE),
('Lamar Jackson','QB','BAL',TRUE),
('Kirk Cousins','QB','MIN',TRUE),
('Derrick Henry II','RB','TEN',TRUE),
('Mike Evans','WR','TB',TRUE),
('Stefon Diggs','WR','BUF',TRUE),
('CeeDee Lamb','WR','DAL',TRUE),
('Aaron Rodgers','QB','NYJ',TRUE),
('Geno Smith','QB','SEA',TRUE),
('Cam Akers','RB','LAR',TRUE),
('Najee Harris','RB','PIT',TRUE),
('Tony Pollard','RB','DAL',TRUE),
('Deebo Samuel','WR','SF',TRUE),
('Garrett Wilson','WR','NYJ',TRUE),
('Joe Mixon','RB','CIN',TRUE),
('Ezekiel Elliott','RB','DAL',FALSE),
('George Kittle','TE','SF',TRUE),
('Dalvin Cook','RB','NYJ',TRUE),
('Diontae Johnson','WR','PIT',TRUE),
('Chris Godwin','WR','TB',TRUE),
('Terry McLaurin','WR','WAS',TRUE),
('Breece Hall','RB','NYJ',TRUE),
('Zach Ertz','TE','ARI',TRUE),
('Rhamondre Stevenson','RB','NE',TRUE),
('Keenan Allen','WR','LAC',TRUE),
('Amari Cooper','WR','CLE',TRUE),
('Tyler Lockett','WR','SEA',TRUE),
('J.K. Dobbins','RB','BAL',FALSE),
('Brandon Aiyuk','WR','SF',TRUE),
('Jaylen Waddle','WR','MIA',TRUE),
('Calvin Ridley','WR','JAX',TRUE),
('Patrick Peterson','DB','MIN',FALSE),
('Travis Homer','RB','SEA',TRUE),
('Austin Hooper','TE','NYG',TRUE),
('Hunter Renfrow','WR','LV',TRUE),
('Miles Sanders','RB','PHI',TRUE),
('D.J. Moore','WR','CHI',TRUE),
('Michael Pittman Jr.','WR','IND',TRUE),
('Dalton Schultz','TE','HOU',TRUE),
('Cordarrelle Patterson','RB','ATL',TRUE),
('Rashaad Penny','RB','PHI',TRUE);

-- TeamPlayers: link some players to Alice Avengers (TeamId = 1)
INSERT INTO TeamPlayers (TeamId, PlayerId, RosterSlot, AcquiredOn) VALUES
(1, 1, 'QB1', '2025-01-10'),
(1, 9, 'RB1', '2025-01-10'),
(1, 10, 'RB2', '2025-01-10'),
(1, 5, 'WR1', '2025-01-11'),
(1, 6, 'WR2', '2025-01-11'),
(1, 13, 'TE', '2025-01-12');

-- TeamPlayers for Bob Bombers (TeamId = 3)
INSERT INTO TeamPlayers (TeamId, PlayerId, RosterSlot, AcquiredOn) VALUES
(3, 2, 'QB1', '2025-01-05'),
(3, 11, 'RB1', '2025-01-06'),
(3, 7, 'WR1', '2025-01-06');

-- Quick count check: you can run SELECT COUNT(*) FROM Players; (should be 60)
