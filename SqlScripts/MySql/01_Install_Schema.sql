
SET FOREIGN_KEY_CHECKS=0;

START TRANSACTION;
-- ----------------------------
-- Table structure for spb_userprofiles
-- ----------------------------
DROP TABLE IF EXISTS `spb_userprofiles`;
CREATE TABLE `spb_userprofiles` (
  `UserId` bigint(20) NOT NULL,
  `Gender` smallint(6) NOT NULL,
  `BirthdayType` smallint(6) NOT NULL,
  `Birthday` datetime NOT NULL,
  `LunarBirthday` datetime NOT NULL,
  `NowAreaCode` varchar(8) DEFAULT NULL,
  `QQ` varchar(64) DEFAULT NULL,
  `CardType` smallint(6) DEFAULT NULL,
  `CardID` varchar(64) DEFAULT NULL,
  `Introduction` longtext,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  `Integrity` int(11) DEFAULT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for spb_video
-- ----------------------------
DROP TABLE IF EXISTS `spb_video`;
CREATE TABLE `spb_video` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ContentItemId` bigint(20) DEFAULT NULL,
  `VideoUrl` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_accountbindings
-- ----------------------------
DROP TABLE IF EXISTS `tn_accountbindings`;
CREATE TABLE `tn_accountbindings` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `AccountTypeKey` varchar(64) DEFAULT NULL,
  `Identification` varchar(255) DEFAULT NULL,
  `AccessToken` varchar(255) DEFAULT NULL,
  `ExpiredDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_accounttypes
-- ----------------------------
DROP TABLE IF EXISTS `tn_accounttypes`;
CREATE TABLE `tn_accounttypes` (
  `AccountTypeKey` varchar(64) NOT NULL,
  `ThirdAccountGetterClassType` varchar(255) DEFAULT NULL,
  `AppKey` varchar(255) DEFAULT NULL,
  `AppSecret` varchar(255) DEFAULT NULL,
  `IsEnabled` int(11) DEFAULT NULL,
  PRIMARY KEY (`AccountTypeKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_advertisingpositions
-- ----------------------------
DROP TABLE IF EXISTS `tn_advertisingpositions`;
CREATE TABLE `tn_advertisingpositions` (
  `PositionId` bigint(20) NOT NULL,
  `Description` longtext DEFAULT NULL,
  `Width` int(11) DEFAULT NULL,
  `Height` int(11) DEFAULT NULL,
  `IsEnable` int(11) DEFAULT NULL,
  `ImageAttachmentId` bigint(20) DEFAULT NULL,
  `IsLocked` int(11) DEFAULT NULL,
  PRIMARY KEY (`PositionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_advertisings
-- ----------------------------
DROP TABLE IF EXISTS `tn_advertisings`;
CREATE TABLE `tn_advertisings` (
  `AdvertisingId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `AdvertisingType` int(11) DEFAULT NULL,
  `Body` longtext,
  `LinkUrl` varchar(255) DEFAULT NULL,
  `IsEnable` int(11) DEFAULT NULL,
  `TargetBlank` int(11) DEFAULT NULL,
  `StartDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `EndDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `DisplayOrder` bigint(20) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  `ImageAttachmentId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`AdvertisingId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_advertisingsinpositions
-- ----------------------------
DROP TABLE IF EXISTS `tn_advertisingsinpositions`;
CREATE TABLE `tn_advertisingsinpositions` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `AdvertisingId` bigint(20) DEFAULT NULL,
  `PositionId` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_areas
-- ----------------------------
DROP TABLE IF EXISTS `tn_areas`;
CREATE TABLE `tn_areas` (
  `AreaCode` varchar(8) NOT NULL,
  `ParentCode` varchar(8) DEFAULT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `PostCode` varchar(8) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `Depth` int(11) DEFAULT NULL,
  `ChildCount` int(11) DEFAULT NULL,
  PRIMARY KEY (`AreaCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_attachmentaccessrecords
-- ----------------------------
DROP TABLE IF EXISTS `tn_attachmentaccessrecords`;
CREATE TABLE `tn_attachmentaccessrecords` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `AttachmentId` int(11) DEFAULT NULL,
  `AccessType` int(11) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `UserDisplayName` varchar(64) DEFAULT NULL,
  `Price` int(11) DEFAULT NULL,
  `LastDownloadDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `DownloadDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `IP` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_attachments
-- ----------------------------
DROP TABLE IF EXISTS `tn_attachments`;
CREATE TABLE `tn_attachments` (
  `AttachmentId` bigint(20) NOT NULL AUTO_INCREMENT,
  `AssociateId` bigint(20) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `UserDisplayName` varchar(64) DEFAULT NULL,
  `FileName` varchar(255) DEFAULT NULL,
  `FriendlyFileName` varchar(255) DEFAULT NULL,
  `MediaType` int(11) DEFAULT NULL,
  `ContentType` varchar(128) DEFAULT NULL,
  `FileLength` bigint(20) DEFAULT NULL,
  `Price` int(11) DEFAULT NULL,
  `IP` varchar(64) DEFAULT NULL,
  `BrowseIsReady` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Discription` longtext DEFAULT NULL,
  `IsShowInAttachmentList` int(11) DEFAULT NULL,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  `DisplayOrder` int(11) DEFAULT NULL,
  PRIMARY KEY (`AttachmentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_attituderecords
-- ----------------------------
DROP TABLE IF EXISTS `tn_attituderecords`;
CREATE TABLE `tn_attituderecords` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `ObjectId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_attitudes
-- ----------------------------
DROP TABLE IF EXISTS `tn_attitudes`;
CREATE TABLE `tn_attitudes` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `ObjectId` bigint(20) DEFAULT NULL,
  `SupportCount` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_atusers
-- ----------------------------
DROP TABLE IF EXISTS `tn_atusers`;
CREATE TABLE `tn_atusers` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `AssociateId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_audititems
-- ----------------------------
DROP TABLE IF EXISTS `tn_audititems`;
CREATE TABLE `tn_audititems` (
  `ItemKey` varchar(32) NOT NULL,
  `ItemName` varchar(64) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  PRIMARY KEY (`ItemKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_audititemsinuserroles
-- ----------------------------
DROP TABLE IF EXISTS `tn_audititemsinuserroles`;
CREATE TABLE `tn_audititemsinuserroles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RoleId` bigint(20) DEFAULT NULL,
  `ItemKey` varchar(32) DEFAULT NULL,
  `StrictDegree` smallint(6) DEFAULT NULL,
  `IsLocked` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_categories
-- ----------------------------
DROP TABLE IF EXISTS `tn_categories`;
CREATE TABLE `tn_categories` (
  `CategoryId` bigint(20) NOT NULL AUTO_INCREMENT,
  `ParentId` bigint(20) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  `CategoryName` varchar(128) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `Depth` int(11) DEFAULT NULL,
  `ChildCount` int(11) DEFAULT NULL,
  `ItemCount` int(11) DEFAULT NULL,
  `ImageAttachmentId` bigint(20) DEFAULT NULL,
  `LastModified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `DateCreated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_categorymanagers
-- ----------------------------
DROP TABLE IF EXISTS `tn_categorymanagers`;
CREATE TABLE `tn_categorymanagers` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CategoryId` bigint(20) DEFAULT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  `ReferenceCategoryId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_comments
-- ----------------------------
DROP TABLE IF EXISTS `tn_comments`;
CREATE TABLE `tn_comments` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ParentIds` longtext,
  `ParentId` bigint(20) DEFAULT NULL,
  `CommentedObjectId` bigint(20) DEFAULT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  `CommentType` int(11) DEFAULT NULL,
  `ChildrenCount` int(11) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `Author` varchar(64) DEFAULT NULL,
  `Subject` varchar(255) DEFAULT NULL,
  `Body` longtext,
  `IsAnonymous` int(11) DEFAULT NULL,
  `IsPrivate` int(11) DEFAULT NULL,
  `ApprovalStatus` int(11) DEFAULT NULL,
  `IP` varchar(64) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1439 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_contentcategories
-- ----------------------------
DROP TABLE IF EXISTS `tn_contentcategories`;
CREATE TABLE `tn_contentcategories` (
  `CategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(255) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `ParentIdList` varchar(255) DEFAULT NULL,
  `ChildCount` int(11) DEFAULT NULL,
  `Depth` int(11) DEFAULT NULL,
  `IsEnabled` int(11) DEFAULT NULL,
  `ContentCount` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ContentModelKeys` varchar(255) DEFAULT NULL,
  `ProcessDefinitionId` bigint(20) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_contentitems
-- ----------------------------
DROP TABLE IF EXISTS `tn_contentitems`;
CREATE TABLE `tn_contentitems` (
  `ContentItemId` bigint(20) NOT NULL AUTO_INCREMENT,
  `ContentCategoryId` int(11) DEFAULT NULL,
  `ContentModelId` int(11) DEFAULT NULL,
  `Subject` varchar(255) DEFAULT NULL,
  `FeaturedImageAttachmentId` bigint(20) DEFAULT NULL,
  `DepartmentGuid` varchar(64) DEFAULT NULL,
   `Points` int(11) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `Author` varchar(64) DEFAULT NULL,
  `Body` longtext,
  `Summary` varchar(255) DEFAULT NULL,
  `IsLocked` int(11) DEFAULT NULL,
  `IsSticky` int(11) DEFAULT NULL,
  `ApprovalStatus` smallint(6) DEFAULT NULL,
  `IP` varchar(64) DEFAULT NULL,
  `DatePublished` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `DateCreated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `LastModified` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`ContentItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_contentmodeladditionalfields
-- ----------------------------
DROP TABLE IF EXISTS `tn_contentmodeladditionalfields`;
CREATE TABLE `tn_contentmodeladditionalfields` (
  `FieIdId` int(11) NOT NULL AUTO_INCREMENT,
  `ModelId` int(11) DEFAULT NULL,
  `FieldName` varchar(64) DEFAULT NULL,
  `FieldLabel` varchar(128) DEFAULT NULL,
  `DataType` varchar(64) DEFAULT NULL,
  `DefaultValue` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`FieIdId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_contentmodels
-- ----------------------------
DROP TABLE IF EXISTS `tn_contentmodels`;
CREATE TABLE `tn_contentmodels` (
  `ModelId` int(11) NOT NULL AUTO_INCREMENT,
  `ModelName` varchar(64) DEFAULT NULL,
  `ModelKey` varchar(64) DEFAULT NULL,
  `IsBuiltIn` int(11) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `PageNew` varchar(128) DEFAULT NULL,
  `PageEdit` varchar(128) DEFAULT NULL,
  `PageManage` varchar(128) DEFAULT NULL,
  `PageList` varchar(128) DEFAULT NULL,
  `PageDetail` varchar(128) DEFAULT NULL,
  `IsEnabled` int(11) DEFAULT NULL,
  `EnableComment` int(11) DEFAULT NULL,
  `AdditionalTableName` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`ModelId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;



-- ----------------------------
-- Table structure for tn_counts
-- ----------------------------
DROP TABLE IF EXISTS `tn_counts`;
CREATE TABLE `tn_counts` (
  `CountId` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `ObjectId` bigint(20) DEFAULT NULL,
  `CountType` varchar(64) DEFAULT NULL,
  `StatisticsCount` int(11) DEFAULT NULL,
  PRIMARY KEY (`CountId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_countsperday
-- ----------------------------
DROP TABLE IF EXISTS `tn_countsperday`;
CREATE TABLE `tn_countsperday` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `ObjectId` bigint(20) DEFAULT NULL,
  `CountType` varchar(64) DEFAULT NULL,
  `ReferenceYear` int(11) DEFAULT NULL,
  `ReferenceMonth` int(11) DEFAULT NULL,
  `ReferenceDay` int(11) DEFAULT NULL,
  `StatisticsCount` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_favorites
-- ----------------------------
DROP TABLE IF EXISTS `tn_favorites`;
CREATE TABLE `tn_favorites` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `ObjectId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_follows
-- ----------------------------
DROP TABLE IF EXISTS `tn_follows`;
CREATE TABLE `tn_follows` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `FollowedUserId` bigint(20) DEFAULT NULL,
  `NoteName` varchar(64) DEFAULT NULL,
  `IsQuietly` int(11) DEFAULT NULL,
  `IsNewFollower` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  `IsMutual` int(11) DEFAULT NULL,
  `LastContactDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_invitationcodes
-- ----------------------------
DROP TABLE IF EXISTS `tn_invitationcodes`;
CREATE TABLE `tn_invitationcodes` (
  `Code` varchar(32) NOT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `IsMultiple` int(11) DEFAULT NULL,
  `ExpiredDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `DateCreated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_invitefriendrecords
-- ----------------------------
DROP TABLE IF EXISTS `tn_invitefriendrecords`;
CREATE TABLE `tn_invitefriendrecords` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `InvitedUserId` bigint(20) DEFAULT NULL,
  `Code` varchar(255) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsRewarded` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_itemsincategories
-- ----------------------------
DROP TABLE IF EXISTS `tn_itemsincategories`;
CREATE TABLE `tn_itemsincategories` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CategoryId` bigint(20) DEFAULT NULL,
  `ItemId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_itemsintags
-- ----------------------------
DROP TABLE IF EXISTS `tn_itemsintags`;
CREATE TABLE `tn_itemsintags` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TagName` varchar(128) DEFAULT NULL,
  `ItemId` bigint(20) DEFAULT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_kvstore
-- ----------------------------
DROP TABLE IF EXISTS `tn_kvstore`;
CREATE TABLE `tn_kvstore` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Tkey` varchar(128) DEFAULT NULL,
  `TValue` longtext,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_links
-- ----------------------------
DROP TABLE IF EXISTS `tn_links`;
CREATE TABLE `tn_links` (
  `LinkId` bigint(20) NOT NULL AUTO_INCREMENT,
  `LinkName` varchar(128) DEFAULT NULL,
  `LinkUrl` varchar(255) DEFAULT NULL,
  `ImageAttachmentId` bigint(20) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `IsEnabled` int(11) DEFAULT NULL,
  `DisplayOrder` bigint(20) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`LinkId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_listitems
-- ----------------------------
DROP TABLE IF EXISTS `tn_listitems`;
CREATE TABLE `tn_listitems` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ItemCode` varchar(32) DEFAULT NULL,
  `ListCode` varchar(32) DEFAULT NULL,
  `ParentCode` varchar(32) DEFAULT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `ChildrenCount` int(11) DEFAULT NULL,
  `Depth` int(11) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_lists
-- ----------------------------
DROP TABLE IF EXISTS `tn_lists`;
CREATE TABLE `tn_lists` (
  `Code` varchar(32) NOT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `IsMultilevel` int(11) DEFAULT NULL,
  `AllowAddOrDelete` int(11) DEFAULT NULL,
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_messages
-- ----------------------------
DROP TABLE IF EXISTS `tn_messages`;
CREATE TABLE `tn_messages` (
  `MessageId` bigint(20) NOT NULL AUTO_INCREMENT,
  `SenderUserId` bigint(20) DEFAULT NULL,
  `Sender` varchar(64) DEFAULT NULL,
  `ReceiverUserId` bigint(20) DEFAULT NULL,
  `Receiver` varchar(64) DEFAULT NULL,
  `Subject` varchar(255) DEFAULT NULL,
  `Body` varchar(255) DEFAULT NULL,
  `IsRead` int(11) DEFAULT NULL,
  `IP` varchar(64) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`MessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_messagesessions
-- ----------------------------
DROP TABLE IF EXISTS `tn_messagesessions`;
CREATE TABLE `tn_messagesessions` (
  `SessionId` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `OtherUserId` bigint(20) DEFAULT NULL,
  `LastMessageId` bigint(20) DEFAULT NULL,
  `MessageCount` int(11) DEFAULT NULL,
  `UnreadMessageCount` int(11) DEFAULT NULL,
  `MessageType` int(11) DEFAULT NULL,
  `LastModified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `AsAnonymous` int(11) DEFAULT NULL,
  `SenderSessionId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`SessionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_messagesinsessions
-- ----------------------------
DROP TABLE IF EXISTS `tn_messagesinsessions`;
CREATE TABLE `tn_messagesinsessions` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `SessionId` bigint(20) DEFAULT NULL,
  `MessageId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_navigations
-- ----------------------------
DROP TABLE IF EXISTS `tn_navigations`;
CREATE TABLE `tn_navigations` (
  `NavigationId` int(11) NOT NULL AUTO_INCREMENT,
  `ParentNavigationId` int(11) DEFAULT NULL,
  `Depth` int(11) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `NavigationType` int(11) DEFAULT NULL,
  `NavigationText` varchar(64) DEFAULT NULL,
  `NavigationUrl` varchar(255) DEFAULT NULL,
  `UrlRouteName` varchar(64) DEFAULT NULL,
  `RouteDataName` varchar(255) DEFAULT NULL,
  `NavigationTarget` varchar(32) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `IsLocked` int(11) DEFAULT NULL,
  `IsEnabled` int(11) DEFAULT NULL,
  PRIMARY KEY (`NavigationId`)
) ENGINE=InnoDB AUTO_INCREMENT=20000882 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_notices
-- ----------------------------
DROP TABLE IF EXISTS `tn_notices`;
CREATE TABLE `tn_notices` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `NoticeTypeKey` varchar(64) DEFAULT NULL,
  `ReceiverId` bigint(20) DEFAULT NULL,
  `LeadingActorUserId` bigint(20) DEFAULT NULL,
  `LeadingActor` varchar(64) DEFAULT NULL,
  `RelativeObjectName` varchar(128) DEFAULT NULL,
  `RelativeObjectId` bigint(20) DEFAULT NULL,
  `RelativeObjectUrl` varchar(255) DEFAULT NULL,
  `Body` varchar(255) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Times` int(11) DEFAULT NULL,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  `LastSendDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `ObjectId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_noticetypes
-- ----------------------------
DROP TABLE IF EXISTS `tn_noticetypes`;
CREATE TABLE `tn_noticetypes` (
  `NoticeTypeKey` varchar(64) NOT NULL,
  `Name` varchar(128) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  PRIMARY KEY (`NoticeTypeKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_noticetypesettings
-- ----------------------------
DROP TABLE IF EXISTS `tn_noticetypesettings`;
CREATE TABLE `tn_noticetypesettings` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `NoticeTypeKey` varchar(64) DEFAULT NULL,
  `Time` int(11) DEFAULT NULL,
  `IntervaI` int(11) DEFAULT NULL,
  `SendMode` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_onlineusers
-- ----------------------------
DROP TABLE IF EXISTS `tn_onlineusers`;
CREATE TABLE `tn_onlineusers` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `UserName` varchar(64) DEFAULT NULL,
  `DisplayName` varchar(64) DEFAULT NULL,
  `LastActivityTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `LastAction` varchar(255) DEFAULT NULL,
  `Ip` varchar(64) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_operationlogs
-- ----------------------------
DROP TABLE IF EXISTS `tn_operationlogs`;
CREATE TABLE `tn_operationlogs` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `OperationType` varchar(64) DEFAULT NULL,
  `OperationObjectId` bigint(20) DEFAULT NULL,
  `OperationObjectName` varchar(2000) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `OperationUserRole` varchar(64) DEFAULT NULL,
  `OperationUserId` bigint(20) DEFAULT NULL,
  `Operator` varchar(64) DEFAULT NULL,
  `OperatorIP` varchar(64) DEFAULT NULL,
  `AccessUrl` varchar(255) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_permissionitems
-- ----------------------------
DROP TABLE IF EXISTS `tn_permissionitems`;
CREATE TABLE `tn_permissionitems` (
  `ItemKey` varchar(32) NOT NULL,
  `ItemName` varchar(64) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `Discription` longtext DEFAULT NULL,
  PRIMARY KEY (`ItemKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_permissions
-- ----------------------------
DROP TABLE IF EXISTS `tn_permissions`;
CREATE TABLE `tn_permissions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PermissionItemKey` varchar(32) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `OwnerType` int(11) DEFAULT NULL,
  `IsLocked` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_pointcategories
-- ----------------------------
DROP TABLE IF EXISTS `tn_pointcategories`;
CREATE TABLE `tn_pointcategories` (
  `CategoryKey` varchar(32) NOT NULL,
  `CategoryName` varchar(64) DEFAULT NULL,
  `Unit` varchar(8) DEFAULT NULL,
  `QuotaPerDay` int(11) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  PRIMARY KEY (`CategoryKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_pointitems
-- ----------------------------
DROP TABLE IF EXISTS `tn_pointitems`;
CREATE TABLE `tn_pointitems` (
  `ItemKey` varchar(32) NOT NULL,
  `ItemName` varchar(64) DEFAULT NULL,
  `DisplayOrder` int(11) DEFAULT NULL,
  `ExperiencePoints` int(11) DEFAULT NULL,
  `ReputationPoints` int(11) DEFAULT NULL,
  `TradePoints` int(11) DEFAULT NULL,
  `TradePoints2` int(11) DEFAULT NULL,
  `TradePoints3` int(11) DEFAULT NULL,
  `TradePoints4` int(11) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  PRIMARY KEY (`ItemKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_pointrecords
-- ----------------------------
DROP TABLE IF EXISTS `tn_pointrecords`;
CREATE TABLE `tn_pointrecords` (
  `RecordId` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `OperatorUserId` bigint(20) DEFAULT NULL,
  `PointItemName` varchar(64) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `ExperiencePoints` int(11) DEFAULT NULL,
  `ReputationPoints` int(11) DEFAULT NULL,
  `TradePoints` int(11) DEFAULT NULL,
  `TradePoints2` int(11) DEFAULT NULL,
  `TradePoints3` int(11) DEFAULT NULL,
  `TradePoints4` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`RecordId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_relatedtags
-- ----------------------------
DROP TABLE IF EXISTS `tn_relatedtags`;
CREATE TABLE `tn_relatedtags` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TagId` bigint(20) DEFAULT NULL,
  `RelatedTagId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_reviews
-- ----------------------------
DROP TABLE IF EXISTS `tn_reviews`;
CREATE TABLE `tn_reviews` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `ParentId` bigint(20) DEFAULT NULL,
  `ReviewedObjectId` bigint(20) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `Author` varchar(64) DEFAULT NULL,
  `Body` longtext,
  `RateNumber` int(11) DEFAULT NULL,
  `ReviewRank` int(11) DEFAULT NULL,
  `IsAnonymous` int(11) DEFAULT NULL,
  `IP` varchar(64) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_reviewsummaries
-- ----------------------------
DROP TABLE IF EXISTS `tn_reviewsummaries`;
CREATE TABLE `tn_reviewsummaries` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `ReviewedObjectId` bigint(20) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `RateSum` int(11) DEFAULT NULL,
  `RateCount` int(11) DEFAULT NULL,
  `PositiveReivewCount` int(11) DEFAULT NULL,
  `ModerateReivewCount` int(11) DEFAULT NULL,
  `NegativeReivewCount` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_roles
-- ----------------------------
DROP TABLE IF EXISTS `tn_roles`;
CREATE TABLE `tn_roles` (
  `RoleId` bigint(20) NOT NULL,
  `RoleName` varchar(64) DEFAULT NULL,
  `IsBuiltIn` int(11) DEFAULT NULL,
  `ConnectToUser` int(11) DEFAULT NULL,
  `IsPublic` int(11) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `RoleImageAttachmentId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_sections
-- ----------------------------
DROP TABLE IF EXISTS `tn_sections`;
CREATE TABLE `tn_sections` (
  `SectionId` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `Description` longtext,
  `FeaturedImageAttachmentId` bigint(20) DEFAULT NULL,
  `IsEnabled` int(11) DEFAULT NULL,
  `ThreadCategorySettings` smallint(6) DEFAULT NULL,
  `DisplayOrder` bigint(20) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`SectionId`)
) ENGINE=InnoDB AUTO_INCREMENT=1643857892083 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_settings
-- ----------------------------
DROP TABLE IF EXISTS `tn_settings`;
CREATE TABLE `tn_settings` (
  `ClassType` varchar(128) NOT NULL,
  `Settings` longtext,
  PRIMARY KEY (`ClassType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_specialcontentitems
-- ----------------------------
DROP TABLE IF EXISTS `tn_specialcontentitems`;
CREATE TABLE `tn_specialcontentitems` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `TypeId` int(11) DEFAULT NULL,
  `RegionId` bigint(20) DEFAULT NULL,
  `ItemId` bigint(20) DEFAULT NULL,
  `ItemName` varchar(255) DEFAULT NULL,
  `FeaturedImageAttachmentId` bigint(20) DEFAULT NULL,
  `Recommender` varchar(64) DEFAULT NULL,
  `RecommenderUserId` bigint(20) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ExpiredDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `DisplayOrder` bigint(20) DEFAULT NULL,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_specialcontenttypes
-- ----------------------------
DROP TABLE IF EXISTS `tn_specialcontenttypes`;
CREATE TABLE `tn_specialcontenttypes` (
  `TypeId` int(11) NOT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `RequireFeaturedImage` int(11) DEFAULT NULL,
  `RequireExpiredDate` int(11) DEFAULT NULL,
  `FeaturedImageDescrption` char(64) DEFAULT NULL,
  `IsBuiltIn` int(11) DEFAULT NULL,
  `AllowExternalLink` int(11) DEFAULT NULL,
  PRIMARY KEY (`TypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_tags
-- ----------------------------
DROP TABLE IF EXISTS `tn_tags`;
CREATE TABLE `tn_tags` (
  `TagId` bigint(20) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `TagName` varchar(64) DEFAULT NULL,
  `Description` longtext DEFAULT NULL,
  `ImageAttachmentId` bigint(20) DEFAULT NULL,
  `IsFeatured` int(11) DEFAULT NULL,
  `ItemCount` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`TagId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_taskdetails
-- ----------------------------
DROP TABLE IF EXISTS `tn_taskdetails`;
CREATE TABLE `tn_taskdetails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) DEFAULT NULL,
  `TaskRule` varchar(64) DEFAULT NULL,
  `ClassType` varchar(255) DEFAULT NULL,
  `Enabled` int(11) DEFAULT NULL,
  `RunAtRestart` int(11) DEFAULT NULL,
  `IsRunning` int(11) DEFAULT NULL,
  `LastStart` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `LastEnd` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `LastIsSuccess` int(11) DEFAULT NULL,
  `NextStart` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `StartDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `EndDate` timestamp  NULL,
  `RunAtServer` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_tenanttypes
-- ----------------------------
DROP TABLE IF EXISTS `tn_tenanttypes`;
CREATE TABLE `tn_tenanttypes` (
  `TenantTypeId` char(6) NOT NULL,
  `Name` varchar(32) DEFAULT NULL,
  `ClassType` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`TenantTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_tenanttypesinservices
-- ----------------------------
DROP TABLE IF EXISTS `tn_tenanttypesinservices`;
CREATE TABLE `tn_tenanttypesinservices` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TenantTypeId` char(6) DEFAULT NULL,
  `ServiceKey` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=126 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_threads
-- ----------------------------
DROP TABLE IF EXISTS `tn_threads`;
CREATE TABLE `tn_threads` (
  `ThreadId` bigint(20) NOT NULL AUTO_INCREMENT,
  `SectionId` bigint(20) DEFAULT NULL,
  `TenantTypeId` char(6) DEFAULT NULL,
  `OwnerId` bigint(20) DEFAULT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `Author` varchar(64) DEFAULT NULL,
  `Subject` varchar(128) DEFAULT NULL,
  `Body` longtext,
  `IsLocked` int(11) DEFAULT NULL,
  `IsSticky` int(11) DEFAULT NULL,
  `ApprovalStatus` smallint(6) DEFAULT NULL,
  `IP` varchar(60) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `LastModified` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `PropertyNames` longtext,
  `PropertyValues` longtext,
  PRIMARY KEY (`ThreadId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_userranks
-- ----------------------------
DROP TABLE IF EXISTS `tn_userranks`;
CREATE TABLE `tn_userranks` (
  `Rank` int(11) NOT NULL,
  `PointLower` int(11) DEFAULT NULL,
  `RankName` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`Rank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_users
-- ----------------------------
DROP TABLE IF EXISTS `tn_users`;
CREATE TABLE `tn_users` (
  `UserId` bigint(20) NOT NULL,
  `UserName` varchar(64) DEFAULT NULL,
  `Password` varchar(128) DEFAULT NULL,
  `HasAvatar` int(11) DEFAULT NULL,
  `HasCover` int(11) DEFAULT NULL,
  `PasswordFormat` int(11) DEFAULT NULL,
  `AccountEmail` varchar(64) DEFAULT NULL,
  `IsEmailVerified` int(11) DEFAULT NULL,
  `AccountMobile` varchar(64) DEFAULT NULL,
  `IsMobileVerified` int(11) DEFAULT NULL,
  `TrueName` varchar(64) DEFAULT NULL,
  `ForceLogin` int(11) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `DateCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IpCreated` varchar(64) DEFAULT NULL,
  `UserType` int(11) DEFAULT NULL,
  `LastActivityTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `LastAction` varchar(255) DEFAULT NULL,
  `IpLastActivity` varchar(64) DEFAULT NULL,
  `IsBanned` int(11) DEFAULT NULL,
  `BanReason` varchar(64) DEFAULT NULL,
  `BanDeadline` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `IsModerated` int(11) DEFAULT NULL,
  `IsForceModerated` int(11) DEFAULT NULL,
  `DatabaseQuota` int(11) DEFAULT NULL,
  `DatabaseQuotaUsed` int(11) DEFAULT NULL,
  `IsUseCustomStyle` int(11) DEFAULT NULL,
  `FollowedCount` int(11) DEFAULT NULL,
  `FollowerCount` int(11) DEFAULT NULL,
  `ExperiencePoints` int(11) DEFAULT NULL,
  `ReputationPoints` int(11) DEFAULT NULL,
  `TradePoints` int(11) DEFAULT NULL,
  `TradePoints2` int(11) DEFAULT NULL,
  `TradePoints3` int(11) DEFAULT NULL,
  `TradePoints4` int(11) DEFAULT NULL,
  `FrozenTradePoints` int(11) DEFAULT NULL,
  `Rank` int(11) DEFAULT NULL,
  `AuditStatus` smallint(6) DEFAULT NULL,
  `UserGuid` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tn_usersinroles
-- ----------------------------
DROP TABLE IF EXISTS `tn_usersinroles`;
CREATE TABLE `tn_usersinroles` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) DEFAULT NULL,
  `RoleId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8;


-- 定时任务
INSERT INTO `tn_taskdetails` VALUES ('2', '保存计数队列（每分钟执行一次）', '0 * * * * ?', 'Tunynet.Common.ExecCountQueueTask,Tunynet.Core', '1', '0', '0', '2017-04-28 17:45:01', '2017-04-28 17:45:01', '1', '2017-04-28 09:46:00', '2012-01-01 00:00:00', null, '0');
INSERT INTO `tn_taskdetails` VALUES ('3', '更新阶段计数 （每分钟执行一次）', '0 0/1 * * * ?', 'Tunynet.Common.UpdateStageCountTask,Tunynet.Core', '1', '0', '0', '2017-04-28 17:45:01', '2017-04-28 17:45:01', '1', '2017-04-28 09:46:00', '2012-01-01 00:00:00', null, '0');
INSERT INTO `tn_taskdetails` VALUES ('10', '解封用户 （每天凌晨1点执行）', '0 0 1 * * ?', 'Tunynet.Common.UnbanUserTask,Tunynet.Core', '1', '1', '0', '2017-04-28 17:44:38', '2017-04-28 17:44:38', '1', '2017-04-28 17:00:00', '2012-08-08 00:00:00', null, '0');
INSERT INTO `tn_taskdetails` VALUES ('11', '清理垃圾临时附件（每天凌晨1点执行）', '0 0 1 * * ?', 'Tunynet.Common.DeleteTrashTemporaryAttachmentsTask,Tunynet.Core', '1', '0', '0', '2017-04-28 17:44:38', '2017-04-28 17:44:38', '1', '2017-04-28 17:00:00', '2012-08-08 11:48:32', null, '0');
INSERT INTO `tn_taskdetails` VALUES ('14', '资讯索引任务(每分钟执行)', '0 0/1 * * * ?', 'Tunynet.Common.CmsSearchTask,Spacebuilder.SearchApi', '1', '0', '0', '2017-04-28 17:45:01', '2017-04-28 17:45:01', '1', '2017-04-28 09:46:00', '2012-08-08 00:00:00', null, '2');
INSERT INTO `tn_taskdetails` VALUES ('15', '评论索引任务(每分钟执行)', '0 0/1 * * * ?', 'Tunynet.Common.CommentSearchTask,Spacebuilder.SearchApi', '1', '0', '0', '2017-04-28 17:45:01', '2017-04-28 17:45:01', '1', '2017-04-28 09:46:00', '2012-08-08 00:00:00', null, '2');
INSERT INTO `tn_taskdetails` VALUES ('16', '贴子索引任务(每分钟执行)', '0 0/1 * * * ?', 'Tunynet.Common.ThreadSearchTask,Spacebuilder.SearchApi', '1', '0', '0', '2017-04-28 17:45:01', '2017-04-28 17:45:01', '1', '2017-04-28 09:46:00', '2012-08-08 00:00:00', null, '2');
INSERT INTO `tn_taskdetails` VALUES ('26', '清除邀请码（每天凌晨1点执行）', '0 0 1 * * ?', 'Tunynet.Common.DeleteTrashInvitationCodesTask,Tunynet.Modules', '1', '1', '0', '2017-04-28 17:44:38', '2017-04-28 17:44:38', '1', '2017-04-28 17:00:00', '2012-01-01 00:00:00', null, '0');
INSERT INTO `tn_taskdetails` VALUES ('28', '定期移除垃圾数据（每天凌晨1点执行）', '0 0/1 * * * ? ', 'Tunynet.Common.DeleteTrashDataTask,Tunynet.Spacebuilder', '1', '0', '0', '2017-04-28 17:45:01', '2017-04-28 17:45:01', '1', '2017-04-28 09:46:00', '2012-08-08 11:48:32', null, '0');
INSERT INTO `tn_taskdetails` VALUES ('29', '定期移除过期推荐（每天凌晨1点执行）', '0 0 1 * * ?', 'Tunynet.Common.DeleteOverdueSpecialTask,Tunynet.Modules', '1', '0', '0', '2017-04-28 17:44:38', '2017-04-28 17:44:38', '1', '2017-04-28 17:00:00', '2012-08-08 11:48:32', null, '0');

-- 租户类型
INSERT INTO `tn_tenanttypes` VALUES ('000001', '用户', 'Tunynet.Common.User,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('000002', '角色', 'Tunynet.Common.Role,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('000021', '分类', 'Tunynet.Common.Category,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('000031', '评论', 'Tunynet.Common.Comment,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('000041', '标签', 'Tunynet.Common.Tag,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000051', '附件', 'Tunynet.Common.Attachment,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('000061', '推荐', 'Tunynet.Common.SpecialContentItem,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000071', '友情链接', 'Tunynet.Common.LinkEntity,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000081', '广告', 'Tunynet.Common.Advertising,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000082', '广告位', 'Tunynet.Common.AdvertisingPosition,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000101', '评价', 'Tunynet.Common.Review,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000121', '权限', 'Tunynet.Common.Permission,Tunynet.Modules');
INSERT INTO `tn_tenanttypes` VALUES ('000131', '导航', 'Tunynet.UI.Navigation,Tunynet.Presentation');
INSERT INTO `tn_tenanttypes` VALUES ('100001', '板块', 'Tunynet.Post.Section,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('100002', '贴子', 'Tunynet.Post.Thread,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('100003', '贴吧', '');
INSERT INTO `tn_tenanttypes` VALUES ('100011', '资讯', 'Tunynet.CMS.ContentItem,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('100012', '资讯栏目', 'Tunynet.CMS.ContentCategory,Tunynet.Core');
INSERT INTO `tn_tenanttypes` VALUES ('100013', '资讯_文章', '');
INSERT INTO `tn_tenanttypes` VALUES ('100014', '资讯_组图', '');
INSERT INTO `tn_tenanttypes` VALUES ('100015', '资讯_视频', '');

-- 第三方账号类型
INSERT INTO `tn_accounttypes` VALUES ('QQ', 'Tunynet.Spacebuilder.QQAccountGetter,Tunynet.AccountBindings', '', '', '0');
INSERT INTO `tn_accounttypes` VALUES ('WeChat', 'Tunynet.Spacebuilder.WeChatAccountGetter,Tunynet.AccountBindings', '', '', '0');
-- 配置数据
INSERT INTO `tn_settings` VALUES ('Spacebuilder.Common.Configuration.UserProfileSettings, Tunynet.Core', '{\"OriginalAvatarWidth\":350,\"OriginalAvatarHeight\":350,\"AvatarWidth\":160,\"AvatarHeight\":160,\"MediumAvatarWidth\":100,\"MediumAvatarHeight\":100,\"SmallAvatarWidth\":50,\"SmallAvatarHeight\":50,\"MicroAvatarWidth\":25,\"MicroAvatarHeight\":25,\"IntegrityProportions\":[20,10,10,10,10,0,15,15,10],\"MinIntegrity\":50,\"MaxPersonTag\":10}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Attachments.FileSettings, Tunynet.Core', '{\"MaxAttachmentLength\":10240,\"BatchUploadLimit\":10,\"AllowedFileExtensions\":\"zip,rar,xml,txt,gif,jpg,jpeg,png,doc,xls,ppt,pdf,swf,flv,mp3,wma,mmv,rm,avi,mov,qt,docx,pptx,xlsx,pps\",\"TemporaryAttachmentStorageDay\":3,\"WatermarkSettings\":{\"WatermarkType\":2,\"WatermarkLocation\":8,\"WatermarkText\":\"近乎\",\"WatermarkImageName\":\"watermark.png\",\"WatermarkMinWidth\":300,\"WatermarkMinHeight\":300,\"WatermarkOpacity\":0.6,\"WatermarkImageDirectory\":\"~/Images/\",\"WatermarkImagePhysicalPath\":\"\\\\Images\\\\watermark.png\"},\"MaxImageWidth\":1920,\"MaxImageHeight\":1920,\"InlinedImageWidth\":800,\"InlinedImageHeight\":600}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Attitude.AttitudeOnlySupportSettings, Tunynet.Modules', '{\"IsCancel\":true}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Attitude.AttitudeSettings, Tunynet.Modules', '{\"SupportWeights\":2,\"OpposeWeights\":1,\"EnableCancel\":false,\"IsModify\":true}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.AreaSettings, Tunynet.Modules', '{\"AreaLevel\":4,\"RootAreaCode\":\"A1560000\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.Configuration.CommentSettings, Tunynet.Core', '{\"_showCommentCount\":false,\"EnableComment\":true,\"ShowCommentCount\":false,\"EnableSupportOppose\":false,\"ShowLowCommentOnLoad\":true,\"MaxCommentLength\":140,\"EnablePrivate\":false,\"AllowAnonymousComment\":false,\"EntryBoxAutoHeight\":true,\"CommentClass\":\"\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.Configuration.UserSettings, Tunynet.Core', '{\"RegisterType\":30,\"MinUserNameLength\":2,\"MaxUserNameLength\":64,\"UserNameRegex\":\"^[\\\\w|\\\\u4e00-\\\\u9fa5]{1,64}$\",\"PhoneRegex\":\"^(13|14|15|18)[0-9]{9}$\",\"NickNameRegex\":\"^[\\\\w|\\\\u4e00-\\\\u9fa5]{1,64}$\",\"MinPasswordLength\":4,\"MinRequiredNonAlphanumericCharacters\":0,\"EmailRegex\":\"^([a-zA-Z0-9_\\\\.-])+@([a-zA-Z0-9_-])+((\\\\.[a-zA-Z0-9_-]{2,3}){1,2})$\",\"EnableTrackAnonymous\":true,\"UserOnlineTimeWindow\":20,\"EnableNotActivatedUsersToLogin\":false,\"RequiresUniqueMobile\":true,\"UserPasswordFormat\":1,\"EnableNickname\":true,\"EnablePhone\":true,\"DisplayNameType\":2,\"AutomaticModerated\":false,\"NoModeratedUserPoint\":11,\"DisallowedUserNames\":\"admin，administrator，super\",\"SuperAdministratorRoleId\":101,\"AnonymousRoleId\":122,\"EnableAudit\":true,\"NoAuditedRoleNames\":[101,111],\"NoCreatedRoleIds\":[121,123,122],\"MinNoAuditedUserRank\":8}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.ContentItemSettings, Tunynet.Core', '{\"AuditStatus\":40}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.EmotionSettings, Tunynet.Modules', '{\"EmoticonPath\":\"~/Img/Emotions\",\"EnableDirectlyUrl\":false,\"DirectlyRootUrl\":\"\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.EmotionSettings, Tunynet.Presentation', '{\"EmoticonPath\":\"~/Img/Emotions\",\"EnableDirectlyUrl\":false,\"DirectlyRootUrl\":\"\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.FileSettings, Tunynet.Core', '{\"MaxAttachmentLength\":10240,\"BatchUploadLimit\":10,\"AllowedFileExtensions\":\"zip,rar,xml,txt,gif,jpg,jpeg,png,doc,xls,ppt,pdf,swf,flv,mp3,wma,mmv,rm,avi,mov,qt,docx,pptx,xlsx,pps\",\"TemporaryAttachmentStorageDay\":3,\"WatermarkSettings\":{\"WatermarkType\":2,\"WatermarkLocation\":8,\"WatermarkText\":\"近乎\",\"WatermarkImageName\":\"watermark.png\",\"WatermarkMinWidth\":300,\"WatermarkMinHeight\":300,\"WatermarkOpacity\":0.6,\"WatermarkImageDirectory\":\"~/Images/\",\"WatermarkImagePhysicalPath\":\"E:\\\\拓宇CMS\\\\代码区域\\\\trunk\\\\程序代码\\\\Tunynet.CMS\\\\Web\\\\Images\\\\watermark.png\"},\"MaxImageWidth\":1920,\"MaxImageHeight\":1920}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.ImageSettings, Tunynet.Core', '{\"WatermarkSettings\":{\"WatermarkType\":1,\"WatermarkLocation\":8,\"WatermarkText\":\"近乎\",\"WatermarkImageName\":\"watermark.png\",\"WatermarkMinWidth\":300,\"WatermarkMinHeight\":300,\"WatermarkOpacity\":0.6,\"WatermarkImageDirectory\":\"~/img/\",\"WatermarkImagePhysicalPath\":\"E:\\\\配置库\\\\SVNonline\\\\spb5above\\\\代码区域\\\\trunk\\\\Spacebuilder\\\\Web\\\\img\\\\watermark.png\"},\"MaxImageLength\":10240,\"AllowedFileExtensions\":\"gif,jpg,jpeg,png,bpm\",\"MaxImageWidth\":500,\"MaxImageHeight\":500,\"ResizeMethod\":0}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.InviteFriendSettings, Tunynet.Modules', '{\"AllowInvitationCodeUseOnce\":false,\"InvitationCodeTimeLiness\":1,\"InvitationCodeUnitPrice\":1000,\"DefaultUserInvitationCodeCount\":0}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.PauseSiteSettings, Tunynet.Presentation', '{\"pauseAnnouncement\":\"网站暂停中\",\"pauseLink\":\"http://\",\"PauseAnnouncement\":\"网站暂停中\",\"PauseLink\":\"http://\",\"IsEnable\":true,\"PausePageType\":true}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.PointSettings, Tunynet.Core', '{\"ExperiencePointsCoefficient\":1,\"ReputationPointsCoefficient\":2,\"TransactionTax\":0,\"UserIntegratedPointRuleText\":\"经验*1 + 威望*2\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.SectionSettings, Tunynet.Core', '{\"MinimumCreateLevel\":0,\"BodyMaxLength\":500,\"ReplyBodyMaxLength\":500}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.ThreadSettings, Tunynet.Core', '{\"AuditStatus\":10}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.UserProfileSettings, Tunynet.Core', '{\"OriginalAvatarWidth\":350,\"OriginalAvatarHeight\":350,\"AvatarWidth\":160,\"AvatarHeight\":160,\"MediumAvatarWidth\":100,\"MediumAvatarHeight\":100,\"SmallAvatarWidth\":50,\"SmallAvatarHeight\":50,\"MicroAvatarWidth\":25,\"MicroAvatarHeight\":25,\"IntegrityProportions\":[20,10,10,10,10,0,15,15,10],\"MinIntegrity\":50,\"MaxPersonTag\":10}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Common.WatermarkSettings, Tunynet.Core', '{\"WatermarkType\":2,\"WatermarkLocation\":0,\"WatermarkText\":\"近乎\",\"WatermarkImageName\":\"watermark.png\",\"WatermarkMinWidth\":200,\"WatermarkMinHeight\":200,\"WatermarkOpacity\":0.6,\"WatermarkImageDirectory\":\"~/img/\",\"WatermarkImagePhysicalPath\":\"D:\\\\work\\\\spb5above\\\\代码区域\\\\trunk\\\\Spacebuilder\\\\Web\\\\img\\\\watermark.png\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Email.EmailSettings, Tunynet.Infrastructure', '{\"BatchSendLimit\":100,\"AdminEmailAddress\":\"admin@yourdomain.com\",\"NoReplyAddress\":\"noreply@yourdomain.com\",\"NumberOfTries\":6,\"SendTimeInterval\":10,\"SmtpSettings\":null}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Emotion.EmotionSettings, Tunynet.Modules', '{\"EmoticonPath\":\"~/Img/qq\",\"EnableDirectlyUrl\":false,\"DirectlyRootUrl\":\"\"}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Portal.PhotoSiteSettings, Tunynet.Portal', '{\"CategoryID\":43,\"IsHomeDisplay\":false,\"IsLGGKDisplay\":[\"23\",\"42\",\"41\",\"43\"]}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Repositories.Text.BarSettingsTest, Core.Test', '{\"ThreadSubjectMaxLength\":1,\"ThreadBodyMaxLength\":5,\"PostBodyMaxLength\":100,\"EnableRating\":true,\"ReputationPointsMaxValue\":123456789,\"ReputationPointsMinValue\":0,\"UserReputationPointsPerDay\":100,\"EnableUserCreateSection\":true,\"UserRankOfCreateSection\":0,\"OnlyFollowerCreateThread\":true,\"OnlyFollowerCreatePost\":true,\"SectionManagerMaxCount\":3}');
INSERT INTO `tn_settings` VALUES ('Tunynet.Settings.SiteSettings, Tunynet.Core', '{\"BeiAnScript\":\"\\u003ca target=\\\"_blank\\\" href=\\\"http://www.beian.gov.cn/portal/registerSystemInfo?recordcode=370XXXXXXX\\\" style=\\\"display:inline-block;text-decoration:none;height:20px;line-height:20px;\\\"\\u003e鲁公网安备 370XXXXXXX号\\u003c/a\\u003e\",\"StatScript\":\"\\u003ca href=\\\"http://www.cnzz.com\\\" target=\\\"_blank\\\" title=\\\"这是站长统计工具\\\"\\u003e这是站长统计工具\\u003c/a\\u003e\",\"Links\":\"\\u003ca href=\\\"http://www.huobanxietong.com\\\" target=\\\"_blank\\\" title=\\\"伙伴协同\\\"\\u003e伙伴协同\\u003c/a\\u003e\\u003ca href=\\\"http://www.jinhusns.com\\\" target=\\\"_blank\\\" title=\\\"近乎\\\"\\u003e近乎\\u003c/a\\u003e\\u003ca href=\\\"http://www.jinhusns.com\\\" target=\\\"_blank\\\" title=\\\"这是页脚链接\\\"\\u003e这是页脚链接\\u003c/a\\u003e\",\"SiteKey\":\"81246e3f-6222-44b4-b069-2f51e96aa323\",\"SiteName\":\"近乎 \",\"SiteDescription\":\"基于asp.net mvc最强大的SNS社区软件\",\"Copyright\":\"©2005-2017 Tunynet Inc.\\u003ca  target=\\\"_blank\\\" href=\\\"http://www.jinhusns.com\\\"\\u003e青岛拓宇网络科技有限公司\\u003c/a\\u003e\",\"SearchMetaDescription\":\"**近乎（Spacebuilder）是基于asp.net mvc最强大的SNS社区软件。借助预置的资讯、组图、视频、贴吧、问答等系统应用模块，近乎可以帮助客户快速搭建以用户为中心、用户乐于贡献内容、互动无处不在、易于运营的社区网站。\",\"SearchMetaKeyWords\":\"**近乎,近乎SNS,jinhusns,Spacebuilder,SNS社区软件,SNS社区系统,SNS源码,SNS系统,asp.net,SNS,开源SNS社区,开源社区系统,开源社区软件,网络学习空间,三通两平台,网络教育,集体备课,数字校园,知识管理,开源微博系统,群组系统,开源博客系统,相册管理系统,开源贴吧系统,开源问答系统\",\"DefaultLanguage\":\"zh-cn\",\"MainSiteRootUrl\":\"http://localhost\",\"AuditStatus\":19,\"EnableAnonymousBrowse\":true,\"SiteStyle\":0}');
-- 租户与服务关系
INSERT INTO `tn_tenanttypesinservices` VALUES ('62', '100012', 'CategoryManager');
INSERT INTO `tn_tenanttypesinservices` VALUES ('63', '100011', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('64', '000031', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('65', '100001', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('66', '100011', 'Comment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('67', '100002', 'Comment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('68', '100011', 'Tag');
INSERT INTO `tn_tenanttypesinservices` VALUES ('69', '100002', 'Recommend');
INSERT INTO `tn_tenanttypesinservices` VALUES ('70', '100001', 'Recommend');
INSERT INTO `tn_tenanttypesinservices` VALUES ('71', '100011', 'Attitude');
INSERT INTO `tn_tenanttypesinservices` VALUES ('72', '100011', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('73', '100002', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('74', '100001', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('75', '000002', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('76', '000061', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('77', '100001', 'Favorites');
INSERT INTO `tn_tenanttypesinservices` VALUES ('78', '100001', 'CategoryManager');
INSERT INTO `tn_tenanttypesinservices` VALUES ('79', '000021', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('80', '100002', 'Favorites');
INSERT INTO `tn_tenanttypesinservices` VALUES ('81', '100011', 'Favorites');
INSERT INTO `tn_tenanttypesinservices` VALUES ('82', '000071', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('83', '000101', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('84', '000061', 'Attachment');
INSERT INTO `tn_tenanttypesinservices` VALUES ('85', '000051', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('86', '100002', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('87', '000001', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('88', '000041', 'Count');
INSERT INTO `tn_tenanttypesinservices` VALUES ('89', '100011', 'Recommend');
INSERT INTO `tn_tenanttypesinservices` VALUES ('90', '100013', 'Recommend');
INSERT INTO `tn_tenanttypesinservices` VALUES ('91', '100014', 'Recommend');
INSERT INTO `tn_tenanttypesinservices` VALUES ('92', '100015', 'Recommend');
INSERT INTO `tn_tenanttypesinservices` VALUES ('106', '100003', 'Category');
INSERT INTO `tn_tenanttypesinservices` VALUES ('107', '100002', 'Category');
INSERT INTO `tn_tenanttypesinservices` VALUES ('108', '100003', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('110', '100002', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('111', '100011', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('112', '100012', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('113', '000111', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('114', '000031', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('115', '000021', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('116', '000001', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('117', '000002', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('119', '000131', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('120', '000121', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('121', '000071', 'Category');
INSERT INTO `tn_tenanttypesinservices` VALUES ('122', '000041', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('123', '000071', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('124', '000081', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('125', '000082', 'OperationLog');
INSERT INTO `tn_tenanttypesinservices` VALUES ('126', '000082', 'Attachment');

COMMIT;