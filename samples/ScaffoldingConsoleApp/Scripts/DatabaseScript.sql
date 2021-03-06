﻿/******************************************************************************/
/***         Generated by IBExpert 2013.2.20.1 29/08/2017 12:24:01          ***/
/******************************************************************************/

SET SQL DIALECT 3;

SET NAMES UTF8;

CREATE DATABASE '127.0.0.1:C:\ScaffoldTestDb.fdb'
USER 'SYSDBA' PASSWORD 'masterkey'
PAGE_SIZE 16384
DEFAULT CHARACTER SET UTF8 COLLATION UTF8;

/******************************************************************************/
/***                               Generators                               ***/
/******************************************************************************/

CREATE GENERATOR GEN_SAMPLETABLE1_ID;
SET GENERATOR GEN_SAMPLETABLE1_ID TO 0;

/******************************************************************************/
/***                                 Tables                                 ***/
/******************************************************************************/

CREATE TABLE SAMPLETABLE1 (
    ID        INTEGER NOT NULL,
    FIELDINT  INTEGER,
    FIELDSTR  VARCHAR(255)
);

CREATE TABLE SAMPLETABLE2 (
    ID              INTEGER NOT NULL,
    SAMPLETABLE1ID  INTEGER NOT NULL,
    FIELDSMALLINT   SMALLINT,
	FIELDBIGINT     BIGINT DEFAULT 42,
    FIELDNUM        NUMERIC(18,4),
    FIELDMEMO       BLOB SUB_TYPE 1 SEGMENT SIZE 80,
    FIELDBLOB       BLOB SUB_TYPE 0 SEGMENT SIZE 80,
    TYPETIMESTAMP   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FIELDDATE       DATE,
    FIELDTIME       TIME,
    FIELDGUID       CHAR(36)
);

/******************************************************************************/
/***                              Primary Keys                              ***/
/******************************************************************************/

ALTER TABLE SAMPLETABLE1 ADD CONSTRAINT PK_SAMPLETABLE1 PRIMARY KEY (ID);
ALTER TABLE SAMPLETABLE2 ADD CONSTRAINT PK_SAMPLETABLE2 PRIMARY KEY (ID);

/******************************************************************************/
/***                              Foreign Keys                              ***/
/******************************************************************************/

ALTER TABLE SAMPLETABLE2 ADD CONSTRAINT FK_SAMPLETABLE2_1 FOREIGN KEY (SAMPLETABLE1ID) REFERENCES SAMPLETABLE1 (ID) ON DELETE CASCADE ON UPDATE CASCADE;

/******************************************************************************/
/***                                Triggers                                ***/
/******************************************************************************/

SET TERM ^ ;

/******************************************************************************/
/***                          Triggers for tables                           ***/
/******************************************************************************/

/* Trigger: SAMPLETABLE1_BI */
CREATE TRIGGER SAMPLETABLE1_BI FOR SAMPLETABLE1
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (new.id is null) then
    new.id = gen_id(gen_sampletable1_id,1);
end
^

SET TERM ; ^
