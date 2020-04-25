CREATE TABLE "Cheque" (
	"Id"	INTEGER NOT NULL,
	"ChequeID"	TEXT NOT NULL UNIQUE,
	"ChequeDate"	TEXT,
	"Atm"	TEXT,
	"Status" TEXT NOT NULL,
	"MetadataFile"	TEXT,
	"FrontImageFile"	TEXT,
	"RearImageFile"	TEXT,
	"CreationDate"	TEXT NOT NULL,
	"LastUpdateDate"	TEXT NOT NULL,
	"IsDeleted"	INTEGER NOT NULL,
	PRIMARY KEY("Id")
)
