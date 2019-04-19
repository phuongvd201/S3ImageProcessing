/*
 Create tables if not exist
	
 Creator: Phuong Vo
 Date: 04/04/2019 20:59:01
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for ImageFile
-- ----------------------------
DROP TABLE IF EXISTS `ImageFile`;
CREATE TABLE `ImageFile`  (
  `FileID` int(11) NOT NULL AUTO_INCREMENT,
  `FileName` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `FileSize` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`FileID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;

-- ----------------------------
-- Table structure for Histogram
-- ----------------------------
DROP TABLE IF EXISTS `Histogram`;
CREATE TABLE `Histogram`  (
  `FileID` int(11) UNSIGNED NOT NULL,
  `BandNumber` int(4) NOT NULL,
  `Value` int(11) NOT NULL,
  PRIMARY KEY (`FileID`, `BandNumber`) USING BTREE,
  FOREIGN KEY(`FileID`) references ImageFile(`FileID`)
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;


