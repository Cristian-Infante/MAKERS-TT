CREATE TABLE Users (
    Id            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Email         NVARCHAR(256)    NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(512)    NOT NULL,
    FullName      NVARCHAR(256)    NOT NULL,
    Role          NVARCHAR(50)     NOT NULL,
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    DeletedAt     DATETIME2        NULL
);

CREATE TABLE Loans (
    Id               UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    UserId           UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id),
    Amount           DECIMAL(18,2)    NOT NULL,
    TermInMonths     INT              NOT NULL,
    Purpose          NVARCHAR(500)    NOT NULL,
    Status           NVARCHAR(50)     NOT NULL DEFAULT 'Pending',
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    ApprovedAt       DATETIME2        NULL,
    RejectedAt       DATETIME2        NULL,
    RejectionReason  NVARCHAR(1000)   NULL,
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    DeletedAt        DATETIME2        NULL
);
