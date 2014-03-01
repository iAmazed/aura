ALTER TABLE `accounts`
  ADD `autobanScore` int(10) NOT NULL DEFAULT '0',
  ADD `autobanCount` int(10) NOT NULL DEFAULT '0',
  ADD`lastAutobanReduction` datetime DEFAULT NULL;

CREATE TABLE IF NOT EXISTS `autoban` (
  `accountId` varchar(50) NOT NULL,
  `date` datetime NOT NULL,
  `severity` int(10) NOT NULL,
  `report` varchar(500) NOT NULL,
  KEY `accountId` (`accountId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


ALTER TABLE `autoban`
  ADD CONSTRAINT `autoban_ibfk_1` FOREIGN KEY (`accountId`) REFERENCES `accounts` (`accountId`) ON DELETE CASCADE ON UPDATE CASCADE;