USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FCG')
BEGIN
    CREATE DATABASE FCG;
    PRINT 'Database FCG created successfully.';
END
ELSE
BEGIN
    PRINT 'Database FCG already exists.';
END
GO

USE FCG;
GO

-- Criar a tabela Usuario baseada na migration inicial
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuario' AND xtype='U')
BEGIN
    CREATE TABLE Usuario (
        Id int IDENTITY(1,1) NOT NULL,
        Nome nvarchar(200) NOT NULL,
        Email nvarchar(250) NOT NULL,
        IsAdmin bit NOT NULL,
        PasswordHash varbinary(max) NULL,
        PasswordSalt varbinary(max) NULL,
        CONSTRAINT PK_Usuario PRIMARY KEY (Id)
    );
    
    PRINT 'Table Usuario created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Usuario already exists.';
END
GO

-- Criar índice no campo Email para performance
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Usuario_Email')
BEGIN
    CREATE UNIQUE INDEX IX_Usuario_Email ON Usuario(Email);
    PRINT 'Index IX_Usuario_Email created successfully.';
END
GO

PRINT 'Database initialization completed successfully!';
PRINT 'Database structure created. Create users via API: POST /api/Usuario/Incluir';
PRINT 'You can now access the API at http://localhost:8080/swagger';
GO 