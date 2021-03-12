CREATE PROC dbo.City_GetList -- ქალაქების სიის წამოღება
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    c.ID
   ,c.CityName
  FROM dic.City c

  ORDER BY c.CityName

END
GO

CREATE PROCEDURE dbo.City_Post -- ქალაქის დამატება
@CityName NVARCHAR(200),
@CityOutputID INT = NULL OUTPUT

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO dic.City (CityName)
      VALUES (@CityName);

    SET @CityOutputID = SCOPE_IDENTITY()
    COMMIT TRANSACTION;


    SELECT
      @CityOutputID
    RETURN @CityOutputID

  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.City_Put -- ქალაქის რედაქტირება
@CityID INT,
@CityName NVARCHAR(200)

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dic.City
    SET CityName = @CityName
    WHERE ID = @CityID

  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.PhoneNumberType_Put -- ტელეფონის ნომრის ტიპის რედაქტირება
@TypeID INT,
@TypeName NVARCHAR(100)

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dic.PhoneNumberType
    SET TypeName = @TypeName
    WHERE ID = @TypeID;

  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.PhoneNumberType_Post -- ტელეფონის ნომრის ტიპის დამატება
@TypeName NVARCHAR(100),
@PhoneNumberOutputID INT = NULL OUTPUT

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO dic.PhoneNumberType (TypeName)
      VALUES (@TypeName);

    SET @PhoneNumberOutputID = SCOPE_IDENTITY()
    COMMIT TRANSACTION;

    SELECT
      @PhoneNumberOutputID
    RETURN @PhoneNumberOutputID

  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROC dbo.PhoneNumberType_GetList -- ტელეფონის ნომრის ტიპების წამოღება
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    pnt.ID
   ,pnt.TypeName
  FROM dic.PhoneNumberType pnt
  ORDER BY pnt.ID

END
GO

CREATE PROC dbo.Gender_GetList -- სქესის წამოღება
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    g.ID
   ,g.GenderName
  FROM dic.Gender g
  ORDER BY g.ID

END
GO

CREATE PROCEDURE dbo.PhoneNumbers_Put -- ტელეფონის ნომრების რედაქტირება
@PhoneNumbersList AS dbo.PhoneNumberList READONLY,
@ContactID AS INT

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE pn
    SET pn.PhoneNumberTypeID = pnl.PhoneNumberTypeID
       ,pn.PhoneNumber = pnl.PhoneNumber
    FROM dbo.PhoneNumber pn
    JOIN @PhoneNumbersList pnl
      ON pn.ID = pnl.ID
      AND pn.ContactID = @ContactID

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.PhoneNumbers_Post -- ტელეფონის ნომრების დამატება
@PhoneNumbersList AS dbo.PhoneNumberList READONLY,
@ContactID AS INT

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO dbo.PhoneNumber (PhoneNumberTypeID, ContactID, PhoneNumber)
      SELECT
        PhoneNumberTypeID
       ,@ContactID
       ,PhoneNumber
      FROM @PhoneNumbersList

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROC dbo.PhoneNumbers_GetList_ByContactID -- ტელეფონის ნომრები სიის წამოღება კონტაქტის მიხედვით
@ContactID INT
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    pn.ID
   ,pn.PhoneNumberTypeID
   ,pn.PhoneNumber
  FROM dbo.PhoneNumber pn
  WHERE pn.ContactID = @ContactID

END
GO

CREATE PROCEDURE dbo.Contact_AddFavorite -- მონიშნული კონტაქტების ფავორიტებში დამატება
@IDList AS dbo.IDList READONLY

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.Contact
    SET IsFavorite = 1
       ,DataModified = GETDATE()
    WHERE ID IN (SELECT
        i.ID
      FROM @IDList i)

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.Contact_Delete -- მონიშნული კონტაქტების წაშლა
@IDList AS dbo.IDList READONLY

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.Contact
    SET IsDeleted = 1
       ,DataModified = GETDATE()
    WHERE ID IN (SELECT
        i.ID
      FROM @IDList i)

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.Contact_Deleted_GetList -- მხოლოდ წაშლილი კონტაქტების სიის წამოღება
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    co.ID AS ContactID
   ,co.Firstname + ' ' + co.Lastname AS Fullname
   ,SUBSTRING((SELECT
        ', ' + pnt.TypeName + ': ' + pn1.PhoneNumber
      FROM dbo.PhoneNumber pn1
      LEFT JOIN dic.PhoneNumberType pnt
        ON pn1.PhoneNumberTypeID = pnt.ID
      WHERE pn1.ID IN (SELECT
          pn2.ID
        FROM dbo.PhoneNumber pn2
        WHERE pn2.ContactID = co.ID)

      FOR XML PATH (''))
    , 2, 5000) AS PhoneNumber
   ,ci.CityName
   ,g.GenderName
   ,co.Address
   ,co.DOB
   ,co.IsFavorite
   ,co.ContactPhoto
   ,co.AdditionalDetails

  FROM dbo.Contact co
  LEFT JOIN dic.City ci
    ON co.CityID = ci.ID
  LEFT JOIN dic.Gender g
    ON co.GenderID = g.ID
  WHERE co.IsDeleted = 1

  ORDER BY co.Firstname, co.Lastname, co.DOB, ci.CityName

END;
GO

CREATE PROCEDURE dbo.Contact_Favorited_GetList -- მხოლოდ ფავორიტი კონტაქტების სიის წამოღება
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    co.ID AS ContactID
   ,co.Firstname + ' ' + co.Lastname AS Fullname
   ,SUBSTRING((SELECT
        ', ' + pnt.TypeName + ': ' + pn1.PhoneNumber
      FROM dbo.PhoneNumber pn1
      LEFT JOIN dic.PhoneNumberType pnt
        ON pn1.PhoneNumberTypeID = pnt.ID
      WHERE pn1.ID IN (SELECT
          pn2.ID
        FROM dbo.PhoneNumber pn2
        WHERE pn2.ContactID = co.ID)

      FOR XML PATH (''))
    , 2, 5000) AS PhoneNumber
   ,co.IsFavorite
   ,ci.CityName
   ,g.GenderName
   ,co.Address
   ,co.DOB
   ,co.ContactPhoto
   ,co.AdditionalDetails

  FROM dbo.Contact co
  LEFT JOIN dic.City ci
    ON co.CityID = ci.ID
  LEFT JOIN dic.Gender g
    ON co.GenderID = g.ID
  WHERE co.IsDeleted = 0
  AND co.IsFavorite = 1

  ORDER BY co.Firstname, co.Lastname, co.DOB, ci.CityName

END;
GO

CREATE PROC dbo.Contact_Get -- ერთი კონტაქტის წამოღება
@ContactID INT
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    c.ID AS ContactID
   ,c.GenderID
   ,c.CityID
   ,c.Firstname
   ,c.Lastname
   ,c.DOB
   ,c.Address
   ,c.ContactPhoto
   ,c.IsFavorite
   ,c.AdditionalDetails
  FROM dbo.Contact c
  WHERE c.ID = @ContactID
  AND c.IsDeleted = 0

END
GO

CREATE PROCEDURE dbo.Contact_GetList -- კონტაქტების სრული სიის წამოღება
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    co.ID AS ContactID
   ,co.Firstname + ' ' + co.Lastname AS Fullname
   ,SUBSTRING((SELECT
        ', ' + pnt.TypeName + ': ' + pn1.PhoneNumber
      FROM dbo.PhoneNumber pn1
      LEFT JOIN dic.PhoneNumberType pnt
        ON pn1.PhoneNumberTypeID = pnt.ID
      WHERE pn1.ID IN (SELECT
          pn2.ID
        FROM dbo.PhoneNumber pn2
        WHERE pn2.ContactID = co.ID)

      FOR XML PATH (''))
    , 2, 5000) AS PhoneNumber
   ,co.IsFavorite
   ,ci.CityName
   ,g.GenderName
   ,co.Address
   ,co.DOB
   ,co.ContactPhoto
   ,co.AdditionalDetails

  FROM dbo.Contact co
  LEFT JOIN dic.City ci
    ON co.CityID = ci.ID
  LEFT JOIN dic.Gender g
    ON co.GenderID = g.ID
  WHERE co.IsDeleted = 0

  ORDER BY co.Firstname, co.Lastname, co.DOB, ci.CityName

END;
GO

CREATE PROCEDURE dbo.Contact_Post -- ახალი კონტაქტის დამატება
@Firstname NVARCHAR(100),
@Lastname NVARCHAR(100),
@DOB DATE,
@GenderID INT,
@CityID INT = NULL,
@Address NVARCHAR(500) = NULL,
@AdditionalDetails NVARCHAR(500) = NULL,
@ContactPhoto IMAGE = NULL,
@IsFavorite BIT = 0,

@CityActionID BIT, -- თუ 0-ია მაშინ ქალაქი დაემატება არსებული სიიდან, თუ 1-ია უნდა გამოვიძახოთ ახალი ქალაქის ჩამატების პროცედურა
@CityName NVARCHAR(200) = NULL,

@PhoneNumberTypeActionID BIT, -- თუ 0-ია მაშინ დაემატება არსებული სიიდან, თუ 1-ია უნდა გამოვიძახოთ ახალი ტელეფონის ნომრის ტიპის ჩამატების პროცედურა
@PhoneNumberTypeName NVARCHAR = NULL,
@PhoneNumbersList AS dbo.PhoneNumberList READONLY -- ტელეფონის ნომრების ცხრილი

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @PhoneNumberTypeID INT
    DECLARE @ContactID INT

    -- ტელეფონის ნომრების სრული სიისთვის
    DECLARE @PhoneNumbers TABLE (
      PhoneNumberTypeID INT
     ,PhoneNumber VARCHAR(20)
    )

    INSERT INTO @PhoneNumbers (PhoneNumberTypeID, PhoneNumber)
      SELECT
        PhoneNumberTypeID
       ,PhoneNumber
      FROM @PhoneNumbersList


    IF @CityActionID = 1
    BEGIN
      EXEC City_Post @CityName = @CityName
                    ,@CityOutputID = @CityID OUTPUT
    END


    INSERT INTO dbo.Contact (GenderID, CityID, Firstname, Lastname, DOB, Address, AdditionalDetails, ContactPhoto, IsFavorite, DataCreated, DataModified)
      VALUES (@GenderID, @CityID, @Firstname, @Lastname, @DOB, @Address, @AdditionalDetails, @ContactPhoto, @IsFavorite, GETDATE(), GETDATE());

    SET @ContactID = SCOPE_IDENTITY()

    -- თუ ActionID = 1 ჯერ ტელეფონის ნომრის ტიპებში ვამატებთ ახალ ტიპს 
    IF @PhoneNumberTypeActionID = 1
    BEGIN
      EXEC PhoneNumberType_Post @TypeName = @PhoneNumberTypeName
                               ,@PhoneNumberOutputID = @PhoneNumberTypeID OUTPUT
      -- ვაინსერტებთ ახალ ტიპს და ტელეფონის ნომერს ტელეფონების ცხრილში
      INSERT INTO @PhoneNumbers (PhoneNumberTypeID, PhoneNumber)
        VALUES (@PhoneNumberTypeID, @PhoneNumberTypeName);
    END

    -- ტელეფონის ნომრების ჩამატების პროცედურის გამოძახება უკვე დამატებულ კონტაქტზე
    EXEC PhoneNumbers_Post @PhoneNumbersList = @PhoneNumbers
                          ,@ContactID = @ContactID

    COMMIT TRANSACTION;

    SELECT
      @ContactID AS ContactID

  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.Contact_Put @ContactID AS INT, -- კონტაქტის რედაქტირება 
@Firstname NVARCHAR(100),
@Lastname NVARCHAR(100),
@DOB DATE,
@GenderID INT,
@CityID INT = NULL,
@Address NVARCHAR(500) = NULL,
@AdditionalDetails NVARCHAR(500) = NULL,
@ContactPhoto IMAGE = NULL,
@IsFavorite BIT = 0,

@CityActionID BIT, -- თუ 0-ია მაშინ ქალაქი დაემატება არსებული სიიდან, თუ 1-ია უნდა გამოვიძახოთ ახალი ქალაქის ჩამატების პროცედურა
@CityName NVARCHAR(200) = NULL,

@PhoneNumberTypeActionID BIT, -- თუ 0-ია მაშინ დაემატება არსებული სიიდან, თუ 1-ია უნდა გამოვიძახოთ ახალი ტელეფონის ნომრის ტიპის ჩამატების პროცედურა
@PhoneNumberTypeName NVARCHAR = NULL,
@PhoneNumbersList AS dbo.PhoneNumberList READONLY, -- ტელეფონის ნომრების ცხრილი
@PhoneNumberTypeID INT

AS
BEGIN

  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    -- ტელეფონის ნომრების სრული სიისთვის
    DECLARE @PhoneNumbers TABLE (
      ID INT
     ,PhoneNumberTypeID INT
     ,PhoneNumber VARCHAR(20)
    )

    INSERT INTO @PhoneNumbers (ID, PhoneNumberTypeID, PhoneNumber)
      SELECT
        ID
       ,PhoneNumberTypeID
       ,PhoneNumber
      FROM @PhoneNumbersList


    IF @CityActionID = 1
    BEGIN
      EXEC City_Put @CityID = @CityID
                   ,@CityName = @CityName
    END

    UPDATE dbo.Contact
    SET GenderID = @GenderID
       ,CityID = @CityID
       ,Firstname = @Firstname
       ,Lastname = @Lastname
       ,DOB = @DOB
       ,Address = @Address
       ,AdditionalDetails = @AdditionalDetails
       ,ContactPhoto = @ContactPhoto
       ,IsFavorite = @IsFavorite
       ,DataModified = GETDATE()
    WHERE ID = @ContactID;

    -- თუ ActionID = 1 ჯერ ტელეფონის ნომრის ტიპებში ვამატებთ ახალ ტიპს 
    IF @PhoneNumberTypeActionID = 1
    BEGIN
      EXEC PhoneNumberType_Put @TypeID = @PhoneNumberTypeID
                              ,@TypeName = @PhoneNumberTypeName
    END

    -- ტელეფონის ნომრების ჩამატების პროცედურის გამოძახება უკვე დამატებულ კონტაქტზე
    EXEC PhoneNumbers_Put @PhoneNumbersList = @PhoneNumbers
                         ,@ContactID = @ContactID

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@trancount > 0
    BEGIN
      ROLLBACK TRANSACTION;
    END;

    DECLARE @err NVARCHAR(MAX);
    SET @err = ERROR_MESSAGE();
    RAISERROR (@err, 16, 1);

  END CATCH;
END;
GO

CREATE PROCEDURE dbo.Contact_Search -- კონტაქტის ძებნა სახელით ან გვარით (სწრაფი)
@SearchText AS NVARCHAR(200)
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    co.ID AS ContactID
   ,co.Firstname + ' ' + co.Lastname AS Fullname
   ,SUBSTRING((SELECT
        ', ' + pnt.TypeName + ': ' + pn1.PhoneNumber
      FROM dbo.PhoneNumber pn1
      LEFT JOIN dic.PhoneNumberType pnt
        ON pn1.PhoneNumberTypeID = pnt.ID
      WHERE pn1.ID IN (SELECT
          pn2.ID
        FROM dbo.PhoneNumber pn2
        WHERE pn2.ContactID = co.ID)

      FOR XML PATH (''))
    , 2, 5000) AS PhoneNumber
   ,co.IsFavorite
   ,ci.CityName
   ,g.GenderName
   ,co.Address
   ,co.DOB
   ,co.ContactPhoto
   ,co.AdditionalDetails

  FROM dbo.Contact co
  LEFT JOIN dic.City ci
    ON co.CityID = ci.ID
  LEFT JOIN dic.Gender g
    ON co.GenderID = g.ID
  WHERE co.IsDeleted = 0
  AND (co.Firstname LIKE N'%' + @SearchText + '%'
  OR co.Lastname LIKE N'%' + @SearchText + '%')

  ORDER BY co.Firstname, co.Lastname, co.DOB, ci.CityName

END;
GO

CREATE PROCEDURE dbo.Contact_Search_Detailed -- კონტაქტის ძებნა დეტალური
@Firstname AS NVARCHAR(100) = NULL,
@Lastname AS NVARCHAR(100) = NULL,
@DOB AS DATE = NULL,
@Address NVARCHAR(500) = NULL,
@MobilePhone VARCHAR(20) = NULL
AS
BEGIN
  SET NOCOUNT ON

  SELECT
    co.ID AS ContactID
   ,co.Firstname + ' ' + co.Lastname AS Fullname
   ,SUBSTRING((SELECT
        ', ' + pnt.TypeName + ': ' + pn1.PhoneNumber
      FROM dbo.PhoneNumber pn1
      LEFT JOIN dic.PhoneNumberType pnt
        ON pn1.PhoneNumberTypeID = pnt.ID
      WHERE pn1.ID IN (SELECT
          pn2.ID
        FROM dbo.PhoneNumber pn2
        WHERE pn2.ContactID = co.ID)

      FOR XML PATH (''))
    , 2, 5000) AS PhoneNumber
   ,co.IsFavorite
   ,ci.CityName
   ,g.GenderName
   ,co.Address
   ,co.DOB
   ,co.ContactPhoto
   ,co.AdditionalDetails

  FROM dbo.Contact co
  LEFT JOIN dic.City ci
    ON co.CityID = ci.ID
  LEFT JOIN dic.Gender g
    ON co.GenderID = g.ID
  WHERE co.IsDeleted = 0
  AND (co.Firstname LIKE N'%' + @Firstname + '%'
  OR @Firstname IS NULL)
  AND (co.Lastname LIKE N'%' + @Lastname + '%'
  OR @Lastname IS NULL)
  AND (co.Address LIKE N'%' + @Address + '%'
  OR @Address IS NULL)
  AND (co.DOB = @DOB
  OR @DOB IS NULL)
  AND (EXISTS (SELECT
      pn.PhoneNumber
    FROM PhoneNumber pn
    WHERE pn.ContactID = co.ID
    AND pn.PhoneNumber LIKE N'%' + @MobilePhone + '%')
  OR @MobilePhone IS NULL)

  ORDER BY co.Firstname, co.Lastname, co.DOB, ci.CityName

END;
GO