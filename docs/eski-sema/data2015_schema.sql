--
-- Table structure for table `birim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `birim` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `bolge`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `bolge` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `bolum`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `bolum` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_bolum`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_bolum` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(20) DEFAULT NULL,
  `ad` varchar(200) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_brans`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_brans` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_ders`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_ders` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_dersane`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_dersane` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  `adres` longtext,
  `tel` varchar(60) DEFAULT NULL,
  `fax` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_giris`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_giris` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_grup`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_grup` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(200) DEFAULT '0',
  `pztg` time DEFAULT NULL,
  `pztc` time DEFAULT NULL,
  `salg` time DEFAULT NULL,
  `salc` time DEFAULT NULL,
  `carg` time DEFAULT NULL,
  `carc` time DEFAULT NULL,
  `perg` time DEFAULT NULL,
  `perc` time DEFAULT NULL,
  `cumg` time DEFAULT NULL,
  `cumc` time DEFAULT NULL,
  `cmtg` time DEFAULT NULL,
  `cmtc` time DEFAULT NULL,
  `pazg` time DEFAULT NULL,
  `pazc` time DEFAULT NULL,
  `g1` tinyint(1) DEFAULT '0',
  `g2` tinyint(1) DEFAULT '0',
  `g3` tinyint(1) DEFAULT '0',
  `g4` tinyint(1) DEFAULT '0',
  `g5` tinyint(1) DEFAULT '0',
  `g6` tinyint(1) DEFAULT '0',
  `g7` tinyint(1) DEFAULT '0',
  `aciklama` varchar(100) DEFAULT NULL,
  `gelmedigindesmsgonder` tinyint(1) DEFAULT '1',
  `girisyaptigindasmsgonder` tinyint(1) DEFAULT '0',
  `cikisyaptigindasmsgonder` tinyint(1) DEFAULT '0',
  `gecgiristesmsgonder` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_sinavnotlari`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_sinavnotlari` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `sinavadi` varchar(150) DEFAULT NULL,
  `sayisal` int(11) DEFAULT NULL,
  `sozel` int(11) DEFAULT NULL,
  `esitagirlik` int(11) DEFAULT NULL,
  `yabancidil` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_sinif`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_sinif` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(20) DEFAULT NULL,
  `ad` varchar(200) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `der_tempgc`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `der_tempgc` (
  `kartno` varchar(5) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  `soyad` varchar(60) DEFAULT NULL,
  `ogrenci_veliad` varchar(60) DEFAULT NULL,
  `ogrenci_veliceptel` varchar(60) DEFAULT NULL,
  `ceptel` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `dil`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `dil` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `egitim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `egitim` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `evrak`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `evrak` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `firma`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `firma` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  `adres` longtext,
  `tel` varchar(60) DEFAULT NULL,
  `fax` varchar(60) DEFAULT NULL,
  `vergidaire` varchar(300) DEFAULT NULL,
  `vergino` varchar(60) DEFAULT NULL,
  `sgk_no` varchar(60) DEFAULT NULL,
  `il` varchar(60) DEFAULT NULL,
  `ilce` varchar(60) DEFAULT NULL,
  `yetkili` varchar(100) DEFAULT NULL,
  `yetkiliceptel` varchar(20) DEFAULT NULL,
  `secim` tinyint(1) DEFAULT '0',
  `kontrolno` varchar(20) DEFAULT '0',
  `aracino` varchar(20) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `gc_giriscikis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `gc_giriscikis` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `toplamsure` varchar(5) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  `surefarki` varchar(6) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT '0',
  `kapino` int(11) DEFAULT NULL,
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  `giristerminal` varchar(60) DEFAULT NULL,
  `cikisterminal` varchar(60) DEFAULT NULL,
  `kapiterminalkod` varchar(10) DEFAULT NULL,
  `girisyetkiliid` bigint(20) DEFAULT NULL,
  `cikisyetkiliid` bigint(20) DEFAULT NULL,
  `kapiterminalid` int(11) DEFAULT NULL,
  `girissaati1` time DEFAULT NULL,
  `cikissaati1` time DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`,`cikistarihi`,`cikissaati`),
  KEY `idx_gc_tarih_personel` (`giristarihi`,`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `gc_giriscikis1`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `gc_giriscikis1` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `toplamsure` varchar(5) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  `surefarki` varchar(6) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT '0',
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  `giristerminal` varchar(60) DEFAULT NULL,
  `cikisterminal` varchar(60) DEFAULT NULL,
  `kapino` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`,`cikistarihi`,`cikissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `gc_giriscikis2`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `gc_giriscikis2` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `toplamsure` varchar(5) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  `surefarki` varchar(6) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT '0',
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  `giristerminal` varchar(60) DEFAULT NULL,
  `cikisterminal` varchar(60) DEFAULT NULL,
  `kapino` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`,`cikistarihi`,`cikissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `gecisgrup`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `gecisgrup` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `gecisyetkisi` varchar(20) DEFAULT NULL,
  `surekisitlama` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `gecisgrupalt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `gecisgrupalt` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `gecisgrupid` bigint(20) unsigned NOT NULL,
  `zamanbolgeid` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `gorev`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `gorev` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `grup`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `grup` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `gunluksaat` varchar(6) DEFAULT NULL,
  `donemliksaat` varchar(6) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  `secim` tinyint(1) DEFAULT NULL,
  `mesaidonemliksaat` varchar(6) DEFAULT NULL,
  `donemliksaat1` varchar(6) DEFAULT NULL,
  `htkkullan` tinyint(1) DEFAULT '0',
  `hitkullan` tinyint(1) DEFAULT '0',
  `haftatatilibordroid` int(11) DEFAULT NULL,
  `haftatailibordroid` int(11) DEFAULT NULL,
  `donemhesaptipi` varchar(10) DEFAULT NULL,
  `ucrethesaptipi` varchar(10) DEFAULT NULL,
  `cmtaktarma` tinyint(1) DEFAULT '0',
  `pazaktarma` tinyint(1) DEFAULT '0',
  `httamamla` tinyint(1) DEFAULT '0' COMMENT 'perakende sektoru icin gelmedigi gunlere hafta tatili ver',
  `calisma4izin2` tinyint(1) DEFAULT '0' COMMENT '4 gun calisip 2 gun izin kullananlar',
  `donemtamamlasonramesaiver` tinyint(1) DEFAULT '0' COMMENT 'donemdeki nc yi tamamlar fazlasını kod=2ye mesai verir',
  `terminalgrupkodu` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `il`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `il` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` int(11) DEFAULT NULL,
  `ad` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `ilce`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `ilce` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ilkod` int(11) DEFAULT NULL,
  `kod` int(11) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `kapiterminal`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `kapiterminal` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(100) DEFAULT '0',
  `tip` varchar(10) DEFAULT NULL,
  `kapino` int(11) DEFAULT NULL,
  `terminalno` varchar(3) DEFAULT NULL,
  `genelmi` tinyint(1) DEFAULT '0',
  `girisip` varchar(30) DEFAULT NULL,
  `cikisip` varchar(30) DEFAULT NULL,
  `girisport` int(11) DEFAULT NULL,
  `cikisport` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `kapiterminal_yetkisaatleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `kapiterminal_yetkisaatleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `kapiterminalkod` int(11) NOT NULL,
  `baslamasaati` time DEFAULT NULL,
  `bitissaati` time DEFAULT NULL,
  `gunlukgecis` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `kapiterminalalt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `kapiterminalalt` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(100) DEFAULT '0',
  `tip` varchar(10) DEFAULT NULL,
  `kapino` int(11) DEFAULT NULL,
  `terminalno` varchar(3) DEFAULT NULL,
  `kapiterminalid` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `kullanici`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `kullanici` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kullaniciadi` varchar(30) DEFAULT NULL,
  `sifre` varchar(30) DEFAULT NULL,
  `kayittarihi` date DEFAULT NULL,
  `tip` varchar(20) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  `personelid` bigint(20) DEFAULT NULL,
  `yedekalsin` tinyint(1) DEFAULT '1',
  `firmasec` tinyint(1) DEFAULT '1',
  `yedekkonum` tinyint(1) DEFAULT '0' COMMENT '0=local 1=server',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `kullanicifirmalar`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `kullanicifirmalar` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kullaniciid` int(11) DEFAULT NULL,
  `yetki` tinyint(1) DEFAULT '0',
  `firmaid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `kullaniciyetkileri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `kullaniciyetkileri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kullaniciid` int(11) DEFAULT NULL,
  `yetki` tinyint(1) DEFAULT '0',
  `aciklama` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailadres`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailadres` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `isim` varchar(100) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `mailsablonid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailayarlari`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailayarlari` (
  `kullanici` varchar(50) DEFAULT NULL,
  `sifre` varchar(30) DEFAULT NULL,
  `host` varchar(50) DEFAULT NULL,
  `port` int(11) DEFAULT NULL,
  `logintipi` varchar(5) DEFAULT NULL,
  `sslmod` varchar(20) DEFAULT NULL,
  `geckalma_mesaj` longtext,
  `eksiksure_mesaj` longtext,
  `devamsizlik_mesaj` longtext,
  `geckalma_dosya` varchar(250) DEFAULT NULL,
  `eksiksure_dosya` varchar(250) DEFAULT NULL,
  `devamsizlik_dosya` varchar(250) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailbirim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailbirim` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `mailsablonid` int(11) NOT NULL,
  `mailadresid` int(11) NOT NULL,
  `birimid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailbolge`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailbolge` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `mailsablonid` int(11) NOT NULL,
  `mailadresid` int(11) NOT NULL,
  `bolgeid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailbolum`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailbolum` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `mailsablonid` int(11) NOT NULL,
  `mailadresid` int(11) NOT NULL,
  `bolumid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailfirma`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailfirma` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `mailsablonid` int(11) NOT NULL,
  `mailadresid` int(11) NOT NULL,
  `firmaid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailgorev`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailgorev` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `mailsablonid` int(11) NOT NULL,
  `mailadresid` int(11) NOT NULL,
  `gorevid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailsaatleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailsaatleri` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `saat` time DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailsablon`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailsablon` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(60) DEFAULT '0',
  `gonderileceksaat` varchar(6) DEFAULT NULL,
  `tarihbas` int(11) DEFAULT '0',
  `tarihbit` int(11) DEFAULT '0',
  `gunluk` tinyint(1) DEFAULT '0',
  `haftalik` tinyint(1) DEFAULT '0',
  `aylik` tinyint(1) DEFAULT '0',
  `alan` varchar(30) DEFAULT NULL,
  `rc` int(11) DEFAULT '0',
  `geckalanlaramailgonder` tinyint(1) DEFAULT '0',
  `dogumgunuolanlar` tinyint(1) DEFAULT '0',
  `puantajtur` tinyint(1) DEFAULT '0' COMMENT '0 ise gunluk 1 ise aylik',
  `ayinkaci` smallint(6) DEFAULT NULL,
  `devamsizlaramailgonder` tinyint(1) DEFAULT '0',
  `eksikcalisanlaramailgonder` tinyint(1) DEFAULT '0',
  `aylikcizelge` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `mailservis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `mailservis` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `mailsablonid` int(11) NOT NULL,
  `mailadresid` int(11) NOT NULL,
  `servisid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `meslek`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `meslek` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(200) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `parakupuru`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `parakupuru` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kupur` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `kartfc` varchar(10) NOT NULL DEFAULT '',
  `kartno` varchar(10) NOT NULL DEFAULT '',
  `kartid` varchar(10) NOT NULL DEFAULT '',
  `ad` varchar(60) DEFAULT NULL,
  `soyad` varchar(60) DEFAULT NULL,
  `sicilno` varchar(30) DEFAULT NULL,
  `durum` varchar(10) DEFAULT NULL,
  `firmaid` int(11) DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `bolumid` int(11) DEFAULT NULL,
  `gorevid` int(11) DEFAULT NULL,
  `servisid` int(11) DEFAULT NULL,
  `bolgeid` int(11) DEFAULT NULL,
  `birimid` int(11) DEFAULT NULL,
  `takimid` int(11) DEFAULT NULL,
  `subeid` int(11) DEFAULT NULL,
  `isegiristarihi` date DEFAULT NULL,
  `istencikistarihi` date DEFAULT NULL,
  `istenayrilmanedeni` longtext,
  `ucret` decimal(11,2) DEFAULT NULL,
  `saatucreti` decimal(11,2) DEFAULT NULL,
  `ucrettipi` varchar(20) DEFAULT NULL,
  `ucret_onceki` decimal(11,2) DEFAULT NULL COMMENT 'ucret\'e zam yaparsa ilk ucret degerini tutar',
  `primucrettipi` varchar(10) DEFAULT NULL,
  `primucreti` decimal(11,2) DEFAULT NULL COMMENT 'gunluk prim ucreti',
  `yolucrettipi` char(10) DEFAULT NULL,
  `yolucreti` decimal(11,2) DEFAULT NULL COMMENT 'gunluk mu aylık mı',
  `yemekucrettipi` varchar(10) DEFAULT NULL,
  `yemekucreti` decimal(11,2) DEFAULT NULL COMMENT 'gunluk mu aylık mı',
  `resim` longblob,
  `webadres` varchar(150) DEFAULT NULL,
  `vergino` varchar(50) DEFAULT NULL,
  `evtel` varchar(100) DEFAULT NULL,
  `ceptel` varchar(100) DEFAULT NULL,
  `istel` varchar(20) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `egitim` varchar(30) DEFAULT NULL,
  `yabancidil` varchar(30) DEFAULT NULL,
  `askerlikdurumu` varchar(10) DEFAULT NULL,
  `gozluk` varchar(10) DEFAULT NULL,
  `sabika` varchar(10) DEFAULT NULL,
  `kilo` decimal(11,2) DEFAULT NULL,
  `boy` decimal(11,2) DEFAULT NULL,
  `ayak` decimal(11,2) DEFAULT NULL,
  `pan` decimal(11,2) DEFAULT NULL,
  `medenihali` varchar(10) DEFAULT NULL,
  `cocuksayisi` smallint(6) DEFAULT NULL,
  `esdurumu` varchar(15) DEFAULT NULL,
  `adres` longtext,
  `il` varchar(40) DEFAULT NULL,
  `ilce` varchar(60) DEFAULT NULL,
  `ilksoyad` varchar(60) DEFAULT NULL,
  `kimlikno` varchar(30) DEFAULT NULL,
  `babaad` varchar(30) DEFAULT NULL,
  `annead` varchar(30) DEFAULT NULL,
  `dogumyeri` varchar(50) DEFAULT NULL,
  `dogumtarihi` date DEFAULT NULL,
  `kangrubu` varchar(10) DEFAULT NULL,
  `uyruk` varchar(30) DEFAULT NULL,
  `cinsiyet` varchar(5) DEFAULT NULL,
  `nufus_il` varchar(30) DEFAULT NULL,
  `nufus_ilce` varchar(30) DEFAULT NULL,
  `nufus_mahkoy` varchar(30) DEFAULT NULL,
  `nufus_ciltno` varchar(10) DEFAULT NULL,
  `nufus_ailesirano` varchar(10) DEFAULT NULL,
  `nufus_sirano` varchar(10) DEFAULT NULL,
  `banka_subeadi` varchar(200) DEFAULT NULL,
  `banka_hesapno` varchar(30) DEFAULT NULL,
  `ehliyet_sinif` varchar(2) DEFAULT NULL,
  `ehliyet_verililce` varchar(60) DEFAULT NULL,
  `ehliyet_belgeno` varchar(30) DEFAULT NULL,
  `ehliyet_vertarih` date DEFAULT NULL,
  `ehliyet_kulcihazprotez` varchar(100) DEFAULT NULL,
  `personelturu` varchar(10) DEFAULT NULL,
  `aciklama` longtext,
  `gunlukkontor` int(11) DEFAULT NULL,
  `uyedurumu` varchar(20) DEFAULT NULL,
  `uyebaslamatarihi` date DEFAULT NULL,
  `uyebitistarihi` date DEFAULT NULL,
  `odedigitutar` decimal(11,2) DEFAULT NULL,
  `kalantutar` decimal(11,2) DEFAULT NULL,
  `uyebaslamasaati` time DEFAULT NULL,
  `uyebitissaati` time DEFAULT NULL,
  `ogrenci_kayittarihi` date DEFAULT NULL,
  `ogrenci_cikistarihi` date DEFAULT NULL,
  `ogrenci_basariorani` int(11) DEFAULT NULL,
  `ogrenci_grupid` int(11) DEFAULT NULL,
  `ogrenci_bolumid` int(11) DEFAULT NULL,
  `ogrenci_sinifid` int(11) DEFAULT NULL,
  `ogretmen_bransid` varchar(60) DEFAULT NULL,
  `ogretmen_dersid` varchar(100) DEFAULT NULL,
  `ogrenci_alanturu` varchar(20) DEFAULT NULL,
  `ogrenci_geldigiokul` varchar(150) DEFAULT NULL,
  `ogrenci_sinif` varchar(20) DEFAULT NULL,
  `ogrenci_bolum` varchar(50) DEFAULT NULL,
  `ogrenci_okulno` varchar(20) DEFAULT NULL,
  `ogrenci_veliad` varchar(60) DEFAULT NULL,
  `ogrenci_velievtel` varchar(20) DEFAULT NULL,
  `ogrenci_veliistel` varchar(20) DEFAULT NULL,
  `ogrenci_veliceptel` varchar(20) DEFAULT NULL,
  `mesaisaatucreti` decimal(11,2) DEFAULT NULL,
  `ozelucrettipi` char(10) DEFAULT NULL,
  `ozelucret` decimal(11,2) DEFAULT NULL COMMENT 'gunluk mu aylık mı',
  `yetkiid` int(11) DEFAULT NULL,
  `uyetipiid` int(11) DEFAULT NULL,
  `ilkgiristarihi` date DEFAULT NULL,
  `songiristarihi` date DEFAULT NULL,
  `toplamgirissayisi` int(11) DEFAULT NULL,
  `gecisgrupid` int(11) DEFAULT NULL,
  `gecisyetki` varchar(128) DEFAULT NULL,
  `toplamkontor` int(11) DEFAULT NULL,
  `kalankontor` int(11) DEFAULT NULL,
  `isegiristarihi1` date DEFAULT NULL,
  `ucret1` decimal(11,2) DEFAULT NULL,
  `fazlamesaialir` tinyint(1) DEFAULT '1',
  `devamsizlikcezasi` decimal(11,2) DEFAULT NULL,
  `agialir` tinyint(1) DEFAULT '0',
  `nufus_cuzdanverildigiyer` varchar(60) DEFAULT NULL,
  `nufus_cuzdanverilisnedeni` varchar(160) DEFAULT NULL,
  `nufus_cuzdankayitno` varchar(30) DEFAULT NULL,
  `nufus_cuzdanverilistarihi` date DEFAULT NULL,
  `elbisebedenno` varchar(30) DEFAULT NULL,
  `kapiyetki` int(11) DEFAULT NULL,
  `yillikizinonceki` int(11) DEFAULT NULL,
  `saglikraporu1` date DEFAULT NULL,
  `saglikraporu2` date DEFAULT NULL,
  `saglikraporu3` date DEFAULT NULL,
  `ay` int(11) DEFAULT NULL COMMENT 'dogumtarihi icin ay alani',
  `parmaktanimlandi` tinyint(1) DEFAULT '0',
  `sgk_no` varchar(30) DEFAULT NULL,
  `sgk_isegiristarihi` date DEFAULT NULL,
  `sgk_istencikiskodu` varchar(10) DEFAULT NULL,
  `sgk_meslekkodu` varchar(20) DEFAULT NULL,
  `sgk_calismadonemi` varchar(20) DEFAULT NULL,
  `sgk_kapsam` varchar(10) DEFAULT NULL,
  `sgk_ucrettipi` varchar(10) DEFAULT NULL,
  `sgk_hesaptipi` varchar(20) DEFAULT NULL,
  `sgk_issizliksigortakes` tinyint(1) DEFAULT '0',
  `sgk_grubu` varchar(10) DEFAULT 'Normal',
  `sgk_belgeturu` varchar(10) DEFAULT '01',
  `sgk_tabioldugukanun` varchar(10) DEFAULT '00000',
  `sgk_kanunbitistarihi` date DEFAULT NULL,
  `sgk_ozurlulukderecesi` smallint(6) DEFAULT '0',
  `ilkisegiristarihi` date DEFAULT NULL,
  `brutucret` decimal(11,2) DEFAULT NULL,
  `sgk_devredenkgvm` decimal(11,5) DEFAULT NULL,
  `wckesintisiuygula` tinyint(1) DEFAULT '1',
  `bakiyekontrol` tinyint(1) DEFAULT '0',
  `sendikadurumu` varchar(30) DEFAULT NULL,
  `meslekid` int(11) DEFAULT NULL COMMENT 'dogumtarihi icin ay alani',
  `sgk_devredenkgvmyil` int(11) DEFAULT NULL,
  `agicocukparasialsin` tinyint(1) DEFAULT '1',
  `terfi1id` int(11) DEFAULT NULL,
  `terfi2id` int(11) DEFAULT NULL,
  `terfi3id` int(11) DEFAULT NULL,
  `terfi4id` int(11) DEFAULT NULL,
  `terfi5id` int(11) DEFAULT NULL,
  `terfi1tarih` date DEFAULT NULL,
  `terfi2tarih` date DEFAULT NULL,
  `terfi3tarih` date DEFAULT NULL,
  `terfi4tarih` date DEFAULT NULL,
  `terfi5tarih` date DEFAULT NULL,
  `ncsaatucreti` decimal(11,2) DEFAULT NULL,
  `tz1` smallint(6) DEFAULT NULL,
  `tz2` smallint(6) DEFAULT NULL,
  `tz3` smallint(6) DEFAULT NULL,
  `tz4` smallint(6) DEFAULT NULL,
  `tz5` smallint(6) DEFAULT NULL,
  `gelmedigindesmsgonder` tinyint(1) DEFAULT '1',
  `girisyaptigindasmsgonder` tinyint(1) DEFAULT '0',
  `cikisyaptigindasmsgonder` tinyint(1) DEFAULT '0',
  `gecgiristesmsgonder` tinyint(1) DEFAULT '0',
  `pztg` time DEFAULT NULL,
  `pztc` time DEFAULT NULL,
  `salg` time DEFAULT NULL,
  `salc` time DEFAULT NULL,
  `carg` time DEFAULT NULL,
  `carc` time DEFAULT NULL,
  `perg` time DEFAULT NULL,
  `perc` time DEFAULT NULL,
  `cumg` time DEFAULT NULL,
  `cumc` time DEFAULT NULL,
  `cmtg` time DEFAULT NULL,
  `cmtc` time DEFAULT NULL,
  `pazg` time DEFAULT NULL,
  `pazc` time DEFAULT NULL,
  `g1` tinyint(1) DEFAULT '0',
  `g2` tinyint(1) DEFAULT '0',
  `g3` tinyint(1) DEFAULT '0',
  `g4` tinyint(1) DEFAULT '0',
  `g5` tinyint(1) DEFAULT '0',
  `g6` tinyint(1) DEFAULT '0',
  `g7` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `kartno` (`kartno`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_ailefertleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_ailefertleri` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `yakinlikderecesi` varchar(10) DEFAULT NULL,
  `sgk_yakinlikderecesi` tinyint(1) DEFAULT '0' COMMENT '0=cocuk 1=es',
  `ad` varchar(100) DEFAULT NULL,
  `soyad` varchar(100) DEFAULT NULL,
  `cinsiyet` varchar(10) DEFAULT NULL,
  `dogumtarihi` date DEFAULT NULL,
  `kimlikno` varchar(20) DEFAULT NULL,
  `agiyetabi` tinyint(1) DEFAULT '0',
  `ucretlimi` tinyint(1) DEFAULT '0',
  `ogrenimdurumu` tinyint(1) DEFAULT '0',
  `kacincicocugu` int(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_dosya`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_dosya` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(11) NOT NULL,
  `ad` varchar(200) DEFAULT NULL,
  `konum` varchar(600) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_egitim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_egitim` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `egitimid` int(11) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `aciklama` varchar(500) DEFAULT NULL,
  `sure` int(11) DEFAULT NULL,
  `gecerliliksuresi` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_evrak`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_evrak` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(11) NOT NULL,
  `evrakid` int(11) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `konum` varchar(600) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_iskazasi`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_iskazasi` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(11) NOT NULL,
  `tur` varchar(60) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `aciklama` varchar(600) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_ucret`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_ucret` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `ucret` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personel_zimmet`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personel_zimmet` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `zimmetbolumid` int(11) DEFAULT NULL,
  `zimmetkategoriid` int(11) DEFAULT NULL,
  `teslimpersonelid` bigint(20) DEFAULT NULL,
  `aciklama` varchar(400) DEFAULT NULL,
  `verilistarihi` date DEFAULT '2000-01-01',
  `verilissaati` time DEFAULT NULL,
  `iadetarihi` date DEFAULT NULL,
  `iadesaati` time DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `personeldetay`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `personeldetay` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(11) NOT NULL,
  `personelid_yakini` varchar(100) DEFAULT NULL,
  `yakinlikderecesi` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_altgrup`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_altgrup` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `grupid` int(11) NOT NULL,
  `ad` varchar(100) DEFAULT NULL,
  `gundonumu` varchar(5) DEFAULT NULL,
  `gunbitisi` varchar(5) DEFAULT NULL,
  `gunluksaat` varchar(5) DEFAULT NULL,
  `birsonrakigunesay` tinyint(1) DEFAULT '0',
  `gunluksaat1` varchar(6) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `grupid` (`grupid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_araplan`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_araplan` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `bastarih` date DEFAULT NULL,
  `bittarih` date DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `plangrupid` int(11) DEFAULT NULL,
  `plangrupad` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_araplanper`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_araplanper` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `araplanid` bigint(20) DEFAULT NULL,
  `personelid` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `araplanid` (`araplanid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_calismaparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_calismaparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `puantajid` int(11) NOT NULL,
  `bordroid` int(11) NOT NULL,
  `bordroad` varchar(100) DEFAULT NULL,
  `igs` varchar(5) DEFAULT NULL,
  `dcs` varchar(5) DEFAULT NULL,
  `ggt` varchar(5) DEFAULT NULL,
  `ect` varchar(5) DEFAULT NULL,
  `minimum` varchar(5) DEFAULT NULL,
  `maximum` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `puantajid` (`puantajid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_calismaturleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_calismaturleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` smallint(6) DEFAULT NULL,
  `ad` varchar(10) DEFAULT NULL,
  `tip` varchar(20) DEFAULT NULL,
  `katsayi` int(10) DEFAULT NULL,
  `aciklama` varchar(100) DEFAULT NULL,
  `hesaptipi` varchar(10) DEFAULT NULL,
  `aktarimkod` smallint(6) DEFAULT NULL,
  `gunluksaat` varchar(5) DEFAULT NULL,
  `kod2` smallint(6) DEFAULT NULL,
  `sgk` varchar(6) DEFAULT 'var',
  `gv` varchar(6) DEFAULT 'var',
  `dv` varchar(6) DEFAULT 'var',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_cezaparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_cezaparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `puantajid` int(11) NOT NULL,
  `tip` varchar(3) DEFAULT NULL,
  `tolerans` int(11) DEFAULT NULL,
  `kesinti` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `puantajid` (`puantajid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_cezaparametre2`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_cezaparametre2` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `puantajid` int(11) NOT NULL,
  `geckatsayi` decimal(11,2) DEFAULT NULL,
  `erkkatsayi` decimal(11,2) DEFAULT NULL,
  `ekskatsayi` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `puantajid` (`puantajid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_digerparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_digerparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `puantajid` int(11) NOT NULL,
  `devamsizlikceza_saat` varchar(5) DEFAULT NULL,
  `nctamamlasonramesaiver` tinyint(1) DEFAULT '0',
  `eksiksureyoksay` tinyint(1) DEFAULT '0',
  `dvmkesinti_bordroid` int(11) DEFAULT NULL COMMENT 'devamsizlik',
  `geckesinti_bordroid` int(11) DEFAULT NULL COMMENT 'gec kalma',
  `erkkesinti_bordroid` int(11) DEFAULT NULL COMMENT 'erken çıkma',
  `ekskesinti_bordroid` int(11) DEFAULT NULL COMMENT 'eksik süre',
  `gecErkEksCezaKullan` tinyint(1) DEFAULT '0',
  `gunasiri` tinyint(1) DEFAULT '0',
  `birsonrakigunesay` tinyint(1) DEFAULT '0',
  `maxtolerans` varchar(10) DEFAULT NULL COMMENT 'nctamamlasonramesaiver icin',
  `tolerans_saat` varchar(10) DEFAULT NULL,
  `tolerans_fmid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `puantajid` (`puantajid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_donemparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_donemparametre` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `grupid` bigint(20) NOT NULL,
  `bastarih` date DEFAULT NULL,
  `bittarih` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `eksikgun` smallint(6) DEFAULT NULL,
  `eksiksaat` varchar(6) DEFAULT NULL,
  `fazlagun` smallint(6) DEFAULT NULL,
  `fazlasaat` varchar(6) DEFAULT NULL,
  `yil` int(11) DEFAULT NULL,
  `gunfarki` smallint(6) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  `altgrupid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `grupid` (`grupid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_extra`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_extra` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `tutar` decimal(11,2) DEFAULT NULL,
  `extraturid` int(11) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_extra1`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_extra1` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `tutar` decimal(11,2) DEFAULT NULL,
  `extraturid` int(11) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_extraturleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_extraturleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` smallint(6) DEFAULT NULL,
  `ad` varchar(10) DEFAULT NULL,
  `aciklama` varchar(100) DEFAULT NULL,
  `tip` varchar(15) DEFAULT NULL,
  `aktarimkod` smallint(6) DEFAULT NULL,
  `kod2` smallint(6) DEFAULT NULL,
  `sgk` varchar(6) DEFAULT 'var',
  `gv` varchar(6) DEFAULT 'var',
  `dv` varchar(6) DEFAULT 'var',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_gcyuvarlamaparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_gcyuvarlamaparametre` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `baslamasaati` varchar(6) DEFAULT NULL,
  `bitissaati` varchar(6) DEFAULT NULL,
  `yuvarlamasaati` varchar(6) DEFAULT NULL,
  `tip` varchar(10) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  `yuvarlamatolerans` int(11) DEFAULT NULL,
  `yuvarlamagun` varchar(15) DEFAULT NULL,
  `grupid` int(11) NOT NULL,
  `aktarilsinmi` tinyint(1) DEFAULT '1',
  `gecebassaat` varchar(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_genelayarlar`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_genelayarlar` (
  `agigoster` tinyint(1) DEFAULT '0',
  `yolgoster` tinyint(1) DEFAULT '0',
  `primgoster` tinyint(1) DEFAULT '0',
  `yemekgoster` tinyint(1) DEFAULT '0',
  `wcgoster` tinyint(1) DEFAULT '0',
  `agibruttutar` decimal(11,2) DEFAULT NULL,
  `yuvarlamasaat` int(11) DEFAULT NULL,
  `sgkkullan` tinyint(1) DEFAULT '0',
  `devirkullan` tinyint(1) DEFAULT '0' COMMENT 'odeme islemi kullanılıyorsa, gecen aydan kalan maasi bir sonraki aya devretsin',
  `cikistayedekal` tinyint(1) DEFAULT '0',
  `htkkullan` tinyint(1) DEFAULT '0',
  `hitkullan` tinyint(1) DEFAULT '0',
  `printerindex` smallint(6) DEFAULT NULL,
  `donembaslamatarihi` date DEFAULT NULL,
  `donembitistarihi` date DEFAULT NULL,
  `firmaayri` tinyint(1) DEFAULT '1',
  `gcsaatyuvarla` tinyint(1) DEFAULT '0',
  `appTask` varchar(10) DEFAULT NULL,
  `aktarimpath` longtext,
  `agitipi` tinyint(1) DEFAULT '0',
  `eksikcalisma` tinyint(1) DEFAULT '0',
  `donemfarkigoster` tinyint(1) DEFAULT '0',
  `ucretayri` tinyint(1) DEFAULT '0',
  `data2` varchar(30) DEFAULT NULL,
  `zeminrengi` int(11) DEFAULT NULL,
  `ozelucretgoster` tinyint(1) DEFAULT '0',
  `gunlukgecissayisi` tinyint(1) DEFAULT '0',
  `birimucret` decimal(11,2) DEFAULT NULL,
  `uyeprogrammodu` tinyint(1) DEFAULT '0',
  `saatyuvarlamatipi` tinyint(1) DEFAULT '0',
  `ucretyuvarlamatipi` tinyint(1) DEFAULT '0',
  `saatyon` varchar(1) DEFAULT NULL,
  `ucretyon` varchar(1) DEFAULT NULL,
  `yuvarlamaucret` decimal(11,2) DEFAULT NULL,
  `agi_bayanesdurumu` tinyint(1) DEFAULT '0',
  `cezakesinti` tinyint(1) DEFAULT '0',
  `terminalototipi` tinyint(1) DEFAULT '0',
  `otodakika` int(11) DEFAULT NULL,
  `kartnooto` tinyint(1) DEFAULT '0' COMMENT 'yeni kayitta kartno yu otomatik arttirmak icin',
  `puantajkitle` int(11) DEFAULT NULL,
  `maxmesaisaat` varchar(10) DEFAULT NULL COMMENT 'resmide max mesai saati icin',
  `eksikcalismasubat` tinyint(1) DEFAULT '0' COMMENT 'Subat ayı icin',
  `odemekullan` tinyint(1) DEFAULT '0' COMMENT 'pts_odeme iptal olur kazanckesinti olarak odeme yapmak icin',
  `kisiselbordro` tinyint(1) DEFAULT '0' COMMENT 'dizayn ekrani olan ayrintili kisisel bordroyu aktif yapar',
  `donemaltgrupkullan` tinyint(1) DEFAULT '0' COMMENT 'vardiyali gruplarda altgruplar icin',
  `gclistekapiterminalgoster` tinyint(1) DEFAULT '0',
  `brutucretkullan` tinyint(1) DEFAULT '0',
  `sgkisegiristarihikullan` tinyint(1) DEFAULT '0',
  `yillikmaxmesaisaati` varchar(6) DEFAULT NULL,
  `ncyeekle` tinyint(1) DEFAULT '0',
  `ncgunal` tinyint(1) DEFAULT '0',
  `mesaigoster` tinyint(1) DEFAULT '0',
  `egitimgoster` tinyint(1) DEFAULT '0',
  `dosyagoster` tinyint(1) DEFAULT '0',
  `zimmetgoster` tinyint(1) DEFAULT '0',
  `yemekhane1` tinyint(1) DEFAULT '0',
  `yemekhane2` tinyint(1) DEFAULT '0',
  `giriscikis1` tinyint(1) DEFAULT '0',
  `giriscikis2` tinyint(1) DEFAULT '0',
  `gctablo` varchar(50) DEFAULT 'gc_giriscikis',
  `evrak` tinyint(1) DEFAULT '0',
  `calismamodu` tinyint(1) DEFAULT '0',
  `iskazasi` tinyint(1) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_giriscikis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_giriscikis` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `toplamsure` varchar(5) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `izintipi` varchar(10) DEFAULT NULL,
  `izinturid` int(11) DEFAULT NULL,
  `izinsure` varchar(6) DEFAULT NULL,
  `aciklama` varchar(250) DEFAULT NULL,
  `geciptal` tinyint(1) DEFAULT '0',
  `erkiptal` tinyint(1) DEFAULT '0',
  `eksiptal` tinyint(1) DEFAULT '0',
  `mesaiiptal` tinyint(1) DEFAULT '0',
  `kapiterminalid` int(11) DEFAULT NULL,
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  `kapiterminalkod` varchar(10) DEFAULT NULL,
  `surefarki` varchar(6) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT '0',
  `iizinsure` int(11) DEFAULT '0',
  `kartno` varchar(10) DEFAULT NULL,
  `girissaati1` time DEFAULT NULL,
  `cikissaati1` time DEFAULT NULL,
  `girisresim` longblob,
  `cikisresim` longblob,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`,`cikistarihi`,`cikissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_giriscikis1`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_giriscikis1` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `toplamsure` varchar(5) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `izintipi` varchar(10) DEFAULT NULL,
  `izinturid` int(11) DEFAULT NULL,
  `izinsure` varchar(6) DEFAULT NULL,
  `aciklama` varchar(250) DEFAULT NULL,
  `geciptal` tinyint(1) DEFAULT '0',
  `erkiptal` tinyint(1) DEFAULT '0',
  `eksiptal` int(11) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`,`cikistarihi`,`cikissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_gruphit`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_gruphit` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `grupid` int(11) NOT NULL,
  `tip` tinyint(1) DEFAULT NULL,
  `hi1` tinyint(1) DEFAULT NULL,
  `hi2` tinyint(1) DEFAULT NULL,
  `hi3` tinyint(1) DEFAULT NULL,
  `hi4` tinyint(1) DEFAULT NULL,
  `hi5` tinyint(1) DEFAULT NULL,
  `hi6` tinyint(1) DEFAULT NULL,
  `hi7` tinyint(1) DEFAULT NULL,
  `hi8` tinyint(1) DEFAULT NULL,
  `hs1` tinyint(1) DEFAULT NULL,
  `hs2` tinyint(1) DEFAULT NULL,
  `hs3` tinyint(1) DEFAULT NULL,
  `hs4` tinyint(1) DEFAULT NULL,
  `hs5` tinyint(1) DEFAULT NULL,
  `hs6` tinyint(1) DEFAULT NULL,
  `hs7` tinyint(1) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_gruphtk`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_gruphtk` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `grupid` int(11) NOT NULL,
  `tip` tinyint(1) DEFAULT NULL,
  `hi1` tinyint(1) DEFAULT NULL,
  `hi2` tinyint(1) DEFAULT NULL,
  `hi3` tinyint(1) DEFAULT NULL,
  `hi4` tinyint(1) DEFAULT NULL,
  `hi5` tinyint(1) DEFAULT NULL,
  `hi6` tinyint(1) DEFAULT NULL,
  `hi7` tinyint(1) DEFAULT NULL,
  `hi8` tinyint(1) DEFAULT NULL,
  `hs1` tinyint(1) DEFAULT NULL,
  `hs2` tinyint(1) DEFAULT NULL,
  `hs3` tinyint(1) DEFAULT NULL,
  `hs4` tinyint(1) DEFAULT NULL,
  `hs5` tinyint(1) DEFAULT NULL,
  `hs6` tinyint(1) DEFAULT NULL,
  `hs7` tinyint(1) DEFAULT NULL,
  `hesaplamatipi` varchar(10) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_gunparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_gunparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `grupid` int(11) DEFAULT NULL,
  `altgrupid` int(11) NOT NULL,
  `g1` int(11) DEFAULT NULL,
  `g2` int(11) DEFAULT NULL,
  `g3` int(11) DEFAULT NULL,
  `g4` int(11) DEFAULT NULL,
  `g5` int(11) DEFAULT NULL,
  `g6` int(11) DEFAULT NULL,
  `g7` int(11) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `altgrupid` (`altgrupid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
;

--
-- Table structure for table `pts_icmal`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_icmal` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(10) NOT NULL,
  `bastarih` date DEFAULT NULL,
  `bittarih` date DEFAULT NULL,
  `devamsizlikgun` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat` varchar(6) DEFAULT NULL,
  `devamsizlikucret` decimal(11,2) DEFAULT NULL,
  `geckalmagun` decimal(11,2) DEFAULT NULL,
  `geckalmasaat` varchar(6) DEFAULT NULL,
  `geckalmaucret` decimal(11,2) DEFAULT NULL,
  `erkencikmagun` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat` varchar(6) DEFAULT NULL,
  `erkencikmaucret` decimal(11,2) DEFAULT NULL,
  `eksikcalismagun` decimal(11,2) DEFAULT NULL,
  `eksikcalismasaat` varchar(6) DEFAULT NULL,
  `eksikcalismaucret` decimal(11,2) DEFAULT NULL,
  `devamsizlikgun_ceza` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat_ceza` varchar(6) DEFAULT NULL,
  `devamsizlikucret_ceza` decimal(11,2) DEFAULT NULL,
  `geckalmagun_ceza` decimal(11,2) DEFAULT NULL,
  `geckalmasaat_ceza` varchar(6) DEFAULT NULL,
  `geckalmaucret_ceza` decimal(11,2) DEFAULT NULL,
  `erkencikmagun_ceza` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat_ceza` varchar(6) DEFAULT NULL,
  `erkencikmaucret_ceza` decimal(11,2) DEFAULT NULL,
  `eksikcalismagun_ceza` decimal(11,2) DEFAULT NULL,
  `eksikcalismasaat_ceza` varchar(6) DEFAULT NULL,
  `eksikcalismaucret_ceza` decimal(11,2) DEFAULT NULL,
  `cgun1` decimal(11,2) DEFAULT NULL,
  `csaat1` varchar(6) DEFAULT NULL,
  `cucret1` decimal(11,2) DEFAULT NULL,
  `cgun2` decimal(11,2) DEFAULT NULL,
  `csaat2` varchar(6) DEFAULT NULL,
  `cucret2` decimal(11,2) DEFAULT NULL,
  `cgun3` decimal(11,2) DEFAULT NULL,
  `csaat3` varchar(6) DEFAULT NULL,
  `cucret3` decimal(11,2) DEFAULT NULL,
  `cgun4` decimal(11,2) DEFAULT NULL,
  `csaat4` varchar(6) DEFAULT NULL,
  `cucret4` decimal(11,2) DEFAULT NULL,
  `cgun5` decimal(11,2) DEFAULT NULL,
  `csaat5` varchar(6) DEFAULT NULL,
  `cucret5` decimal(11,2) DEFAULT NULL,
  `cgun6` decimal(11,2) DEFAULT NULL,
  `csaat6` varchar(6) DEFAULT NULL,
  `cucret6` decimal(11,2) DEFAULT NULL,
  `cgun7` decimal(11,2) DEFAULT NULL,
  `csaat7` varchar(6) DEFAULT NULL,
  `cucret7` decimal(11,2) DEFAULT NULL,
  `cgun8` decimal(11,2) DEFAULT NULL,
  `csaat8` varchar(6) DEFAULT NULL,
  `cucret8` decimal(11,2) DEFAULT NULL,
  `cgun9` decimal(11,2) DEFAULT NULL,
  `csaat9` varchar(6) DEFAULT NULL,
  `cucret9` decimal(11,2) DEFAULT NULL,
  `cgun10` decimal(11,2) DEFAULT NULL,
  `csaat10` varchar(6) DEFAULT NULL,
  `cucret10` decimal(11,2) DEFAULT NULL,
  `cgun11` decimal(11,2) DEFAULT NULL,
  `csaat11` varchar(6) DEFAULT NULL,
  `cucret11` decimal(11,2) DEFAULT NULL,
  `cgun12` decimal(11,2) DEFAULT NULL,
  `csaat12` varchar(6) DEFAULT NULL,
  `cucret12` decimal(11,2) DEFAULT NULL,
  `cgun13` decimal(11,2) DEFAULT NULL,
  `csaat13` varchar(6) DEFAULT NULL,
  `cucret13` decimal(11,2) DEFAULT NULL,
  `cgun14` decimal(11,2) DEFAULT NULL,
  `csaat14` varchar(6) DEFAULT NULL,
  `cucret14` decimal(11,2) DEFAULT NULL,
  `cgun15` decimal(11,2) DEFAULT NULL,
  `csaat15` varchar(6) DEFAULT NULL,
  `cucret15` decimal(11,2) DEFAULT NULL,
  `cgun16` decimal(11,2) DEFAULT NULL,
  `csaat16` varchar(6) DEFAULT NULL,
  `cucret16` decimal(11,2) DEFAULT NULL,
  `cgun17` decimal(11,2) DEFAULT NULL,
  `csaat17` varchar(6) DEFAULT NULL,
  `cucret17` decimal(11,2) DEFAULT NULL,
  `cgun18` decimal(11,2) DEFAULT NULL,
  `csaat18` varchar(6) DEFAULT NULL,
  `cucret18` decimal(11,2) DEFAULT NULL,
  `cgun19` decimal(11,2) DEFAULT NULL,
  `csaat19` varchar(6) DEFAULT NULL,
  `cucret19` decimal(11,2) DEFAULT NULL,
  `cgun20` decimal(11,2) DEFAULT NULL,
  `csaat20` varchar(6) DEFAULT NULL,
  `cucret20` decimal(11,2) DEFAULT NULL,
  `cgun21` decimal(11,2) DEFAULT NULL,
  `csaat21` varchar(6) DEFAULT NULL,
  `cucret21` decimal(11,2) DEFAULT NULL,
  `cgun22` decimal(11,2) DEFAULT NULL,
  `csaat22` varchar(6) DEFAULT NULL,
  `cucret22` decimal(11,2) DEFAULT NULL,
  `cgun23` decimal(11,2) DEFAULT NULL,
  `csaat23` varchar(6) DEFAULT NULL,
  `cucret23` decimal(11,2) DEFAULT NULL,
  `cgun24` decimal(11,2) DEFAULT NULL,
  `csaat24` varchar(6) DEFAULT NULL,
  `cucret24` decimal(11,2) DEFAULT NULL,
  `cgun25` decimal(11,2) DEFAULT NULL,
  `csaat25` varchar(6) DEFAULT NULL,
  `cucret25` decimal(11,2) DEFAULT NULL,
  `cgun26` decimal(11,2) DEFAULT NULL,
  `csaat26` varchar(6) DEFAULT NULL,
  `cucret26` decimal(11,2) DEFAULT NULL,
  `cgun27` decimal(11,2) DEFAULT NULL,
  `csaat27` varchar(6) DEFAULT NULL,
  `cucret27` decimal(11,2) DEFAULT NULL,
  `cgun28` decimal(11,2) DEFAULT NULL,
  `csaat28` varchar(6) DEFAULT NULL,
  `cucret28` decimal(11,2) DEFAULT NULL,
  `cgun29` decimal(11,2) DEFAULT NULL,
  `csaat29` varchar(6) DEFAULT NULL,
  `cucret29` decimal(11,2) DEFAULT NULL,
  `cgun30` decimal(11,2) DEFAULT NULL,
  `csaat30` varchar(6) DEFAULT NULL,
  `cucret30` decimal(11,2) DEFAULT NULL,
  `cgun31` decimal(11,2) DEFAULT NULL,
  `csaat31` varchar(6) DEFAULT NULL,
  `cucret31` decimal(11,2) DEFAULT NULL,
  `cgun32` decimal(11,2) DEFAULT NULL,
  `csaat32` varchar(6) DEFAULT NULL,
  `cucret32` decimal(11,2) DEFAULT NULL,
  `igun1` decimal(11,2) DEFAULT NULL,
  `isaat1` varchar(6) DEFAULT NULL,
  `iucret1` decimal(11,2) DEFAULT NULL,
  `igun2` decimal(11,2) DEFAULT NULL,
  `isaat2` varchar(6) DEFAULT NULL,
  `iucret2` decimal(11,2) DEFAULT NULL,
  `igun3` decimal(11,2) DEFAULT NULL,
  `isaat3` varchar(6) DEFAULT NULL,
  `iucret3` decimal(11,2) DEFAULT NULL,
  `igun4` decimal(11,2) DEFAULT NULL,
  `isaat4` varchar(6) DEFAULT NULL,
  `iucret4` decimal(11,2) DEFAULT NULL,
  `igun5` decimal(11,2) DEFAULT NULL,
  `isaat5` varchar(6) DEFAULT NULL,
  `iucret5` decimal(11,2) DEFAULT NULL,
  `igun6` decimal(11,2) DEFAULT NULL,
  `isaat6` varchar(6) DEFAULT NULL,
  `iucret6` decimal(11,2) DEFAULT NULL,
  `igun7` decimal(11,2) DEFAULT NULL,
  `isaat7` varchar(6) DEFAULT NULL,
  `iucret7` decimal(11,2) DEFAULT NULL,
  `igun8` decimal(11,2) DEFAULT NULL,
  `isaat8` varchar(6) DEFAULT NULL,
  `iucret8` decimal(11,2) DEFAULT NULL,
  `igun9` decimal(11,2) DEFAULT NULL,
  `isaat9` varchar(6) DEFAULT NULL,
  `iucret9` decimal(11,2) DEFAULT NULL,
  `igun10` decimal(11,2) DEFAULT NULL,
  `isaat10` varchar(6) DEFAULT NULL,
  `iucret10` decimal(11,2) DEFAULT NULL,
  `igun11` decimal(11,2) DEFAULT NULL,
  `isaat11` varchar(6) DEFAULT NULL,
  `iucret11` decimal(11,2) DEFAULT NULL,
  `igun12` decimal(11,2) DEFAULT NULL,
  `isaat12` varchar(6) DEFAULT NULL,
  `iucret12` decimal(11,2) DEFAULT NULL,
  `igun13` decimal(11,2) DEFAULT NULL,
  `isaat13` varchar(6) DEFAULT NULL,
  `iucret13` decimal(11,2) DEFAULT NULL,
  `igun14` decimal(11,2) DEFAULT NULL,
  `isaat14` varchar(6) DEFAULT NULL,
  `iucret14` decimal(11,2) DEFAULT NULL,
  `igun15` decimal(11,2) DEFAULT NULL,
  `isaat15` varchar(6) DEFAULT NULL,
  `iucret15` decimal(11,2) DEFAULT NULL,
  `igun16` decimal(11,2) DEFAULT NULL,
  `isaat16` varchar(6) DEFAULT NULL,
  `iucret16` decimal(11,2) DEFAULT NULL,
  `igun17` decimal(11,2) DEFAULT NULL,
  `isaat17` varchar(6) DEFAULT NULL,
  `iucret17` decimal(11,2) DEFAULT NULL,
  `igun18` decimal(11,2) DEFAULT NULL,
  `isaat18` varchar(6) DEFAULT NULL,
  `iucret18` decimal(11,2) DEFAULT NULL,
  `igun19` decimal(11,2) DEFAULT NULL,
  `isaat19` varchar(6) DEFAULT NULL,
  `iucret19` decimal(11,2) DEFAULT NULL,
  `igun20` decimal(11,2) DEFAULT NULL,
  `isaat20` varchar(6) DEFAULT NULL,
  `iucret20` decimal(11,2) DEFAULT NULL,
  `igun21` decimal(11,2) DEFAULT NULL,
  `isaat21` varchar(6) DEFAULT NULL,
  `iucret21` decimal(11,2) DEFAULT NULL,
  `igun22` decimal(11,2) DEFAULT NULL,
  `isaat22` varchar(6) DEFAULT NULL,
  `iucret22` decimal(11,2) DEFAULT NULL,
  `igun23` decimal(11,2) DEFAULT NULL,
  `isaat23` varchar(6) DEFAULT NULL,
  `iucret23` decimal(11,2) DEFAULT NULL,
  `igun24` decimal(11,2) DEFAULT NULL,
  `isaat24` varchar(6) DEFAULT NULL,
  `iucret24` decimal(11,2) DEFAULT NULL,
  `igun25` decimal(11,2) DEFAULT NULL,
  `isaat25` varchar(6) DEFAULT NULL,
  `iucret25` decimal(11,2) DEFAULT NULL,
  `igun26` decimal(11,2) DEFAULT NULL,
  `isaat26` varchar(6) DEFAULT NULL,
  `iucret26` decimal(11,2) DEFAULT NULL,
  `igun27` decimal(11,2) DEFAULT NULL,
  `isaat27` varchar(6) DEFAULT NULL,
  `iucret27` decimal(11,2) DEFAULT NULL,
  `igun28` decimal(11,2) DEFAULT NULL,
  `isaat28` varchar(6) DEFAULT NULL,
  `iucret28` decimal(11,2) DEFAULT NULL,
  `igun29` decimal(11,2) DEFAULT NULL,
  `isaat29` varchar(6) DEFAULT NULL,
  `iucret29` decimal(11,2) DEFAULT NULL,
  `igun30` decimal(11,2) DEFAULT NULL,
  `isaat30` varchar(6) DEFAULT NULL,
  `iucret30` decimal(11,2) DEFAULT NULL,
  `igun31` decimal(11,2) DEFAULT NULL,
  `isaat31` varchar(6) DEFAULT NULL,
  `iucret31` decimal(11,2) DEFAULT NULL,
  `igun32` decimal(11,2) DEFAULT NULL,
  `isaat32` varchar(6) DEFAULT NULL,
  `iucret32` decimal(11,2) DEFAULT NULL,
  `extra1` decimal(11,2) DEFAULT NULL,
  `extra2` decimal(11,2) DEFAULT NULL,
  `extra3` decimal(11,2) DEFAULT NULL,
  `extra4` decimal(11,2) DEFAULT NULL,
  `extra5` decimal(11,2) DEFAULT NULL,
  `extra6` decimal(11,2) DEFAULT NULL,
  `extra7` decimal(11,2) DEFAULT NULL,
  `extra8` decimal(11,2) DEFAULT NULL,
  `extra9` decimal(11,2) DEFAULT NULL,
  `extra10` decimal(11,2) DEFAULT NULL,
  `extra11` decimal(11,2) DEFAULT NULL,
  `extra12` decimal(11,2) DEFAULT NULL,
  `extra13` decimal(11,2) DEFAULT NULL,
  `extra14` decimal(11,2) DEFAULT NULL,
  `extra15` decimal(11,2) DEFAULT NULL,
  `extra16` decimal(11,2) DEFAULT NULL,
  `extra17` decimal(11,2) DEFAULT NULL,
  `extra18` decimal(11,2) DEFAULT NULL,
  `extra19` decimal(11,2) DEFAULT NULL,
  `extra20` decimal(11,2) DEFAULT NULL,
  `extra21` decimal(11,2) DEFAULT NULL,
  `extra22` decimal(11,2) DEFAULT NULL,
  `extra23` decimal(11,2) DEFAULT NULL,
  `extra24` decimal(11,2) DEFAULT NULL,
  `extra25` decimal(11,2) DEFAULT NULL,
  `extra26` decimal(11,2) DEFAULT NULL,
  `extra27` decimal(11,2) DEFAULT NULL,
  `extra28` decimal(11,2) DEFAULT NULL,
  `extra29` decimal(11,2) DEFAULT NULL,
  `extra30` decimal(11,2) DEFAULT NULL,
  `extra31` decimal(11,2) DEFAULT NULL,
  `extra32` decimal(11,2) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_izin`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_izin` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `izintipi` varchar(10) DEFAULT NULL,
  `izinturid` int(11) DEFAULT NULL,
  `izinsure` varchar(6) DEFAULT NULL,
  `iizinsure` int(11) DEFAULT '0',
  `aciklama` varchar(250) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `sgk_eksikgunkodu` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_izinad`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_izinad` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_izinturleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_izinturleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` smallint(6) DEFAULT NULL,
  `ad` varchar(10) DEFAULT NULL,
  `aciklama` varchar(100) DEFAULT NULL,
  `tip` varchar(20) DEFAULT NULL,
  `katsayi` int(11) DEFAULT NULL,
  `aktarimkod` smallint(6) DEFAULT NULL,
  `kod2` smallint(6) DEFAULT NULL,
  `eksikkod` smallint(6) DEFAULT NULL,
  `sgk` varchar(6) DEFAULT 'var',
  `gv` varchar(6) DEFAULT 'var',
  `dv` varchar(6) DEFAULT 'var',
  `izinkod` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_kesintiparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_kesintiparametre` (
  `dvmkesinti_bordroid` int(11) DEFAULT NULL,
  `geckesinti_bordroid` int(11) DEFAULT NULL,
  `erkkesinti_bordroid` int(11) DEFAULT NULL,
  `ekskesinti_bordroid` int(11) DEFAULT NULL,
  `appTask` varchar(10) DEFAULT NULL,
  `uszikesinti_bordroid` int(11) DEFAULT '7',
  `dvmkesinti_bordroid1` int(11) DEFAULT NULL,
  `dvmkesinti_bordroid2` int(11) DEFAULT NULL,
  `dvmkesinti_bordroid3` int(11) DEFAULT NULL,
  `geckesinti_bordroid1` int(11) DEFAULT NULL,
  `geckesinti_bordroid2` int(11) DEFAULT NULL,
  `geckesinti_bordroid3` int(11) DEFAULT NULL,
  `erkkesinti_bordroid1` int(11) DEFAULT NULL,
  `erkkesinti_bordroid2` int(11) DEFAULT NULL,
  `erkkesinti_bordroid3` int(11) DEFAULT NULL,
  `ekskesinti_bordroid1` int(11) DEFAULT NULL,
  `ekskesinti_bordroid2` int(11) DEFAULT NULL,
  `ekskesinti_bordroid3` int(11) DEFAULT NULL,
  `uszikesinti_bordroid1` int(11) DEFAULT NULL,
  `uszikesinti_bordroid2` int(11) DEFAULT NULL,
  `uszikesinti_bordroid3` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_kisiselplan`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_kisiselplan` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `tarih` date DEFAULT NULL,
  `personelid` bigint(20) DEFAULT NULL,
  `puantajid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_kstalanlar`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_kstalanlar` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `alanid` int(11) DEFAULT NULL,
  `ad` varchar(20) DEFAULT NULL,
  `yetki` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_mesai`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_mesai` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `isaat` int(11) DEFAULT '0',
  `calismaturid` int(11) DEFAULT NULL,
  `aciklama` varchar(250) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_mesaiplani`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_mesaiplani` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `mesai` varchar(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_molaparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_molaparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `puantajid` int(11) NOT NULL,
  `ad` varchar(100) DEFAULT NULL,
  `baslamasaati` varchar(5) DEFAULT NULL,
  `bitissaati` varchar(5) DEFAULT NULL,
  `tip` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `puantajid` (`puantajid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_odeme`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_odeme` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `tutar` decimal(11,2) DEFAULT NULL,
  `tip` varchar(10) DEFAULT NULL,
  `aciklama` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_ozelplan`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_ozelplan` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `tarih` date DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `altgrupid` int(11) DEFAULT NULL,
  `puantajid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_perraporsablon`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_perraporsablon` (
  `secim` tinyint(1) DEFAULT '0',
  `ad` varchar(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_puantaj`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_puantaj` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `devamsizlik` varchar(6) DEFAULT NULL,
  `geckalma` varchar(6) DEFAULT NULL,
  `erkencikma` varchar(6) DEFAULT NULL,
  `eksiksure` varchar(6) DEFAULT NULL,
  `devamsizlik_ceza` varchar(6) DEFAULT NULL,
  `geckalma_ceza` varchar(6) DEFAULT NULL,
  `erkencikma_ceza` varchar(6) DEFAULT NULL,
  `eksiksure_ceza` varchar(6) DEFAULT NULL,
  `devamsizlik_net` varchar(6) DEFAULT NULL,
  `geckalma_net` varchar(6) DEFAULT NULL,
  `erkencikma_net` varchar(6) DEFAULT NULL,
  `eksiksure_net` varchar(6) DEFAULT NULL,
  `devamsizlik_dak` int(11) DEFAULT '0',
  `geckalma_dak` int(11) DEFAULT '0',
  `erkencikma_dak` int(11) DEFAULT '0',
  `eksiksure_dak` int(11) DEFAULT '0',
  `devamsizlik_cezadak` int(11) DEFAULT '0',
  `geckalma_cezadak` int(11) DEFAULT '0',
  `erkencikma_cezadak` int(11) DEFAULT '0',
  `eksiksure_cezadak` int(11) DEFAULT '0',
  `devamsizlik_netdak` int(11) DEFAULT '0',
  `geckalma_netdak` int(11) DEFAULT '0',
  `erkencikma_netdak` int(11) DEFAULT '0',
  `eksiksure_netdak` int(11) DEFAULT '0',
  `c1` varchar(6) DEFAULT NULL,
  `c2` varchar(6) DEFAULT NULL,
  `c3` varchar(6) DEFAULT NULL,
  `c4` varchar(6) DEFAULT NULL,
  `c5` varchar(6) DEFAULT NULL,
  `c6` varchar(6) DEFAULT NULL,
  `c7` varchar(6) DEFAULT NULL,
  `c8` varchar(6) DEFAULT NULL,
  `c9` varchar(6) DEFAULT NULL,
  `c10` varchar(6) DEFAULT NULL,
  `c11` varchar(6) DEFAULT NULL,
  `c12` varchar(6) DEFAULT NULL,
  `c13` varchar(6) DEFAULT NULL,
  `c14` varchar(6) DEFAULT NULL,
  `c15` varchar(6) DEFAULT NULL,
  `c16` varchar(6) DEFAULT NULL,
  `c17` varchar(6) DEFAULT NULL,
  `c18` varchar(6) DEFAULT NULL,
  `c19` varchar(6) DEFAULT NULL,
  `c20` varchar(6) DEFAULT NULL,
  `c21` varchar(6) DEFAULT NULL,
  `c22` varchar(6) DEFAULT NULL,
  `c23` varchar(6) DEFAULT NULL,
  `c24` varchar(6) DEFAULT NULL,
  `c25` varchar(6) DEFAULT NULL,
  `c26` varchar(6) DEFAULT NULL,
  `c27` varchar(6) DEFAULT NULL,
  `c28` varchar(6) DEFAULT NULL,
  `c29` varchar(6) DEFAULT NULL,
  `c30` varchar(6) DEFAULT NULL,
  `c31` varchar(6) DEFAULT NULL,
  `c32` varchar(6) DEFAULT NULL,
  `i1` varchar(6) DEFAULT NULL,
  `i2` varchar(6) DEFAULT NULL,
  `i3` varchar(6) DEFAULT NULL,
  `i4` varchar(6) DEFAULT NULL,
  `i5` varchar(6) DEFAULT NULL,
  `i6` varchar(6) DEFAULT NULL,
  `i7` varchar(6) DEFAULT NULL,
  `i8` varchar(6) DEFAULT NULL,
  `i9` varchar(6) DEFAULT NULL,
  `i10` varchar(6) DEFAULT NULL,
  `i11` varchar(6) DEFAULT NULL,
  `i12` varchar(6) DEFAULT NULL,
  `i13` varchar(6) DEFAULT NULL,
  `i14` varchar(6) DEFAULT NULL,
  `i15` varchar(6) DEFAULT NULL,
  `i16` varchar(6) DEFAULT NULL,
  `i17` varchar(6) DEFAULT NULL,
  `i18` varchar(6) DEFAULT NULL,
  `i19` varchar(6) DEFAULT NULL,
  `i20` varchar(6) DEFAULT NULL,
  `i21` varchar(6) DEFAULT NULL,
  `i22` varchar(6) DEFAULT NULL,
  `i23` varchar(6) DEFAULT NULL,
  `i24` varchar(6) DEFAULT NULL,
  `i25` varchar(6) DEFAULT NULL,
  `i26` varchar(6) DEFAULT NULL,
  `i27` varchar(6) DEFAULT NULL,
  `i28` varchar(6) DEFAULT NULL,
  `i29` varchar(6) DEFAULT NULL,
  `i30` varchar(6) DEFAULT NULL,
  `i31` varchar(6) DEFAULT NULL,
  `i32` varchar(6) DEFAULT NULL,
  `cdak1` int(11) DEFAULT '0',
  `cdak2` int(11) DEFAULT '0',
  `cdak3` int(11) DEFAULT '0',
  `cdak4` int(11) DEFAULT '0',
  `cdak5` int(11) DEFAULT '0',
  `cdak6` int(11) DEFAULT '0',
  `cdak7` int(11) DEFAULT '0',
  `cdak8` int(11) DEFAULT '0',
  `cdak9` int(11) DEFAULT '0',
  `cdak10` int(11) DEFAULT '0',
  `cdak11` int(11) DEFAULT '0',
  `cdak12` int(11) DEFAULT '0',
  `cdak13` int(11) DEFAULT '0',
  `cdak14` int(11) DEFAULT '0',
  `cdak15` int(11) DEFAULT '0',
  `cdak16` int(11) DEFAULT '0',
  `cdak17` int(11) DEFAULT '0',
  `cdak18` int(11) DEFAULT '0',
  `cdak19` int(11) DEFAULT '0',
  `cdak20` int(11) DEFAULT '0',
  `cdak21` int(11) DEFAULT '0',
  `cdak22` int(11) DEFAULT '0',
  `cdak23` int(11) DEFAULT '0',
  `cdak24` int(11) DEFAULT '0',
  `cdak25` int(11) DEFAULT '0',
  `cdak26` int(11) DEFAULT '0',
  `cdak27` int(11) DEFAULT '0',
  `cdak28` int(11) DEFAULT '0',
  `cdak29` int(11) DEFAULT '0',
  `cdak30` int(11) DEFAULT '0',
  `cdak31` int(11) DEFAULT '0',
  `cdak32` int(11) DEFAULT '0',
  `idak1` int(11) DEFAULT '0',
  `idak2` int(11) DEFAULT '0',
  `idak3` int(11) DEFAULT '0',
  `idak4` int(11) DEFAULT '0',
  `idak5` int(11) DEFAULT '0',
  `idak6` int(11) DEFAULT '0',
  `idak7` int(11) DEFAULT '0',
  `idak8` int(11) DEFAULT '0',
  `idak9` int(11) DEFAULT '0',
  `idak10` int(11) DEFAULT '0',
  `idak11` int(11) DEFAULT '0',
  `idak12` int(11) DEFAULT '0',
  `toplamsure` varchar(5) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  `izintipi` varchar(10) DEFAULT NULL,
  `turid` int(11) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `htk` varchar(6) DEFAULT NULL COMMENT 'hafta tatili kesintisi',
  `hit` varchar(6) DEFAULT NULL COMMENT 'hafta sonu mesaisini haftaiçine tamamlar',
  `puantajid` int(11) DEFAULT NULL,
  `puantajmax` int(11) DEFAULT '0',
  `altgrupid` int(11) DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `gcidgec` bigint(20) DEFAULT NULL COMMENT 'geckalma ya ait giriscikis tablosundaki kaydin id si',
  `gciderk` bigint(20) DEFAULT NULL COMMENT 'erkencikma ya ait giriscikis tablosundaki kaydin id si',
  `gcideks` bigint(20) DEFAULT NULL COMMENT 'eksiksure ye ait giriscikis tablosundaki kaydin id si',
  `gcidmes` bigint(20) DEFAULT NULL,
  `kapiterminalid` int(11) DEFAULT NULL COMMENT 'ilkgiris ve soncikis icin',
  `eksikvar` tinyint(1) DEFAULT '0' COMMENT 'dvm gec erk eks den harhangi bir var ise',
  `giristerminalad` varchar(10) DEFAULT NULL,
  `cikisterminalad` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`,`girissaati`,`cikissaati`),
  KEY `idx_puantaj_personel_tarih` (`personelid`,`tarih`),
  KEY `idx_puantaj_tarih_personel` (`tarih`,`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_puantaj1`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_puantaj1` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `devamsizlik` varchar(6) DEFAULT NULL,
  `geckalma` varchar(6) DEFAULT NULL,
  `erkencikma` varchar(6) DEFAULT NULL,
  `eksiksure` varchar(6) DEFAULT NULL,
  `devamsizlik_ceza` varchar(6) DEFAULT NULL,
  `geckalma_ceza` varchar(6) DEFAULT NULL,
  `erkencikma_ceza` varchar(6) DEFAULT NULL,
  `eksiksure_ceza` varchar(6) DEFAULT NULL,
  `devamsizlik_net` varchar(6) DEFAULT NULL,
  `geckalma_net` varchar(6) DEFAULT NULL,
  `erkencikma_net` varchar(6) DEFAULT NULL,
  `eksiksure_net` varchar(6) DEFAULT NULL,
  `devamsizlik_dak` int(11) DEFAULT '0',
  `geckalma_dak` int(11) DEFAULT '0',
  `erkencikma_dak` int(11) DEFAULT '0',
  `eksiksure_dak` int(11) DEFAULT '0',
  `devamsizlik_cezadak` int(11) DEFAULT '0',
  `geckalma_cezadak` int(11) DEFAULT '0',
  `erkencikma_cezadak` int(11) DEFAULT '0',
  `eksiksure_cezadak` int(11) DEFAULT '0',
  `devamsizlik_netdak` int(11) DEFAULT '0',
  `geckalma_netdak` int(11) DEFAULT '0',
  `erkencikma_netdak` int(11) DEFAULT '0',
  `eksiksure_netdak` int(11) DEFAULT '0',
  `c1` varchar(6) DEFAULT NULL,
  `c2` varchar(6) DEFAULT NULL,
  `c3` varchar(6) DEFAULT NULL,
  `c4` varchar(6) DEFAULT NULL,
  `c5` varchar(6) DEFAULT NULL,
  `c6` varchar(6) DEFAULT NULL,
  `c7` varchar(6) DEFAULT NULL,
  `c8` varchar(6) DEFAULT NULL,
  `c9` varchar(6) DEFAULT NULL,
  `c10` varchar(6) DEFAULT NULL,
  `c11` varchar(6) DEFAULT NULL,
  `c12` varchar(6) DEFAULT NULL,
  `c13` varchar(6) DEFAULT NULL,
  `c14` varchar(6) DEFAULT NULL,
  `c15` varchar(6) DEFAULT NULL,
  `c16` varchar(6) DEFAULT NULL,
  `c17` varchar(6) DEFAULT NULL,
  `c18` varchar(6) DEFAULT NULL,
  `c19` varchar(6) DEFAULT NULL,
  `c20` varchar(6) DEFAULT NULL,
  `c21` varchar(6) DEFAULT NULL,
  `c22` varchar(6) DEFAULT NULL,
  `c23` varchar(6) DEFAULT NULL,
  `c24` varchar(6) DEFAULT NULL,
  `c25` varchar(6) DEFAULT NULL,
  `c26` varchar(6) DEFAULT NULL,
  `c27` varchar(6) DEFAULT NULL,
  `c28` varchar(6) DEFAULT NULL,
  `c29` varchar(6) DEFAULT NULL,
  `c30` varchar(6) DEFAULT NULL,
  `c31` varchar(6) DEFAULT NULL,
  `c32` varchar(6) DEFAULT NULL,
  `i1` varchar(6) DEFAULT NULL,
  `i2` varchar(6) DEFAULT NULL,
  `i3` varchar(6) DEFAULT NULL,
  `i4` varchar(6) DEFAULT NULL,
  `i5` varchar(6) DEFAULT NULL,
  `i6` varchar(6) DEFAULT NULL,
  `i7` varchar(6) DEFAULT NULL,
  `i8` varchar(6) DEFAULT NULL,
  `i9` varchar(6) DEFAULT NULL,
  `i10` varchar(6) DEFAULT NULL,
  `i11` varchar(6) DEFAULT NULL,
  `i12` varchar(6) DEFAULT NULL,
  `i13` varchar(6) DEFAULT NULL,
  `i14` varchar(6) DEFAULT NULL,
  `i15` varchar(6) DEFAULT NULL,
  `i16` varchar(6) DEFAULT NULL,
  `i17` varchar(6) DEFAULT NULL,
  `i18` varchar(6) DEFAULT NULL,
  `i19` varchar(6) DEFAULT NULL,
  `i20` varchar(6) DEFAULT NULL,
  `i21` varchar(6) DEFAULT NULL,
  `i22` varchar(6) DEFAULT NULL,
  `i23` varchar(6) DEFAULT NULL,
  `i24` varchar(6) DEFAULT NULL,
  `i25` varchar(6) DEFAULT NULL,
  `i26` varchar(6) DEFAULT NULL,
  `i27` varchar(6) DEFAULT NULL,
  `i28` varchar(6) DEFAULT NULL,
  `i29` varchar(6) DEFAULT NULL,
  `i30` varchar(6) DEFAULT NULL,
  `i31` varchar(6) DEFAULT NULL,
  `i32` varchar(6) DEFAULT NULL,
  `cdak1` int(11) DEFAULT '0',
  `cdak2` int(11) DEFAULT '0',
  `cdak3` int(11) DEFAULT '0',
  `cdak4` int(11) DEFAULT '0',
  `cdak5` int(11) DEFAULT '0',
  `cdak6` int(11) DEFAULT '0',
  `cdak7` int(11) DEFAULT '0',
  `cdak8` int(11) DEFAULT '0',
  `cdak9` int(11) DEFAULT '0',
  `cdak10` int(11) DEFAULT '0',
  `cdak11` int(11) DEFAULT '0',
  `cdak12` int(11) DEFAULT '0',
  `cdak13` int(11) DEFAULT '0',
  `cdak14` int(11) DEFAULT '0',
  `cdak15` int(11) DEFAULT '0',
  `cdak16` int(11) DEFAULT '0',
  `cdak17` int(11) DEFAULT '0',
  `cdak18` int(11) DEFAULT '0',
  `cdak19` int(11) DEFAULT '0',
  `cdak20` int(11) DEFAULT '0',
  `cdak21` int(11) DEFAULT '0',
  `cdak22` int(11) DEFAULT '0',
  `cdak23` int(11) DEFAULT '0',
  `cdak24` int(11) DEFAULT '0',
  `cdak25` int(11) DEFAULT '0',
  `cdak26` int(11) DEFAULT '0',
  `cdak27` int(11) DEFAULT '0',
  `cdak28` int(11) DEFAULT '0',
  `cdak29` int(11) DEFAULT '0',
  `cdak30` int(11) DEFAULT '0',
  `cdak31` int(11) DEFAULT '0',
  `cdak32` int(11) DEFAULT '0',
  `idak1` int(11) DEFAULT '0',
  `idak2` int(11) DEFAULT '0',
  `idak3` int(11) DEFAULT '0',
  `idak4` int(11) DEFAULT '0',
  `idak5` int(11) DEFAULT '0',
  `idak6` int(11) DEFAULT '0',
  `idak7` int(11) DEFAULT '0',
  `idak8` int(11) DEFAULT '0',
  `idak9` int(11) DEFAULT '0',
  `idak10` int(11) DEFAULT '0',
  `idak11` int(11) DEFAULT '0',
  `idak12` int(11) DEFAULT '0',
  `toplamsure` varchar(5) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  `izintipi` varchar(10) DEFAULT NULL,
  `turid` int(11) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `htk` varchar(6) DEFAULT NULL COMMENT 'hafta tatili kesintisi',
  `hit` varchar(6) DEFAULT NULL COMMENT 'hafta sonu mesaisini haftaiçine tamamlar',
  `puantajid` int(11) DEFAULT NULL,
  `puantajmax` int(11) DEFAULT '0',
  `altgrupid` int(11) DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `gcidgec` bigint(20) DEFAULT NULL,
  `gciderk` bigint(20) DEFAULT NULL,
  `gcideks` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`,`girissaati`,`cikissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_puantajparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_puantajparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `saat` char(6) DEFAULT NULL COMMENT 'gunluk sure',
  `appTask` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_puantajtoplam`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_puantajtoplam` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(10) NOT NULL,
  `bastarih` date DEFAULT NULL,
  `bittarih` date DEFAULT NULL,
  `devamsizlikgun` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat` varchar(6) DEFAULT NULL,
  `devamsizlikucret` decimal(11,2) DEFAULT NULL,
  `geckalmagun` decimal(11,2) DEFAULT NULL,
  `geckalmasaat` varchar(6) DEFAULT NULL,
  `geckalmaucret` decimal(11,2) DEFAULT NULL,
  `erkencikmagun` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat` varchar(6) DEFAULT NULL,
  `erkencikmaucret` decimal(11,2) DEFAULT NULL,
  `eksiksuregun` decimal(11,2) DEFAULT NULL,
  `eksiksuresaat` varchar(6) DEFAULT NULL,
  `eksiksureucret` decimal(11,2) DEFAULT NULL,
  `devamsizlikgun_ceza` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat_ceza` varchar(6) DEFAULT NULL,
  `devamsizlikucret_ceza` decimal(11,2) DEFAULT NULL,
  `geckalmagun_ceza` decimal(11,2) DEFAULT NULL,
  `geckalmasaat_ceza` varchar(6) DEFAULT NULL,
  `geckalmaucret_ceza` decimal(11,2) DEFAULT NULL,
  `erkencikmagun_ceza` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat_ceza` varchar(6) DEFAULT NULL,
  `erkencikmaucret_ceza` decimal(11,2) DEFAULT NULL,
  `eksiksuregun_ceza` decimal(11,2) DEFAULT NULL,
  `eksiksuresaat_ceza` varchar(6) DEFAULT NULL,
  `eksiksureucret_ceza` decimal(11,2) DEFAULT NULL,
  `devamsizlikgun_net` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat_net` varchar(6) DEFAULT NULL,
  `devamsizlikucret_net` decimal(11,2) DEFAULT NULL,
  `geckalmagun_net` decimal(11,2) DEFAULT NULL,
  `geckalmasaat_net` varchar(6) DEFAULT NULL,
  `geckalmaucret_net` decimal(11,2) DEFAULT NULL,
  `erkencikmagun_net` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat_net` varchar(6) DEFAULT NULL,
  `erkencikmaucret_net` decimal(11,2) DEFAULT NULL,
  `eksiksuregun_net` decimal(11,2) DEFAULT NULL,
  `eksiksuresaat_net` varchar(6) DEFAULT NULL,
  `eksiksureucret_net` decimal(11,2) DEFAULT NULL,
  `cgun1` decimal(11,2) DEFAULT NULL,
  `csaat1` varchar(6) DEFAULT NULL,
  `cucret1` decimal(11,2) DEFAULT NULL,
  `cgun2` decimal(11,2) DEFAULT NULL,
  `csaat2` varchar(6) DEFAULT NULL,
  `cucret2` decimal(11,2) DEFAULT NULL,
  `cgun3` decimal(11,2) DEFAULT NULL,
  `csaat3` varchar(6) DEFAULT NULL,
  `cucret3` decimal(11,2) DEFAULT NULL,
  `cgun4` decimal(11,2) DEFAULT NULL,
  `csaat4` varchar(6) DEFAULT NULL,
  `cucret4` decimal(11,2) DEFAULT NULL,
  `cgun5` decimal(11,2) DEFAULT NULL,
  `csaat5` varchar(6) DEFAULT NULL,
  `cucret5` decimal(11,2) DEFAULT NULL,
  `cgun6` decimal(11,2) DEFAULT NULL,
  `csaat6` varchar(6) DEFAULT NULL,
  `cucret6` decimal(11,2) DEFAULT NULL,
  `cgun7` decimal(11,2) DEFAULT NULL,
  `csaat7` varchar(6) DEFAULT NULL,
  `cucret7` decimal(11,2) DEFAULT NULL,
  `cgun8` decimal(11,2) DEFAULT NULL,
  `csaat8` varchar(6) DEFAULT NULL,
  `cucret8` decimal(11,2) DEFAULT NULL,
  `cgun9` decimal(11,2) DEFAULT NULL,
  `csaat9` varchar(6) DEFAULT NULL,
  `cucret9` decimal(11,2) DEFAULT NULL,
  `cgun10` decimal(11,2) DEFAULT NULL,
  `csaat10` varchar(6) DEFAULT NULL,
  `cucret10` decimal(11,2) DEFAULT NULL,
  `cgun11` decimal(11,2) DEFAULT NULL,
  `csaat11` varchar(6) DEFAULT NULL,
  `cucret11` decimal(11,2) DEFAULT NULL,
  `cgun12` decimal(11,2) DEFAULT NULL,
  `csaat12` varchar(6) DEFAULT NULL,
  `cucret12` decimal(11,2) DEFAULT NULL,
  `cgun13` decimal(11,2) DEFAULT NULL,
  `csaat13` varchar(6) DEFAULT NULL,
  `cucret13` decimal(11,2) DEFAULT NULL,
  `cgun14` decimal(11,2) DEFAULT NULL,
  `csaat14` varchar(6) DEFAULT NULL,
  `cucret14` decimal(11,2) DEFAULT NULL,
  `cgun15` decimal(11,2) DEFAULT NULL,
  `csaat15` varchar(6) DEFAULT NULL,
  `cucret15` decimal(11,2) DEFAULT NULL,
  `cgun16` decimal(11,2) DEFAULT NULL,
  `csaat16` varchar(6) DEFAULT NULL,
  `cucret16` decimal(11,2) DEFAULT NULL,
  `cgun17` decimal(11,2) DEFAULT NULL,
  `csaat17` varchar(6) DEFAULT NULL,
  `cucret17` decimal(11,2) DEFAULT NULL,
  `cgun18` decimal(11,2) DEFAULT NULL,
  `csaat18` varchar(6) DEFAULT NULL,
  `cucret18` decimal(11,2) DEFAULT NULL,
  `cgun19` decimal(11,2) DEFAULT NULL,
  `csaat19` varchar(6) DEFAULT NULL,
  `cucret19` decimal(11,2) DEFAULT NULL,
  `cgun20` decimal(11,2) DEFAULT NULL,
  `csaat20` varchar(6) DEFAULT NULL,
  `cucret20` decimal(11,2) DEFAULT NULL,
  `cgun21` decimal(11,2) DEFAULT NULL,
  `csaat21` varchar(6) DEFAULT NULL,
  `cucret21` decimal(11,2) DEFAULT NULL,
  `cgun22` decimal(11,2) DEFAULT NULL,
  `csaat22` varchar(6) DEFAULT NULL,
  `cucret22` decimal(11,2) DEFAULT NULL,
  `cgun23` decimal(11,2) DEFAULT NULL,
  `csaat23` varchar(6) DEFAULT NULL,
  `cucret23` decimal(11,2) DEFAULT NULL,
  `cgun24` decimal(11,2) DEFAULT NULL,
  `csaat24` varchar(6) DEFAULT NULL,
  `cucret24` decimal(11,2) DEFAULT NULL,
  `cgun25` decimal(11,2) DEFAULT NULL,
  `csaat25` varchar(6) DEFAULT NULL,
  `cucret25` decimal(11,2) DEFAULT NULL,
  `cgun26` decimal(11,2) DEFAULT NULL,
  `csaat26` varchar(6) DEFAULT NULL,
  `cucret26` decimal(11,2) DEFAULT NULL,
  `cgun27` decimal(11,2) DEFAULT NULL,
  `csaat27` varchar(6) DEFAULT NULL,
  `cucret27` decimal(11,2) DEFAULT NULL,
  `cgun28` decimal(11,2) DEFAULT NULL,
  `csaat28` varchar(6) DEFAULT NULL,
  `cucret28` decimal(11,2) DEFAULT NULL,
  `cgun29` decimal(11,2) DEFAULT NULL,
  `csaat29` varchar(6) DEFAULT NULL,
  `cucret29` decimal(11,2) DEFAULT NULL,
  `cgun30` decimal(11,2) DEFAULT NULL,
  `csaat30` varchar(6) DEFAULT NULL,
  `cucret30` decimal(11,2) DEFAULT NULL,
  `cgun31` decimal(11,2) DEFAULT NULL,
  `csaat31` varchar(6) DEFAULT NULL,
  `cucret31` decimal(11,2) DEFAULT NULL,
  `cgun32` decimal(11,2) DEFAULT NULL,
  `csaat32` varchar(6) DEFAULT NULL,
  `cucret32` decimal(11,2) DEFAULT NULL,
  `igun1` decimal(11,2) DEFAULT NULL,
  `isaat1` varchar(6) DEFAULT NULL,
  `iucret1` decimal(11,2) DEFAULT NULL,
  `igun2` decimal(11,2) DEFAULT NULL,
  `isaat2` varchar(6) DEFAULT NULL,
  `iucret2` decimal(11,2) DEFAULT NULL,
  `igun3` decimal(11,2) DEFAULT NULL,
  `isaat3` varchar(6) DEFAULT NULL,
  `iucret3` decimal(11,2) DEFAULT NULL,
  `igun4` decimal(11,2) DEFAULT NULL,
  `isaat4` varchar(6) DEFAULT NULL,
  `iucret4` decimal(11,2) DEFAULT NULL,
  `igun5` decimal(11,2) DEFAULT NULL,
  `isaat5` varchar(6) DEFAULT NULL,
  `iucret5` decimal(11,2) DEFAULT NULL,
  `igun6` decimal(11,2) DEFAULT NULL,
  `isaat6` varchar(6) DEFAULT NULL,
  `iucret6` decimal(11,2) DEFAULT NULL,
  `igun7` decimal(11,2) DEFAULT NULL,
  `isaat7` varchar(6) DEFAULT NULL,
  `iucret7` decimal(11,2) DEFAULT NULL,
  `igun8` decimal(11,2) DEFAULT NULL,
  `isaat8` varchar(6) DEFAULT NULL,
  `iucret8` decimal(11,2) DEFAULT NULL,
  `igun9` decimal(11,2) DEFAULT NULL,
  `isaat9` varchar(6) DEFAULT NULL,
  `iucret9` decimal(11,2) DEFAULT NULL,
  `igun10` decimal(11,2) DEFAULT NULL,
  `isaat10` varchar(6) DEFAULT NULL,
  `iucret10` decimal(11,2) DEFAULT NULL,
  `igun11` decimal(11,2) DEFAULT NULL,
  `isaat11` varchar(6) DEFAULT NULL,
  `iucret11` decimal(11,2) DEFAULT NULL,
  `igun12` decimal(11,2) DEFAULT NULL,
  `isaat12` varchar(6) DEFAULT NULL,
  `iucret12` decimal(11,2) DEFAULT NULL,
  `igun13` decimal(11,2) DEFAULT NULL,
  `isaat13` varchar(6) DEFAULT NULL,
  `iucret13` decimal(11,2) DEFAULT NULL,
  `igun14` decimal(11,2) DEFAULT NULL,
  `isaat14` varchar(6) DEFAULT NULL,
  `iucret14` decimal(11,2) DEFAULT NULL,
  `igun15` decimal(11,2) DEFAULT NULL,
  `isaat15` varchar(6) DEFAULT NULL,
  `iucret15` decimal(11,2) DEFAULT NULL,
  `igun16` decimal(11,2) DEFAULT NULL,
  `isaat16` varchar(6) DEFAULT NULL,
  `iucret16` decimal(11,2) DEFAULT NULL,
  `igun17` decimal(11,2) DEFAULT NULL,
  `isaat17` varchar(6) DEFAULT NULL,
  `iucret17` decimal(11,2) DEFAULT NULL,
  `igun18` decimal(11,2) DEFAULT NULL,
  `isaat18` varchar(6) DEFAULT NULL,
  `iucret18` decimal(11,2) DEFAULT NULL,
  `igun19` decimal(11,2) DEFAULT NULL,
  `isaat19` varchar(6) DEFAULT NULL,
  `iucret19` decimal(11,2) DEFAULT NULL,
  `igun20` decimal(11,2) DEFAULT NULL,
  `isaat20` varchar(6) DEFAULT NULL,
  `iucret20` decimal(11,2) DEFAULT NULL,
  `igun21` decimal(11,2) DEFAULT NULL,
  `isaat21` varchar(6) DEFAULT NULL,
  `iucret21` decimal(11,2) DEFAULT NULL,
  `igun22` decimal(11,2) DEFAULT NULL,
  `isaat22` varchar(6) DEFAULT NULL,
  `iucret22` decimal(11,2) DEFAULT NULL,
  `igun23` decimal(11,2) DEFAULT NULL,
  `isaat23` varchar(6) DEFAULT NULL,
  `iucret23` decimal(11,2) DEFAULT NULL,
  `igun24` decimal(11,2) DEFAULT NULL,
  `isaat24` varchar(6) DEFAULT NULL,
  `iucret24` decimal(11,2) DEFAULT NULL,
  `igun25` decimal(11,2) DEFAULT NULL,
  `isaat25` varchar(6) DEFAULT NULL,
  `iucret25` decimal(11,2) DEFAULT NULL,
  `igun26` decimal(11,2) DEFAULT NULL,
  `isaat26` varchar(6) DEFAULT NULL,
  `iucret26` decimal(11,2) DEFAULT NULL,
  `igun27` decimal(11,2) DEFAULT NULL,
  `isaat27` varchar(6) DEFAULT NULL,
  `iucret27` decimal(11,2) DEFAULT NULL,
  `igun28` decimal(11,2) DEFAULT NULL,
  `isaat28` varchar(6) DEFAULT NULL,
  `iucret28` decimal(11,2) DEFAULT NULL,
  `igun29` decimal(11,2) DEFAULT NULL,
  `isaat29` varchar(6) DEFAULT NULL,
  `iucret29` decimal(11,2) DEFAULT NULL,
  `igun30` decimal(11,2) DEFAULT NULL,
  `isaat30` varchar(6) DEFAULT NULL,
  `iucret30` decimal(11,2) DEFAULT NULL,
  `igun31` decimal(11,2) DEFAULT NULL,
  `isaat31` varchar(6) DEFAULT NULL,
  `iucret31` decimal(11,2) DEFAULT NULL,
  `igun32` decimal(11,2) DEFAULT NULL,
  `isaat32` varchar(6) DEFAULT NULL,
  `iucret32` decimal(11,2) DEFAULT NULL,
  `eucret1` decimal(11,2) DEFAULT NULL,
  `eucret2` decimal(11,2) DEFAULT NULL,
  `eucret3` decimal(11,2) DEFAULT NULL,
  `eucret4` decimal(11,2) DEFAULT NULL,
  `eucret5` decimal(11,2) DEFAULT NULL,
  `eucret6` decimal(11,2) DEFAULT NULL,
  `eucret7` decimal(11,2) DEFAULT NULL,
  `eucret8` decimal(11,2) DEFAULT NULL,
  `eucret9` decimal(11,2) DEFAULT NULL,
  `eucret10` decimal(11,2) DEFAULT NULL,
  `eucret11` decimal(11,2) DEFAULT NULL,
  `eucret12` decimal(11,2) DEFAULT NULL,
  `eucret13` decimal(11,2) DEFAULT NULL,
  `eucret14` decimal(11,2) DEFAULT NULL,
  `eucret15` decimal(11,2) DEFAULT NULL,
  `eucret16` decimal(11,2) DEFAULT NULL,
  `eucret17` decimal(11,2) DEFAULT NULL,
  `eucret18` decimal(11,2) DEFAULT NULL,
  `eucret19` decimal(11,2) DEFAULT NULL,
  `eucret20` decimal(11,2) DEFAULT NULL,
  `eucret21` decimal(11,2) DEFAULT NULL,
  `eucret22` decimal(11,2) DEFAULT NULL,
  `eucret23` decimal(11,2) DEFAULT NULL,
  `eucret24` decimal(11,2) DEFAULT NULL,
  `eucret25` decimal(11,2) DEFAULT NULL,
  `eucret26` decimal(11,2) DEFAULT NULL,
  `eucret27` decimal(11,2) DEFAULT NULL,
  `eucret28` decimal(11,2) DEFAULT NULL,
  `eucret29` decimal(11,2) DEFAULT NULL,
  `eucret30` decimal(11,2) DEFAULT NULL,
  `eucret31` decimal(11,2) DEFAULT NULL,
  `eucret32` decimal(11,2) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  `htkgun` decimal(11,2) DEFAULT NULL,
  `htksaat` varchar(6) DEFAULT NULL,
  `htkucret` decimal(11,2) DEFAULT NULL,
  `hitgun` decimal(11,2) DEFAULT NULL,
  `hitsaat` varchar(6) DEFAULT NULL,
  `hitucret` decimal(11,2) DEFAULT NULL,
  `donemfarkigun` decimal(11,2) DEFAULT NULL,
  `donemfarkisaat` varchar(6) DEFAULT NULL,
  `donemfarkiucret` decimal(11,2) DEFAULT NULL,
  `agiucret` decimal(11,2) DEFAULT NULL,
  `primucret` decimal(11,2) DEFAULT NULL,
  `yolucret` decimal(11,2) DEFAULT NULL,
  `yemekucret` decimal(11,2) DEFAULT NULL,
  `wcsaat` varchar(6) DEFAULT NULL,
  `wcucret` decimal(11,2) DEFAULT NULL,
  `nctoplam` decimal(11,2) DEFAULT NULL,
  `fmtoplam` decimal(11,2) DEFAULT NULL,
  `iztoplam` decimal(11,2) DEFAULT NULL,
  `kktoplam` decimal(11,2) DEFAULT NULL,
  `nchakedis` decimal(11,2) DEFAULT NULL COMMENT 'nctoplam + kktoplam',
  `fmhakedis` decimal(11,2) DEFAULT NULL,
  `ncodenen` decimal(11,2) DEFAULT NULL,
  `fmodenen` decimal(11,2) DEFAULT NULL,
  `nckalan` decimal(11,2) DEFAULT NULL,
  `fmkalan` decimal(11,2) DEFAULT NULL,
  `devir` decimal(11,2) DEFAULT NULL COMMENT 'devir=(maashakedis+mesaihakedis)-(maasodenen+mesaiodenen)  ->gecen ay icin',
  `geneltoplam` decimal(11,2) DEFAULT NULL COMMENT 'eneltoplam=(maashakedis+mesaihakedis+devir)-(maasodenen+mesaiodenen)  ->gecen ay icin',
  `kstsaat` varchar(6) DEFAULT NULL,
  `kstucret` decimal(11,2) DEFAULT NULL,
  `ozelucret` decimal(11,2) DEFAULT NULL,
  `ncgun` decimal(11,2) DEFAULT NULL,
  `ncsaat` varchar(6) DEFAULT NULL,
  `ncucret` decimal(11,2) DEFAULT NULL,
  `geneltoplam1` decimal(11,2) DEFAULT NULL,
  `idonemfarkisaat` int(11) DEFAULT '0',
  `ucret` decimal(11,2) DEFAULT NULL,
  `ncsaatucreti` decimal(11,2) DEFAULT NULL,
  `fm1saatucreti` decimal(11,2) DEFAULT NULL,
  `fm2saatucreti` decimal(11,2) DEFAULT NULL,
  `fm3saatucreti` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`bastarih`,`bittarih`),
  KEY `idx_puantajtoplam_personel` (`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_puantajtoplam1`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_puantajtoplam1` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(10) NOT NULL,
  `bastarih` date DEFAULT NULL,
  `bittarih` date DEFAULT NULL,
  `devamsizlikgun` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat` varchar(6) DEFAULT NULL,
  `devamsizlikucret` decimal(11,2) DEFAULT NULL,
  `geckalmagun` decimal(11,2) DEFAULT NULL,
  `geckalmasaat` varchar(6) DEFAULT NULL,
  `geckalmaucret` decimal(11,2) DEFAULT NULL,
  `erkencikmagun` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat` varchar(6) DEFAULT NULL,
  `erkencikmaucret` decimal(11,2) DEFAULT NULL,
  `eksiksuregun` decimal(11,2) DEFAULT NULL,
  `eksiksuresaat` varchar(6) DEFAULT NULL,
  `eksiksureucret` decimal(11,2) DEFAULT NULL,
  `devamsizlikgun_ceza` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat_ceza` varchar(6) DEFAULT NULL,
  `devamsizlikucret_ceza` decimal(11,2) DEFAULT NULL,
  `geckalmagun_ceza` decimal(11,2) DEFAULT NULL,
  `geckalmasaat_ceza` varchar(6) DEFAULT NULL,
  `geckalmaucret_ceza` decimal(11,2) DEFAULT NULL,
  `erkencikmagun_ceza` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat_ceza` varchar(6) DEFAULT NULL,
  `erkencikmaucret_ceza` decimal(11,2) DEFAULT NULL,
  `eksiksuregun_ceza` decimal(11,2) DEFAULT NULL,
  `eksiksuresaat_ceza` varchar(6) DEFAULT NULL,
  `eksiksureucret_ceza` decimal(11,2) DEFAULT NULL,
  `devamsizlikgun_net` decimal(11,2) DEFAULT NULL,
  `devamsizliksaat_net` varchar(6) DEFAULT NULL,
  `devamsizlikucret_net` decimal(11,2) DEFAULT NULL,
  `geckalmagun_net` decimal(11,2) DEFAULT NULL,
  `geckalmasaat_net` varchar(6) DEFAULT NULL,
  `geckalmaucret_net` decimal(11,2) DEFAULT NULL,
  `erkencikmagun_net` decimal(11,2) DEFAULT NULL,
  `erkencikmasaat_net` varchar(6) DEFAULT NULL,
  `erkencikmaucret_net` decimal(11,2) DEFAULT NULL,
  `eksiksuregun_net` decimal(11,2) DEFAULT NULL,
  `eksiksuresaat_net` varchar(6) DEFAULT NULL,
  `eksiksureucret_net` decimal(11,2) DEFAULT NULL,
  `cgun1` decimal(11,2) DEFAULT NULL,
  `csaat1` varchar(6) DEFAULT NULL,
  `cucret1` decimal(11,2) DEFAULT NULL,
  `cgun2` decimal(11,2) DEFAULT NULL,
  `csaat2` varchar(6) DEFAULT NULL,
  `cucret2` decimal(11,2) DEFAULT NULL,
  `cgun3` decimal(11,2) DEFAULT NULL,
  `csaat3` varchar(6) DEFAULT NULL,
  `cucret3` decimal(11,2) DEFAULT NULL,
  `cgun4` decimal(11,2) DEFAULT NULL,
  `csaat4` varchar(6) DEFAULT NULL,
  `cucret4` decimal(11,2) DEFAULT NULL,
  `cgun5` decimal(11,2) DEFAULT NULL,
  `csaat5` varchar(6) DEFAULT NULL,
  `cucret5` decimal(11,2) DEFAULT NULL,
  `cgun6` decimal(11,2) DEFAULT NULL,
  `csaat6` varchar(6) DEFAULT NULL,
  `cucret6` decimal(11,2) DEFAULT NULL,
  `cgun7` decimal(11,2) DEFAULT NULL,
  `csaat7` varchar(6) DEFAULT NULL,
  `cucret7` decimal(11,2) DEFAULT NULL,
  `cgun8` decimal(11,2) DEFAULT NULL,
  `csaat8` varchar(6) DEFAULT NULL,
  `cucret8` decimal(11,2) DEFAULT NULL,
  `cgun9` decimal(11,2) DEFAULT NULL,
  `csaat9` varchar(6) DEFAULT NULL,
  `cucret9` decimal(11,2) DEFAULT NULL,
  `cgun10` decimal(11,2) DEFAULT NULL,
  `csaat10` varchar(6) DEFAULT NULL,
  `cucret10` decimal(11,2) DEFAULT NULL,
  `cgun11` decimal(11,2) DEFAULT NULL,
  `csaat11` varchar(6) DEFAULT NULL,
  `cucret11` decimal(11,2) DEFAULT NULL,
  `cgun12` decimal(11,2) DEFAULT NULL,
  `csaat12` varchar(6) DEFAULT NULL,
  `cucret12` decimal(11,2) DEFAULT NULL,
  `cgun13` decimal(11,2) DEFAULT NULL,
  `csaat13` varchar(6) DEFAULT NULL,
  `cucret13` decimal(11,2) DEFAULT NULL,
  `cgun14` decimal(11,2) DEFAULT NULL,
  `csaat14` varchar(6) DEFAULT NULL,
  `cucret14` decimal(11,2) DEFAULT NULL,
  `cgun15` decimal(11,2) DEFAULT NULL,
  `csaat15` varchar(6) DEFAULT NULL,
  `cucret15` decimal(11,2) DEFAULT NULL,
  `cgun16` decimal(11,2) DEFAULT NULL,
  `csaat16` varchar(6) DEFAULT NULL,
  `cucret16` decimal(11,2) DEFAULT NULL,
  `cgun17` decimal(11,2) DEFAULT NULL,
  `csaat17` varchar(6) DEFAULT NULL,
  `cucret17` decimal(11,2) DEFAULT NULL,
  `cgun18` decimal(11,2) DEFAULT NULL,
  `csaat18` varchar(6) DEFAULT NULL,
  `cucret18` decimal(11,2) DEFAULT NULL,
  `cgun19` decimal(11,2) DEFAULT NULL,
  `csaat19` varchar(6) DEFAULT NULL,
  `cucret19` decimal(11,2) DEFAULT NULL,
  `cgun20` decimal(11,2) DEFAULT NULL,
  `csaat20` varchar(6) DEFAULT NULL,
  `cucret20` decimal(11,2) DEFAULT NULL,
  `cgun21` decimal(11,2) DEFAULT NULL,
  `csaat21` varchar(6) DEFAULT NULL,
  `cucret21` decimal(11,2) DEFAULT NULL,
  `cgun22` decimal(11,2) DEFAULT NULL,
  `csaat22` varchar(6) DEFAULT NULL,
  `cucret22` decimal(11,2) DEFAULT NULL,
  `cgun23` decimal(11,2) DEFAULT NULL,
  `csaat23` varchar(6) DEFAULT NULL,
  `cucret23` decimal(11,2) DEFAULT NULL,
  `cgun24` decimal(11,2) DEFAULT NULL,
  `csaat24` varchar(6) DEFAULT NULL,
  `cucret24` decimal(11,2) DEFAULT NULL,
  `cgun25` decimal(11,2) DEFAULT NULL,
  `csaat25` varchar(6) DEFAULT NULL,
  `cucret25` decimal(11,2) DEFAULT NULL,
  `cgun26` decimal(11,2) DEFAULT NULL,
  `csaat26` varchar(6) DEFAULT NULL,
  `cucret26` decimal(11,2) DEFAULT NULL,
  `cgun27` decimal(11,2) DEFAULT NULL,
  `csaat27` varchar(6) DEFAULT NULL,
  `cucret27` decimal(11,2) DEFAULT NULL,
  `cgun28` decimal(11,2) DEFAULT NULL,
  `csaat28` varchar(6) DEFAULT NULL,
  `cucret28` decimal(11,2) DEFAULT NULL,
  `cgun29` decimal(11,2) DEFAULT NULL,
  `csaat29` varchar(6) DEFAULT NULL,
  `cucret29` decimal(11,2) DEFAULT NULL,
  `cgun30` decimal(11,2) DEFAULT NULL,
  `csaat30` varchar(6) DEFAULT NULL,
  `cucret30` decimal(11,2) DEFAULT NULL,
  `cgun31` decimal(11,2) DEFAULT NULL,
  `csaat31` varchar(6) DEFAULT NULL,
  `cucret31` decimal(11,2) DEFAULT NULL,
  `cgun32` decimal(11,2) DEFAULT NULL,
  `csaat32` varchar(6) DEFAULT NULL,
  `cucret32` decimal(11,2) DEFAULT NULL,
  `igun1` decimal(11,2) DEFAULT NULL,
  `isaat1` varchar(6) DEFAULT NULL,
  `iucret1` decimal(11,2) DEFAULT NULL,
  `igun2` decimal(11,2) DEFAULT NULL,
  `isaat2` varchar(6) DEFAULT NULL,
  `iucret2` decimal(11,2) DEFAULT NULL,
  `igun3` decimal(11,2) DEFAULT NULL,
  `isaat3` varchar(6) DEFAULT NULL,
  `iucret3` decimal(11,2) DEFAULT NULL,
  `igun4` decimal(11,2) DEFAULT NULL,
  `isaat4` varchar(6) DEFAULT NULL,
  `iucret4` decimal(11,2) DEFAULT NULL,
  `igun5` decimal(11,2) DEFAULT NULL,
  `isaat5` varchar(6) DEFAULT NULL,
  `iucret5` decimal(11,2) DEFAULT NULL,
  `igun6` decimal(11,2) DEFAULT NULL,
  `isaat6` varchar(6) DEFAULT NULL,
  `iucret6` decimal(11,2) DEFAULT NULL,
  `igun7` decimal(11,2) DEFAULT NULL,
  `isaat7` varchar(6) DEFAULT NULL,
  `iucret7` decimal(11,2) DEFAULT NULL,
  `igun8` decimal(11,2) DEFAULT NULL,
  `isaat8` varchar(6) DEFAULT NULL,
  `iucret8` decimal(11,2) DEFAULT NULL,
  `igun9` decimal(11,2) DEFAULT NULL,
  `isaat9` varchar(6) DEFAULT NULL,
  `iucret9` decimal(11,2) DEFAULT NULL,
  `igun10` decimal(11,2) DEFAULT NULL,
  `isaat10` varchar(6) DEFAULT NULL,
  `iucret10` decimal(11,2) DEFAULT NULL,
  `igun11` decimal(11,2) DEFAULT NULL,
  `isaat11` varchar(6) DEFAULT NULL,
  `iucret11` decimal(11,2) DEFAULT NULL,
  `igun12` decimal(11,2) DEFAULT NULL,
  `isaat12` varchar(6) DEFAULT NULL,
  `iucret12` decimal(11,2) DEFAULT NULL,
  `igun13` decimal(11,2) DEFAULT NULL,
  `isaat13` varchar(6) DEFAULT NULL,
  `iucret13` decimal(11,2) DEFAULT NULL,
  `igun14` decimal(11,2) DEFAULT NULL,
  `isaat14` varchar(6) DEFAULT NULL,
  `iucret14` decimal(11,2) DEFAULT NULL,
  `igun15` decimal(11,2) DEFAULT NULL,
  `isaat15` varchar(6) DEFAULT NULL,
  `iucret15` decimal(11,2) DEFAULT NULL,
  `igun16` decimal(11,2) DEFAULT NULL,
  `isaat16` varchar(6) DEFAULT NULL,
  `iucret16` decimal(11,2) DEFAULT NULL,
  `igun17` decimal(11,2) DEFAULT NULL,
  `isaat17` varchar(6) DEFAULT NULL,
  `iucret17` decimal(11,2) DEFAULT NULL,
  `igun18` decimal(11,2) DEFAULT NULL,
  `isaat18` varchar(6) DEFAULT NULL,
  `iucret18` decimal(11,2) DEFAULT NULL,
  `igun19` decimal(11,2) DEFAULT NULL,
  `isaat19` varchar(6) DEFAULT NULL,
  `iucret19` decimal(11,2) DEFAULT NULL,
  `igun20` decimal(11,2) DEFAULT NULL,
  `isaat20` varchar(6) DEFAULT NULL,
  `iucret20` decimal(11,2) DEFAULT NULL,
  `igun21` decimal(11,2) DEFAULT NULL,
  `isaat21` varchar(6) DEFAULT NULL,
  `iucret21` decimal(11,2) DEFAULT NULL,
  `igun22` decimal(11,2) DEFAULT NULL,
  `isaat22` varchar(6) DEFAULT NULL,
  `iucret22` decimal(11,2) DEFAULT NULL,
  `igun23` decimal(11,2) DEFAULT NULL,
  `isaat23` varchar(6) DEFAULT NULL,
  `iucret23` decimal(11,2) DEFAULT NULL,
  `igun24` decimal(11,2) DEFAULT NULL,
  `isaat24` varchar(6) DEFAULT NULL,
  `iucret24` decimal(11,2) DEFAULT NULL,
  `igun25` decimal(11,2) DEFAULT NULL,
  `isaat25` varchar(6) DEFAULT NULL,
  `iucret25` decimal(11,2) DEFAULT NULL,
  `igun26` decimal(11,2) DEFAULT NULL,
  `isaat26` varchar(6) DEFAULT NULL,
  `iucret26` decimal(11,2) DEFAULT NULL,
  `igun27` decimal(11,2) DEFAULT NULL,
  `isaat27` varchar(6) DEFAULT NULL,
  `iucret27` decimal(11,2) DEFAULT NULL,
  `igun28` decimal(11,2) DEFAULT NULL,
  `isaat28` varchar(6) DEFAULT NULL,
  `iucret28` decimal(11,2) DEFAULT NULL,
  `igun29` decimal(11,2) DEFAULT NULL,
  `isaat29` varchar(6) DEFAULT NULL,
  `iucret29` decimal(11,2) DEFAULT NULL,
  `igun30` decimal(11,2) DEFAULT NULL,
  `isaat30` varchar(6) DEFAULT NULL,
  `iucret30` decimal(11,2) DEFAULT NULL,
  `igun31` decimal(11,2) DEFAULT NULL,
  `isaat31` varchar(6) DEFAULT NULL,
  `iucret31` decimal(11,2) DEFAULT NULL,
  `igun32` decimal(11,2) DEFAULT NULL,
  `isaat32` varchar(6) DEFAULT NULL,
  `iucret32` decimal(11,2) DEFAULT NULL,
  `eucret1` decimal(11,2) DEFAULT NULL,
  `eucret2` decimal(11,2) DEFAULT NULL,
  `eucret3` decimal(11,2) DEFAULT NULL,
  `eucret4` decimal(11,2) DEFAULT NULL,
  `eucret5` decimal(11,2) DEFAULT NULL,
  `eucret6` decimal(11,2) DEFAULT NULL,
  `eucret7` decimal(11,2) DEFAULT NULL,
  `eucret8` decimal(11,2) DEFAULT NULL,
  `eucret9` decimal(11,2) DEFAULT NULL,
  `eucret10` decimal(11,2) DEFAULT NULL,
  `eucret11` decimal(11,2) DEFAULT NULL,
  `eucret12` decimal(11,2) DEFAULT NULL,
  `eucret13` decimal(11,2) DEFAULT NULL,
  `eucret14` decimal(11,2) DEFAULT NULL,
  `eucret15` decimal(11,2) DEFAULT NULL,
  `eucret16` decimal(11,2) DEFAULT NULL,
  `eucret17` decimal(11,2) DEFAULT NULL,
  `eucret18` decimal(11,2) DEFAULT NULL,
  `eucret19` decimal(11,2) DEFAULT NULL,
  `eucret20` decimal(11,2) DEFAULT NULL,
  `eucret21` decimal(11,2) DEFAULT NULL,
  `eucret22` decimal(11,2) DEFAULT NULL,
  `eucret23` decimal(11,2) DEFAULT NULL,
  `eucret24` decimal(11,2) DEFAULT NULL,
  `eucret25` decimal(11,2) DEFAULT NULL,
  `eucret26` decimal(11,2) DEFAULT NULL,
  `eucret27` decimal(11,2) DEFAULT NULL,
  `eucret28` decimal(11,2) DEFAULT NULL,
  `eucret29` decimal(11,2) DEFAULT NULL,
  `eucret30` decimal(11,2) DEFAULT NULL,
  `eucret31` decimal(11,2) DEFAULT NULL,
  `eucret32` decimal(11,2) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  `htkgun` decimal(11,2) DEFAULT NULL,
  `htksaat` varchar(6) DEFAULT NULL,
  `htkucret` decimal(11,2) DEFAULT NULL,
  `hitgun` decimal(11,2) DEFAULT NULL,
  `hitsaat` varchar(6) DEFAULT NULL,
  `hitucret` decimal(11,2) DEFAULT NULL,
  `donemfarkigun` decimal(11,2) DEFAULT NULL,
  `donemfarkisaat` varchar(6) DEFAULT NULL,
  `donemfarkiucret` decimal(11,2) DEFAULT NULL,
  `agiucret` decimal(11,2) DEFAULT NULL,
  `primucret` decimal(11,2) DEFAULT NULL,
  `yolucret` decimal(11,2) DEFAULT NULL,
  `yemekucret` decimal(11,2) DEFAULT NULL,
  `wcsaat` varchar(6) DEFAULT NULL,
  `wcucret` decimal(11,2) DEFAULT NULL,
  `nctoplam` decimal(11,2) DEFAULT NULL,
  `fmtoplam` decimal(11,2) DEFAULT NULL,
  `iztoplam` decimal(11,2) DEFAULT NULL,
  `kktoplam` decimal(11,2) DEFAULT NULL,
  `nchakedis` decimal(11,2) DEFAULT NULL COMMENT 'nctoplam + kktoplam',
  `fmhakedis` decimal(11,2) DEFAULT NULL,
  `ncodenen` decimal(11,2) DEFAULT NULL,
  `fmodenen` decimal(11,2) DEFAULT NULL,
  `nckalan` decimal(11,2) DEFAULT NULL,
  `fmkalan` decimal(11,2) DEFAULT NULL,
  `devir` decimal(11,2) DEFAULT NULL COMMENT 'devir=(maashakedis+mesaihakedis)-(maasodenen+mesaiodenen)  ->gecen ay icin',
  `geneltoplam` decimal(11,2) DEFAULT NULL COMMENT 'eneltoplam=(maashakedis+mesaihakedis+devir)-(maasodenen+mesaiodenen)  ->gecen ay icin',
  `kstsaat` varchar(6) DEFAULT NULL,
  `kstucret` decimal(11,2) DEFAULT NULL,
  `ozelucret` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_raporsablonlari`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_raporsablonlari` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` int(11) DEFAULT NULL,
  `ad` varchar(150) DEFAULT NULL,
  `nctoplam` tinyint(1) DEFAULT NULL,
  `fmtoplam` tinyint(1) DEFAULT NULL,
  `geneltoplam` tinyint(1) DEFAULT NULL,
  `raportip` varchar(20) DEFAULT NULL,
  `sicilno` tinyint(1) DEFAULT '0',
  `banka_hesapno` tinyint(1) DEFAULT '0',
  `ucret` tinyint(1) DEFAULT '0',
  `sgk_no` tinyint(1) DEFAULT '0',
  `imza` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_raporsablonlarialt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_raporsablonlarialt` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `sablonid` int(11) DEFAULT NULL,
  `tip` varchar(20) DEFAULT NULL,
  `alan` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `sablonid` (`sablonid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_resmitatiller`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_resmitatiller` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `tarih` date DEFAULT NULL,
  `tatil` varchar(100) DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `altgrupid` int(11) DEFAULT NULL,
  `puantajid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_tahakkuk`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_tahakkuk` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(10) NOT NULL,
  `tarih` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `devamsizlik` varchar(6) DEFAULT NULL,
  `geckalma` varchar(6) DEFAULT NULL,
  `erkencikma` varchar(6) DEFAULT NULL,
  `eksiksure` varchar(6) DEFAULT NULL,
  `devamsizlik_ceza` varchar(6) DEFAULT NULL,
  `geckalma_ceza` varchar(6) DEFAULT NULL,
  `erkencikma_ceza` varchar(6) DEFAULT NULL,
  `eksiksure_ceza` varchar(6) DEFAULT NULL,
  `c1` varchar(6) DEFAULT NULL,
  `c2` varchar(6) DEFAULT NULL,
  `c3` varchar(6) DEFAULT NULL,
  `c4` varchar(6) DEFAULT NULL,
  `c5` varchar(6) DEFAULT NULL,
  `c6` varchar(6) DEFAULT NULL,
  `c7` varchar(6) DEFAULT NULL,
  `c8` varchar(6) DEFAULT NULL,
  `c9` varchar(6) DEFAULT NULL,
  `c10` varchar(6) DEFAULT NULL,
  `c11` varchar(6) DEFAULT NULL,
  `c12` varchar(6) DEFAULT NULL,
  `c13` varchar(6) DEFAULT NULL,
  `c14` varchar(6) DEFAULT NULL,
  `c15` varchar(6) DEFAULT NULL,
  `c16` varchar(6) DEFAULT NULL,
  `c17` varchar(6) DEFAULT NULL,
  `c18` varchar(6) DEFAULT NULL,
  `c19` varchar(6) DEFAULT NULL,
  `c20` varchar(6) DEFAULT NULL,
  `c21` varchar(6) DEFAULT NULL,
  `c22` varchar(6) DEFAULT NULL,
  `c23` varchar(6) DEFAULT NULL,
  `c24` varchar(6) DEFAULT NULL,
  `c25` varchar(6) DEFAULT NULL,
  `c26` varchar(6) DEFAULT NULL,
  `c27` varchar(6) DEFAULT NULL,
  `c28` varchar(6) DEFAULT NULL,
  `c29` varchar(6) DEFAULT NULL,
  `c30` varchar(6) DEFAULT NULL,
  `c31` varchar(6) DEFAULT NULL,
  `c32` varchar(6) DEFAULT NULL,
  `i1` varchar(6) DEFAULT NULL,
  `i2` varchar(6) DEFAULT NULL,
  `i3` varchar(6) DEFAULT NULL,
  `i4` varchar(6) DEFAULT NULL,
  `i5` varchar(6) DEFAULT NULL,
  `i6` varchar(6) DEFAULT NULL,
  `i7` varchar(6) DEFAULT NULL,
  `i8` varchar(6) DEFAULT NULL,
  `i9` varchar(6) DEFAULT NULL,
  `i10` varchar(6) DEFAULT NULL,
  `i11` varchar(6) DEFAULT NULL,
  `i12` varchar(6) DEFAULT NULL,
  `i13` varchar(6) DEFAULT NULL,
  `i14` varchar(6) DEFAULT NULL,
  `i15` varchar(6) DEFAULT NULL,
  `i16` varchar(6) DEFAULT NULL,
  `i17` varchar(6) DEFAULT NULL,
  `i18` varchar(6) DEFAULT NULL,
  `i19` varchar(6) DEFAULT NULL,
  `i20` varchar(6) DEFAULT NULL,
  `i21` varchar(6) DEFAULT NULL,
  `i22` varchar(6) DEFAULT NULL,
  `i23` varchar(6) DEFAULT NULL,
  `i24` varchar(6) DEFAULT NULL,
  `i25` varchar(6) DEFAULT NULL,
  `i26` varchar(6) DEFAULT NULL,
  `i27` varchar(6) DEFAULT NULL,
  `i28` varchar(6) DEFAULT NULL,
  `i29` varchar(6) DEFAULT NULL,
  `i30` varchar(6) DEFAULT NULL,
  `i31` varchar(6) DEFAULT NULL,
  `i32` varchar(6) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  `puantajid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_takimplan`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_takimplan` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `takimkod` varchar(5) DEFAULT NULL,
  `grupid` int(11) DEFAULT NULL,
  `altgrupid` int(11) DEFAULT NULL,
  `puantajid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_tatilparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_tatilparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `puantajid` int(11) NOT NULL,
  `saat` time DEFAULT NULL,
  `tip` varchar(20) DEFAULT NULL,
  `calismaturid` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `puantajid` (`puantajid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_tempaktarim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_tempaktarim` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `kartno` varchar(10) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `trmno` varchar(10) DEFAULT NULL,
  `tus` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_tempaktarim2`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_tempaktarim2` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `kartno` varchar(10) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `trmno` varchar(10) DEFAULT NULL,
  `tus` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `pts_ziyaretci`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `pts_ziyaretci` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `kartfc` varchar(5) NOT NULL DEFAULT '',
  `kartno` varchar(10) NOT NULL DEFAULT '',
  `kartid` varchar(10) NOT NULL DEFAULT '',
  `ad` varchar(60) DEFAULT NULL,
  `soyad` varchar(60) DEFAULT NULL,
  `ceptel` varchar(30) DEFAULT NULL,
  `plaka` varchar(30) DEFAULT NULL,
  `firmaad` varchar(500) DEFAULT NULL,
  `aciklama` varchar(500) DEFAULT NULL,
  `personelturu` varchar(10) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `giristarihi` date DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `kartno` (`kartno`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `servis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `servis` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkbelgeturleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkbelgeturleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` smallint(6) DEFAULT NULL,
  `tur` varchar(10) DEFAULT NULL,
  `aciklama` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkbordroparametreleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkbordroparametreleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gecerlibaslangictarihi` date DEFAULT NULL,
  `yil` smallint(6) DEFAULT NULL,
  `donem` smallint(6) DEFAULT NULL,
  `sgkisci` decimal(11,5) DEFAULT NULL COMMENT 'SSK primi işçi payı',
  `sgkisveren` decimal(11,5) DEFAULT NULL COMMENT 'SSK Primi işveren payı',
  `issizlikisci` decimal(11,5) DEFAULT NULL COMMENT 'İşsizlik sigortası işçi payı',
  `issizlikisveren` decimal(11,5) DEFAULT NULL COMMENT 'İşsizlik sigortası işveren payı',
  `gelirvergisi` decimal(11,5) DEFAULT NULL,
  `damgavergisi` decimal(11,5) DEFAULT NULL,
  `tehlikesinifi` decimal(11,5) DEFAULT NULL,
  `brutucretkatsayisi` decimal(11,5) DEFAULT NULL,
  `sgkisci1` decimal(11,5) DEFAULT NULL COMMENT 'SSK primi işçi payı emekli',
  `sgkisveren1` decimal(11,5) DEFAULT NULL COMMENT 'SSK Primi işveren payı emekli',
  `issizlikisci1` decimal(11,5) DEFAULT NULL COMMENT 'İşsizlik sigortası işçi payı emekli',
  `issizlikisveren1` decimal(11,5) DEFAULT NULL COMMENT 'İşsizlik sigortası işveren payı emekli',
  `ozurlulukindirimi1` decimal(11,2) DEFAULT NULL,
  `ozurlulukindirimi2` decimal(11,2) DEFAULT NULL,
  `ozurlulukindirimi3` decimal(11,2) DEFAULT NULL,
  `agibruttutar16buyuk` decimal(11,2) DEFAULT NULL,
  `agibruttutar16kucuk` decimal(11,2) DEFAULT NULL,
  `agibruttutarustsinir` decimal(11,2) DEFAULT NULL,
  `agioranCalisan` decimal(11,2) DEFAULT NULL,
  `agioranEs` decimal(11,2) DEFAULT NULL,
  `agioran2cocuk` decimal(11,2) DEFAULT NULL,
  `agioran3cocuk` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkgelirvergisidilimleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkgelirvergisidilimleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gecerlibaslangictarihi` date DEFAULT NULL,
  `yil` smallint(6) DEFAULT NULL,
  `donem` smallint(6) DEFAULT NULL,
  `sira` int(11) DEFAULT NULL,
  `altsinir` double(15,5) DEFAULT NULL,
  `ustsinir` double(15,5) DEFAULT NULL,
  `oran` decimal(11,5) DEFAULT NULL,
  `tutar` double(15,5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkgelirvergisioranlari`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkgelirvergisioranlari` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `yil` smallint(6) DEFAULT NULL,
  `donem` smallint(6) DEFAULT NULL,
  `altsinir` double(15,5) DEFAULT NULL COMMENT 'SSK Primi işveren payı',
  `ustsinir` double(15,5) DEFAULT NULL COMMENT 'İşsizlik sigortası işçi payı',
  `oran` decimal(11,5) DEFAULT NULL COMMENT 'İşsizlik sigortası işveren payı',
  `tutar` double(20,5) DEFAULT NULL,
  `aciklama` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkihbarbordrosu`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkihbarbordrosu` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `yil` smallint(6) DEFAULT NULL,
  `ay` smallint(6) DEFAULT NULL,
  `istencikiskodu` varchar(5) DEFAULT NULL,
  `baslangictarihi` date DEFAULT NULL,
  `bitistarihi` date DEFAULT NULL,
  `toplamgun` int(11) DEFAULT NULL,
  `calisilanyil` int(11) DEFAULT NULL,
  `calisilangun` int(11) DEFAULT NULL,
  `calisilanay` int(11) DEFAULT NULL,
  `tavan` decimal(11,3) DEFAULT NULL COMMENT 'kidemtazminattavani ,ihbarda yok',
  `brutucret` decimal(11,3) DEFAULT NULL,
  `giydirilmisbrutucret` decimal(11,3) DEFAULT NULL COMMENT 'brutucret+yol+yemek+prim+ikramiye',
  `matrah` decimal(11,3) DEFAULT NULL COMMENT 'kidem/ihbar in brutu',
  `gelirvergisi` decimal(11,3) DEFAULT NULL,
  `damgavergisi` decimal(11,3) DEFAULT NULL,
  `net` decimal(11,3) DEFAULT NULL COMMENT 'kidem/ihbar in neti',
  PRIMARY KEY (`id`),
  KEY `Index 2` (`personelid`,`yil`,`ay`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkiscipayi`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkiscipayi` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `yil` smallint(6) DEFAULT NULL,
  `donem` smallint(6) DEFAULT NULL,
  `sigortakolu` varchar(200) DEFAULT NULL,
  `normal` decimal(11,5) DEFAULT NULL,
  `emekli` decimal(11,5) DEFAULT NULL,
  `cirak` decimal(11,5) DEFAULT NULL,
  `yabanci` decimal(11,5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkisverenpayi`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkisverenpayi` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `yil` smallint(6) DEFAULT NULL,
  `donem` smallint(6) DEFAULT NULL,
  `sigortakolu` varchar(200) DEFAULT NULL,
  `normal` decimal(11,5) DEFAULT NULL,
  `emekli` decimal(11,5) DEFAULT NULL,
  `cirak` decimal(11,5) DEFAULT NULL,
  `yabanci` decimal(11,5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkkanunlar`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkkanunlar` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `kod` smallint(6) DEFAULT NULL,
  `kanun` varchar(10) DEFAULT NULL,
  `aciklama` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkkidembordrosu`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkkidembordrosu` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `yil` smallint(6) DEFAULT NULL,
  `ay` smallint(6) DEFAULT NULL,
  `istencikiskodu` varchar(5) DEFAULT NULL,
  `baslangictarihi` date DEFAULT NULL,
  `bitistarihi` date DEFAULT NULL,
  `toplamgun` int(11) DEFAULT NULL,
  `calisilanyil` int(11) DEFAULT NULL,
  `calisilangun` int(11) DEFAULT NULL,
  `calisilanay` int(11) DEFAULT NULL,
  `tavan` decimal(11,3) DEFAULT NULL COMMENT 'kidemtazminattavani ,ihbarda yok',
  `brutucret` decimal(11,3) DEFAULT NULL,
  `giydirilmisbrutucret` decimal(11,3) DEFAULT NULL COMMENT 'brutucret+yol+yemek+prim+ikramiye',
  `matrah` decimal(11,3) DEFAULT NULL COMMENT 'kidem/ihbar in brutu',
  `gelirvergisi` decimal(11,3) DEFAULT NULL,
  `damgavergisi` decimal(11,3) DEFAULT NULL,
  `net` decimal(11,3) DEFAULT NULL COMMENT 'kidem/ihbar in neti',
  PRIMARY KEY (`id`),
  KEY `Index 2` (`personelid`,`yil`,`ay`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkrapor`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkrapor` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `tip` varchar(30) DEFAULT NULL,
  `alan` varchar(60) DEFAULT NULL,
  `gun` tinyint(1) DEFAULT '0' COMMENT 'sabitlerde sadece bu gecerli',
  `saat` tinyint(1) DEFAULT '0',
  `ucret` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkucretbordrosu`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkucretbordrosu` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `yil` smallint(6) DEFAULT NULL,
  `ay` smallint(6) DEFAULT NULL,
  `ngun1` decimal(11,2) DEFAULT NULL,
  `nsaat1` decimal(11,2) DEFAULT NULL,
  `nucret1` decimal(11,3) DEFAULT NULL,
  `ngun2` decimal(11,2) DEFAULT NULL,
  `nsaat2` decimal(11,2) DEFAULT NULL,
  `nucret2` decimal(11,3) DEFAULT NULL,
  `ngun3` decimal(11,2) DEFAULT NULL,
  `nsaat3` decimal(11,2) DEFAULT NULL,
  `nucret3` decimal(11,3) DEFAULT NULL,
  `ngun4` decimal(11,2) DEFAULT NULL,
  `nsaat4` decimal(11,2) DEFAULT NULL,
  `nucret4` decimal(11,3) DEFAULT NULL,
  `ngun5` decimal(11,2) DEFAULT NULL,
  `nsaat5` decimal(11,2) DEFAULT NULL,
  `nucret5` decimal(11,3) DEFAULT NULL,
  `ngun6` decimal(11,2) DEFAULT NULL,
  `nsaat6` decimal(11,2) DEFAULT NULL,
  `nucret6` decimal(11,3) DEFAULT NULL,
  `ngun7` decimal(11,2) DEFAULT NULL,
  `nsaat7` decimal(11,2) DEFAULT NULL,
  `nucret7` decimal(11,3) DEFAULT NULL,
  `ngun8` decimal(11,2) DEFAULT NULL,
  `nsaat8` decimal(11,2) DEFAULT NULL,
  `nucret8` decimal(11,3) DEFAULT NULL,
  `ngun9` decimal(11,2) DEFAULT NULL,
  `nsaat9` decimal(11,2) DEFAULT NULL,
  `nucret9` decimal(11,3) DEFAULT NULL,
  `ngun10` decimal(11,2) DEFAULT NULL,
  `nsaat10` decimal(11,2) DEFAULT NULL,
  `nucret10` decimal(11,3) DEFAULT NULL,
  `ngun11` decimal(11,2) DEFAULT NULL,
  `nsaat11` decimal(11,2) DEFAULT NULL,
  `nucret11` decimal(11,3) DEFAULT NULL,
  `ngun12` decimal(11,2) DEFAULT NULL,
  `nsaat12` decimal(11,2) DEFAULT NULL,
  `nucret12` decimal(11,3) DEFAULT NULL,
  `ngun13` decimal(11,2) DEFAULT NULL,
  `nsaat13` varchar(6) DEFAULT NULL,
  `nucret13` decimal(11,3) DEFAULT NULL,
  `ngun14` decimal(11,2) DEFAULT NULL,
  `nsaat14` decimal(11,2) DEFAULT NULL,
  `nucret14` decimal(11,3) DEFAULT NULL,
  `ngun15` decimal(11,2) DEFAULT NULL,
  `nsaat15` decimal(11,2) DEFAULT NULL,
  `nucret15` decimal(11,3) DEFAULT NULL,
  `ngun16` decimal(11,2) DEFAULT NULL,
  `nsaat16` decimal(11,2) DEFAULT NULL,
  `nucret16` decimal(11,3) DEFAULT NULL,
  `toplamngun` decimal(11,2) DEFAULT NULL,
  `toplamnsaat` decimal(11,2) DEFAULT NULL,
  `toplamnucret` decimal(11,3) DEFAULT NULL,
  `fsaat1` decimal(11,2) DEFAULT NULL,
  `fucret1` decimal(11,3) DEFAULT NULL,
  `fsaat2` decimal(11,2) DEFAULT NULL,
  `fucret2` decimal(11,3) DEFAULT NULL,
  `fsaat3` decimal(11,2) DEFAULT NULL,
  `fucret3` decimal(11,3) DEFAULT NULL,
  `fsaat4` decimal(11,2) DEFAULT NULL,
  `fucret4` decimal(11,3) DEFAULT NULL,
  `fsaat5` decimal(11,2) DEFAULT NULL,
  `fucret5` decimal(11,3) DEFAULT NULL,
  `fsaat6` decimal(11,2) DEFAULT NULL,
  `fucret6` decimal(11,3) DEFAULT NULL,
  `fsaat7` decimal(11,2) DEFAULT NULL,
  `fucret7` decimal(11,3) DEFAULT NULL,
  `fsaat8` decimal(11,2) DEFAULT NULL,
  `fucret8` decimal(11,3) DEFAULT NULL,
  `fsaat9` decimal(11,2) DEFAULT NULL,
  `fucret9` decimal(11,3) DEFAULT NULL,
  `fsaat10` decimal(11,2) DEFAULT NULL,
  `fucret10` decimal(11,3) DEFAULT NULL,
  `fsaat11` decimal(11,2) DEFAULT NULL,
  `fucret11` decimal(11,3) DEFAULT NULL,
  `fsaat12` decimal(11,2) DEFAULT NULL,
  `fucret12` decimal(11,3) DEFAULT NULL,
  `fsaat13` decimal(11,2) DEFAULT NULL,
  `fucret13` decimal(11,3) DEFAULT NULL,
  `fsaat14` decimal(11,2) DEFAULT NULL,
  `fucret14` decimal(11,3) DEFAULT NULL,
  `fsaat15` decimal(11,2) DEFAULT NULL,
  `fucret15` decimal(11,3) DEFAULT NULL,
  `fsaat16` decimal(11,2) DEFAULT NULL,
  `fucret16` decimal(11,3) DEFAULT NULL,
  `toplamfsaat` decimal(11,2) DEFAULT NULL,
  `toplamfucret` decimal(11,3) DEFAULT NULL,
  `egun1` decimal(11,2) DEFAULT NULL,
  `esaat1` decimal(11,2) DEFAULT NULL,
  `eucret1` decimal(11,3) DEFAULT NULL,
  `egun2` decimal(11,2) DEFAULT NULL,
  `esaat2` decimal(11,2) DEFAULT NULL,
  `eucret2` decimal(11,3) DEFAULT NULL,
  `egun3` decimal(11,2) DEFAULT NULL,
  `esaat3` decimal(11,2) DEFAULT NULL,
  `eucret3` decimal(11,3) DEFAULT NULL,
  `egun4` decimal(11,2) DEFAULT NULL,
  `esaat4` decimal(11,2) DEFAULT NULL,
  `eucret4` decimal(11,3) DEFAULT NULL,
  `egun5` decimal(11,2) DEFAULT NULL,
  `esaat5` decimal(11,2) DEFAULT NULL,
  `eucret5` decimal(11,3) DEFAULT NULL,
  `egun6` decimal(11,2) DEFAULT NULL,
  `esaat6` decimal(11,2) DEFAULT NULL,
  `eucret6` decimal(11,3) DEFAULT NULL,
  `egun7` decimal(11,2) DEFAULT NULL,
  `esaat7` decimal(11,2) DEFAULT NULL,
  `eucret7` decimal(11,3) DEFAULT NULL,
  `egun8` decimal(11,2) DEFAULT NULL,
  `esaat8` decimal(11,2) DEFAULT NULL,
  `eucret8` decimal(11,3) DEFAULT NULL,
  `egun9` decimal(11,2) DEFAULT NULL,
  `esaat9` decimal(11,2) DEFAULT NULL,
  `eucret9` decimal(11,3) DEFAULT NULL,
  `egun10` decimal(11,2) DEFAULT NULL,
  `esaat10` decimal(11,2) DEFAULT NULL,
  `eucret10` decimal(11,3) DEFAULT NULL,
  `egun11` decimal(11,2) DEFAULT NULL,
  `esaat11` decimal(11,2) DEFAULT NULL,
  `eucret11` decimal(11,3) DEFAULT NULL,
  `egun12` decimal(11,2) DEFAULT NULL,
  `esaat12` decimal(11,2) DEFAULT NULL,
  `eucret12` decimal(11,3) DEFAULT NULL,
  `egun13` decimal(11,2) DEFAULT NULL,
  `esaat13` varchar(6) DEFAULT NULL,
  `eucret13` decimal(11,3) DEFAULT NULL,
  `egun14` decimal(11,2) DEFAULT NULL,
  `esaat14` decimal(11,2) DEFAULT NULL,
  `eucret14` decimal(11,3) DEFAULT NULL,
  `egun15` decimal(11,2) DEFAULT NULL,
  `esaat15` decimal(11,2) DEFAULT NULL,
  `eucret15` decimal(11,3) DEFAULT NULL,
  `egun16` decimal(11,2) DEFAULT NULL,
  `esaat16` decimal(11,2) DEFAULT NULL,
  `eucret16` decimal(11,3) DEFAULT NULL,
  `toplamegun` decimal(11,2) DEFAULT NULL,
  `toplamesaat` decimal(11,2) DEFAULT NULL,
  `toplameucret` decimal(11,3) DEFAULT NULL,
  `ekodeme1` decimal(11,3) DEFAULT NULL,
  `ekodeme2` decimal(11,3) DEFAULT NULL,
  `ekodeme3` decimal(11,3) DEFAULT NULL,
  `ekodeme4` decimal(11,3) DEFAULT NULL,
  `ekodeme5` decimal(11,3) DEFAULT NULL,
  `ekodeme6` decimal(11,3) DEFAULT NULL,
  `ekodeme7` decimal(11,3) DEFAULT NULL,
  `ekodeme8` decimal(11,3) DEFAULT NULL,
  `ekodeme9` decimal(11,3) DEFAULT NULL,
  `ekodeme10` decimal(11,3) DEFAULT NULL,
  `ekodeme11` decimal(11,3) DEFAULT NULL,
  `ekodeme12` decimal(11,3) DEFAULT NULL,
  `ekodeme13` decimal(11,3) DEFAULT NULL,
  `ekodeme14` decimal(11,3) DEFAULT NULL,
  `ekodeme15` decimal(11,3) DEFAULT NULL,
  `ekodeme16` decimal(11,3) DEFAULT NULL,
  `toplamekodeme` decimal(11,3) DEFAULT NULL,
  `ekkesinti1` decimal(11,3) DEFAULT NULL,
  `ekkesinti2` decimal(11,3) DEFAULT NULL,
  `ekkesinti3` decimal(11,3) DEFAULT NULL,
  `ekkesinti4` decimal(11,3) DEFAULT NULL,
  `ekkesinti5` decimal(11,3) DEFAULT NULL,
  `ekkesinti6` decimal(11,3) DEFAULT NULL,
  `ekkesinti7` decimal(11,3) DEFAULT NULL,
  `ekkesinti8` decimal(11,3) DEFAULT NULL,
  `ekkesinti9` decimal(11,3) DEFAULT NULL,
  `ekkesinti10` decimal(11,3) DEFAULT NULL,
  `ekkesinti11` decimal(11,3) DEFAULT NULL,
  `ekkesinti12` decimal(11,3) DEFAULT NULL,
  `ekkesinti13` decimal(11,3) DEFAULT NULL,
  `ekkesinti14` decimal(11,3) DEFAULT NULL,
  `ekkesinti15` decimal(11,3) DEFAULT NULL,
  `ekkesinti16` decimal(11,3) DEFAULT NULL,
  `toplamekkesinti` decimal(11,3) DEFAULT NULL,
  `sgkgun` smallint(6) DEFAULT NULL,
  `toplamkazanc` decimal(11,3) DEFAULT NULL COMMENT '(SGKicin BRUTKAZANC)topnucret+topfucret+topekodeme',
  `myop_isci` decimal(11,3) DEFAULT NULL,
  `kvskp_isci` decimal(11,3) DEFAULT NULL,
  `gssp_isci` decimal(11,3) DEFAULT NULL,
  `sgdp_isci` decimal(11,3) DEFAULT NULL,
  `sgkisciprimi` decimal(11,3) DEFAULT NULL,
  `issizlikisciprimi` decimal(11,3) DEFAULT NULL,
  `sgkiscimatrahi` decimal(11,3) DEFAULT NULL,
  `myop_isveren` decimal(11,3) DEFAULT NULL,
  `kvskp_isveren` decimal(11,3) DEFAULT NULL,
  `gssp_isveren` decimal(11,3) DEFAULT NULL,
  `sgdp_isveren` decimal(11,3) DEFAULT NULL,
  `sgkisverenprimi` decimal(11,3) DEFAULT NULL,
  `issizlikisverenprimi` decimal(11,3) DEFAULT NULL,
  `sgkisverenmatrahi` decimal(11,3) DEFAULT NULL COMMENT 'sgkmatrahi=pek',
  `gelirvergisimatrahi` decimal(11,3) DEFAULT NULL,
  `agioncesigelirvergisi` decimal(11,3) DEFAULT NULL,
  `gelirvergisi` decimal(11,3) DEFAULT NULL,
  `kumulatifgelirvergisimatrahi` decimal(11,3) DEFAULT NULL,
  `damgavergisimatrahi` decimal(11,3) DEFAULT NULL,
  `damgavergisi` decimal(11,3) DEFAULT NULL,
  `toplamyasalkesinti` decimal(11,3) DEFAULT NULL,
  `toplamkesinti` decimal(11,3) DEFAULT NULL COMMENT 'toplamyasalkesinti+toplamekkesinti',
  `devir` decimal(11,3) DEFAULT NULL,
  `netucret` decimal(11,3) DEFAULT NULL,
  `agi` decimal(11,3) DEFAULT NULL,
  `yuvarlamafarki` decimal(11,3) DEFAULT NULL,
  `netodenen` decimal(11,3) DEFAULT NULL,
  `prim_ikramiye` decimal(11,3) DEFAULT NULL,
  `eksikgun` int(11) DEFAULT NULL COMMENT 'eksikgun=toplamegun',
  `eksikneden` varchar(10) DEFAULT NULL,
  `gvdilimi` varchar(30) DEFAULT NULL,
  `aciklama` varchar(150) DEFAULT NULL,
  `terkin1` decimal(11,3) DEFAULT NULL,
  `terkin2` decimal(11,3) DEFAULT NULL,
  `terkin3` decimal(11,3) DEFAULT NULL,
  `terkin4` decimal(11,3) DEFAULT NULL,
  `terkin5` decimal(11,3) DEFAULT NULL,
  `terkin6` decimal(11,3) DEFAULT NULL,
  `terkin7` decimal(11,3) DEFAULT NULL,
  `terkin8` decimal(11,3) DEFAULT NULL,
  `terkin9` decimal(11,3) DEFAULT NULL,
  `terkin10` decimal(11,3) DEFAULT NULL,
  `terkin11` decimal(11,3) DEFAULT NULL,
  `terkin12` decimal(11,3) DEFAULT NULL,
  `terkin13` decimal(11,3) DEFAULT NULL,
  `terkin14` decimal(11,3) DEFAULT NULL,
  `terkin15` decimal(11,3) DEFAULT NULL,
  `terkin16` decimal(11,3) DEFAULT NULL,
  `terkin17` decimal(11,3) DEFAULT NULL,
  `terkin18` decimal(11,3) DEFAULT NULL,
  `terkin19` decimal(11,3) DEFAULT NULL,
  `terkin20` decimal(11,3) DEFAULT NULL,
  `bucret` decimal(11,2) DEFAULT NULL,
  `nucret` decimal(11,2) DEFAULT NULL,
  `nucretn1` decimal(11,3) DEFAULT NULL,
  `nucretn2` decimal(11,3) DEFAULT NULL,
  `nucretn3` decimal(11,3) DEFAULT NULL,
  `nucretn4` decimal(11,3) DEFAULT NULL,
  `nucretn5` decimal(11,3) DEFAULT NULL,
  `nucretn6` decimal(11,3) DEFAULT NULL,
  `nucretn7` decimal(11,3) DEFAULT NULL,
  `nucretn8` decimal(11,3) DEFAULT NULL,
  `nucretn9` decimal(11,3) DEFAULT NULL,
  `nucretn10` decimal(11,3) DEFAULT NULL,
  `nucretn11` decimal(11,3) DEFAULT NULL,
  `nucretn12` decimal(11,3) DEFAULT NULL,
  `nucretn13` decimal(11,3) DEFAULT NULL,
  `nucretn14` decimal(11,3) DEFAULT NULL,
  `nucretn15` decimal(11,3) DEFAULT NULL,
  `nucretn16` decimal(11,3) DEFAULT NULL,
  `fucretn1` decimal(11,3) DEFAULT NULL,
  `fucretn2` decimal(11,3) DEFAULT NULL,
  `fucretn3` decimal(11,3) DEFAULT NULL,
  `fucretn4` decimal(11,3) DEFAULT NULL,
  `fucretn5` decimal(11,3) DEFAULT NULL,
  `fucretn6` decimal(11,3) DEFAULT NULL,
  `fucretn7` decimal(11,3) DEFAULT NULL,
  `fucretn8` decimal(11,3) DEFAULT NULL,
  `fucretn9` decimal(11,3) DEFAULT NULL,
  `fucretn10` decimal(11,3) DEFAULT NULL,
  `fucretn11` decimal(11,3) DEFAULT NULL,
  `fucretn12` decimal(11,3) DEFAULT NULL,
  `fucretn13` decimal(11,3) DEFAULT NULL,
  `fucretn14` decimal(11,3) DEFAULT NULL,
  `fucretn15` decimal(11,3) DEFAULT NULL,
  `fucretn16` decimal(11,3) DEFAULT NULL,
  `ekodemen1` decimal(11,3) DEFAULT NULL,
  `ekodemen2` decimal(11,3) DEFAULT NULL,
  `ekodemen3` decimal(11,3) DEFAULT NULL,
  `ekodemen4` decimal(11,3) DEFAULT NULL,
  `ekodemen5` decimal(11,3) DEFAULT NULL,
  `ekodemen6` decimal(11,3) DEFAULT NULL,
  `ekodemen7` decimal(11,3) DEFAULT NULL,
  `ekodemen8` decimal(11,3) DEFAULT NULL,
  `ekodemen9` decimal(11,3) DEFAULT NULL,
  `ekodemen10` decimal(11,3) DEFAULT NULL,
  `ekodemen11` decimal(11,3) DEFAULT NULL,
  `ekodemen12` decimal(11,3) DEFAULT NULL,
  `ekodemen13` decimal(11,3) DEFAULT NULL,
  `ekodemen14` decimal(11,3) DEFAULT NULL,
  `ekodemen15` decimal(11,3) DEFAULT NULL,
  `ekodemen16` decimal(11,3) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `Index 2` (`personelid`,`yil`,`ay`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sgkyasalparametreler`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sgkyasalparametreler` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `yil` int(11) DEFAULT NULL,
  `donem` smallint(6) DEFAULT NULL COMMENT 'asgariucretbruttutar',
  `asgariucret` decimal(11,3) DEFAULT '0.000' COMMENT '1.donem',
  `sgktabanmatrahi` decimal(11,3) DEFAULT '0.000' COMMENT '1.donem',
  `sgktavanmatrahi` decimal(11,3) DEFAULT '0.000' COMMENT '1.donem',
  `damgavergisiorani` decimal(11,3) DEFAULT '0.000',
  `aylikbildirgedamgavergisitutari` decimal(11,3) DEFAULT '0.000',
  `kidemtazminatitavani` decimal(11,3) DEFAULT '0.000',
  `yemekmuafiyetisgk` decimal(11,3) DEFAULT '0.000',
  `yemekmuafiyetigv` decimal(11,3) DEFAULT '0.000',
  `cocukmuafiyetisgk` decimal(11,3) DEFAULT '0.000',
  `cocukmuafiyetigv` decimal(11,3) DEFAULT '0.000',
  `ailemuafiyetisgk` decimal(11,3) DEFAULT '0.000',
  `sakatlikindirimi1` decimal(11,3) DEFAULT '0.000',
  `sakatlikindirimi2` decimal(11,3) DEFAULT '0.000',
  `sakatlikindirimi3` decimal(11,3) DEFAULT '0.000',
  `agibruttutar` decimal(11,3) DEFAULT '0.000',
  `agibruttutarustsinir` decimal(11,3) DEFAULT '0.000',
  `agioranKendisi` decimal(11,3) DEFAULT '0.000',
  `agioranEsi` decimal(11,3) DEFAULT '0.000',
  `agioran1cocuk` decimal(11,3) DEFAULT '0.000',
  `agioran2cocuk` decimal(11,3) DEFAULT '0.000',
  `agioranDiger` decimal(11,3) DEFAULT '0.000',
  `ogrenci19alti` decimal(11,3) DEFAULT '0.000',
  `ogrenci20ustu` decimal(11,3) DEFAULT '0.000',
  `cirak16yasalti` decimal(11,3) DEFAULT '0.000',
  `cirak16yasustu` decimal(11,3) DEFAULT '0.000',
  `agioran3cocuk` decimal(11,3) DEFAULT '0.000',
  `agioran4cocuk` decimal(11,3) DEFAULT '0.000',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `smsliste`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `smsliste` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `kartno` varchar(10) DEFAULT NULL,
  `isim` varchar(160) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `durum` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `smsmesajlar`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `smsmesajlar` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `mesaj` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `sube`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `sube` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  `adres` varchar(300) DEFAULT NULL,
  `sicilno` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `takim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `takim` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `temp_uye_giris`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `temp_uye_giris` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) DEFAULT NULL,
  `kartno` varchar(5) DEFAULT NULL,
  `isim` varchar(100) DEFAULT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `ip` varchar(16) DEFAULT NULL,
  `kapino` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `temp_yillikizin`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `temp_yillikizin` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `isim` varchar(60) DEFAULT NULL,
  `isegiristarihi` date DEFAULT NULL,
  `toplamizinhakki` varchar(5) DEFAULT NULL,
  `ocak` varchar(5) DEFAULT NULL,
  `subat` varchar(5) DEFAULT NULL,
  `mart` varchar(5) DEFAULT NULL,
  `nisan` varchar(5) DEFAULT NULL,
  `mayis` varchar(5) DEFAULT NULL,
  `haziran` varchar(5) DEFAULT NULL,
  `temmuz` varchar(5) DEFAULT NULL,
  `agustos` varchar(5) DEFAULT NULL,
  `eylul` varchar(5) DEFAULT NULL,
  `ekim` varchar(5) DEFAULT NULL,
  `kasim` varchar(5) DEFAULT NULL,
  `aralik` varchar(5) DEFAULT NULL,
  `toplamkullanilan` varchar(5) DEFAULT NULL,
  `toplamkalan` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `temponline`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `temponline` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `kartno` varchar(10) DEFAULT NULL,
  `isim` varchar(160) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  `kapiterminalkod` varchar(10) DEFAULT NULL,
  `kapiterminalad` varchar(200) DEFAULT NULL,
  `girisyetkiliid` bigint(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `terminal`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `terminal` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `tolerans` int(11) DEFAULT NULL,
  `tip` varchar(6) DEFAULT NULL,
  `programpath` longtext,
  `aktarimpath` longtext,
  `kartnobas` varchar(3) DEFAULT NULL,
  `kartnohane` varchar(3) DEFAULT NULL,
  `yilbas` varchar(3) DEFAULT NULL,
  `yilhane` varchar(3) DEFAULT NULL,
  `aybas` varchar(3) DEFAULT NULL,
  `ayhane` varchar(3) DEFAULT NULL,
  `gunbas` varchar(3) DEFAULT NULL,
  `gunhane` varchar(3) DEFAULT NULL,
  `saatbas` varchar(3) DEFAULT NULL,
  `saathane` varchar(3) DEFAULT NULL,
  `dakikabas` varchar(3) DEFAULT NULL,
  `dakikahane` varchar(3) DEFAULT NULL,
  `saniyebas` varchar(3) DEFAULT NULL,
  `saniyehane` varchar(3) DEFAULT NULL,
  `tusbas` varchar(3) DEFAULT NULL,
  `tushane` varchar(3) DEFAULT NULL,
  `hesaplamatipi` varchar(6) DEFAULT NULL,
  `wcyenikayittolerans` varchar(6) DEFAULT NULL,
  `grupkodubas` varchar(3) DEFAULT NULL,
  `grupkoduhane` varchar(3) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `terminaller`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `terminaller` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `Source` varchar(20) DEFAULT NULL,
  `Baudrate_Remote_Port` int(11) DEFAULT NULL,
  `Device_Num` int(11) DEFAULT NULL,
  `CommPassword` varchar(10) DEFAULT NULL,
  `modultipi` varchar(6) DEFAULT NULL,
  `cihaztipi` varchar(15) DEFAULT NULL,
  `aktarimtipi` varchar(5) DEFAULT NULL,
  `serino` varchar(20) DEFAULT NULL,
  `kod` varchar(3) DEFAULT NULL,
  `tip` varchar(10) DEFAULT NULL,
  `secim` tinyint(1) DEFAULT '1',
  `anamakina` tinyint(1) DEFAULT '0' COMMENT 'prmk aktarimi icin',
  `online` tinyint(1) DEFAULT '0' COMMENT 'prmk aktarimi icin',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `terminalotosaat`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `terminalotosaat` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `saat` varchar(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `terminalparmaksaat`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `terminalparmaksaat` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `saat` varchar(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `terminalpuantajsaat`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `terminalpuantajsaat` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `saat` varchar(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `terminaltuslu`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `terminaltuslu` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `terminalid` int(11) NOT NULL,
  `tus` varchar(10) DEFAULT NULL,
  `tustipi` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `terminalid` (`terminalid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
;

--
-- Table structure for table `textaktarim`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `textaktarim` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `programtipi` varchar(10) DEFAULT NULL,
  `pertipi` varchar(10) DEFAULT NULL,
  `saattipi` varchar(10) DEFAULT NULL,
  `textdosya` longtext,
  `varsayilan` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `textaktarimalt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `textaktarimalt` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `textaktarimid` int(11) DEFAULT NULL,
  `bordrokod` smallint(6) DEFAULT NULL,
  `bordroad` varchar(20) DEFAULT NULL,
  `bordrotip` varchar(10) DEFAULT NULL,
  `gun` varchar(5) DEFAULT NULL,
  `saat` varchar(5) DEFAULT NULL,
  `ucret` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_bolum`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_bolum` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_dondurma`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_dondurma` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `baslamatarihi` date DEFAULT NULL,
  `bitistarihi` date DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_giriscikis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_giriscikis` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `toplamsure` varchar(15) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `izintipi` varchar(10) DEFAULT NULL,
  `izinturid` int(11) DEFAULT NULL,
  `izinsure` varchar(6) DEFAULT NULL,
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  `aciklama` varchar(250) DEFAULT NULL,
  `geciptal` tinyint(1) DEFAULT '0',
  `erkiptal` tinyint(1) DEFAULT '0',
  `eksiptal` tinyint(1) DEFAULT '0',
  `surefarki` varchar(6) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT '0',
  `tur` smallint(6) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`giristarihi`,`girissaati`,`cikistarihi`,`cikissaati`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_grup`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_grup` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_iliskilendirme`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_iliskilendirme` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `personelid1` bigint(20) NOT NULL,
  `isim` varchar(200) DEFAULT NULL,
  `yakinlikderecesi` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_odeme`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_odeme` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `tutar` decimal(11,2) DEFAULT NULL,
  `aciklama` varchar(200) DEFAULT NULL,
  `odemetipi` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_taksit`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_taksit` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `tutar` decimal(11,2) DEFAULT NULL,
  `aciklama` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_terminal`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_terminal` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `Source` varchar(20) DEFAULT NULL,
  `Baudrate_Remote_Port` int(11) DEFAULT NULL,
  `Device_Num` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_tipi`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_tipi` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ad` varchar(200) DEFAULT NULL,
  `kod` smallint(6) DEFAULT NULL,
  `gunlukgecis` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `uye_tipialt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `uye_tipialt` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uyetipiid` int(11) NOT NULL,
  `baslamasaati` varchar(5) DEFAULT NULL,
  `bitissaati` varchar(5) DEFAULT NULL,
  `pzt` tinyint(1) DEFAULT '0',
  `sal` tinyint(1) DEFAULT '0',
  `car` tinyint(1) DEFAULT '0',
  `per` tinyint(1) DEFAULT '0',
  `cum` tinyint(1) DEFAULT '0',
  `cmt` tinyint(1) DEFAULT '0',
  `paz` tinyint(1) DEFAULT '0',
  `gunlukgecis` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tipid` (`uyetipiid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_ayarlar`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_ayarlar` (
  `grupid` int(11) NOT NULL,
  `altgrupid` int(11) DEFAULT NULL,
  `toleranstipi` varchar(10) DEFAULT NULL,
  `tutartipi` varchar(10) DEFAULT NULL,
  `gunluksure` varchar(6) DEFAULT NULL,
  `gunlukgecis` int(11) DEFAULT NULL,
  `sabittutar` decimal(11,2) DEFAULT NULL,
  `ayliksure` varchar(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_giriscikis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_giriscikis` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `surefarki` varchar(6) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT NULL,
  `kapiterminalkod` varchar(10) DEFAULT NULL,
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_mola`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_mola` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `baslamasaati` varchar(5) DEFAULT NULL,
  `bitissaati` varchar(5) DEFAULT NULL,
  `g1` tinyint(1) DEFAULT '1',
  `g2` tinyint(1) DEFAULT '1',
  `g3` tinyint(1) DEFAULT '1',
  `g4` tinyint(1) DEFAULT '1',
  `g5` tinyint(1) DEFAULT '1',
  `g6` tinyint(1) DEFAULT '1',
  `g7` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_molabolum`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_molabolum` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `molaid` int(11) DEFAULT NULL,
  `bolumid` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_molaparametre`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_molaparametre` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `grupid` int(11) NOT NULL,
  `altgrupid` int(11) DEFAULT NULL,
  `ad` varchar(100) DEFAULT NULL,
  `baslamasaati` varchar(5) DEFAULT NULL,
  `bitissaati` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_puantaj`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_puantaj` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `kullanilansaat` varchar(6) DEFAULT NULL,
  `kesintisaat` varchar(6) DEFAULT NULL,
  `kesintitutar` decimal(11,2) DEFAULT NULL,
  `ikesintisaat` decimal(11,2) DEFAULT NULL,
  `ikullanilansaat` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wc_puantajtoplam`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wc_puantajtoplam` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `tarih` date DEFAULT NULL,
  `kullanilansaat` varchar(6) DEFAULT NULL,
  `kesintisaat` varchar(6) DEFAULT NULL,
  `kesintitutar` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `wciptalsaatleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `wciptalsaatleri` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `baslamasaati` time DEFAULT NULL,
  `bitissaati` time DEFAULT NULL,
  `secim` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `yetki`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `yetki` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ad` varchar(200) DEFAULT NULL,
  `gunlukgecis` int(11) DEFAULT NULL,
  `kod` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `yetki_saatleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `yetki_saatleri` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `yetkiid` int(11) NOT NULL,
  `baslamasaati` time DEFAULT NULL,
  `bitissaati` time DEFAULT NULL,
  `gunlukgecis` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `yillikizineklenen`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `yillikizineklenen` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) DEFAULT NULL,
  `izinturid` int(11) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `eklenen` int(11) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `yillikizinkalan`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `yillikizinkalan` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) DEFAULT NULL,
  `yil` int(11) DEFAULT NULL,
  `kalan` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `yillikizinsureleri`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `yillikizinsureleri` (
  `yilbas` int(11) DEFAULT NULL,
  `yilbit` int(11) DEFAULT NULL,
  `hak` int(11) DEFAULT NULL,
  `aciklama` varchar(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `ymk_giris`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `ymk_giris` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) DEFAULT NULL,
  `isim` varchar(100) DEFAULT NULL,
  `tarih` date DEFAULT NULL,
  `saat` time DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`personelid`,`tarih`,`saat`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `ymk_giriscikis`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `ymk_giriscikis` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `personelid` bigint(20) NOT NULL,
  `giristarihi` date DEFAULT NULL,
  `girissaati` time DEFAULT NULL,
  `cikistarihi` date DEFAULT NULL,
  `cikissaati` time DEFAULT NULL,
  `surefarki` varchar(6) DEFAULT NULL,
  `ellegiris` varchar(1) DEFAULT NULL,
  `ellecikis` varchar(1) DEFAULT NULL,
  `aciklama` varchar(60) DEFAULT NULL,
  `islemturu` varchar(2) DEFAULT NULL,
  `kapiterminalkod` varchar(10) DEFAULT NULL,
  `isurefarki` int(11) DEFAULT '0',
  `giristerminal` varchar(60) DEFAULT NULL,
  `cikisterminal` varchar(60) DEFAULT NULL,
  `giristerminalno` varchar(5) DEFAULT NULL,
  `cikisterminalno` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `ymk_ogun`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `ymk_ogun` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  `baslamasaati` time DEFAULT NULL,
  `bitissaati` time DEFAULT NULL,
  `tutar` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `personelid` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `zamanbolge`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `zamanbolge` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `zamanbolgealt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `zamanbolgealt` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `zamanbolgeid` bigint(20) unsigned NOT NULL,
  `g1` int(11) DEFAULT NULL,
  `g2` int(11) DEFAULT NULL,
  `g3` int(11) DEFAULT NULL,
  `g4` int(11) DEFAULT NULL,
  `g5` int(11) DEFAULT NULL,
  `g6` int(11) DEFAULT NULL,
  `g7` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `zamankod`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `zamankod` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `ad` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `zamankodalt`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `zamankodalt` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `zamankodid` bigint(20) unsigned NOT NULL,
  `saat1` varchar(5) DEFAULT NULL,
  `saat2` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `zimmetbolum`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `zimmetbolum` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;

--
-- Table structure for table `zimmetkategori`
-- Created with MySQL Version 5.1.44
--

CREATE TABLE `zimmetkategori` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `kod` varchar(10) DEFAULT NULL,
  `ad` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin5;
;
