Hướng dẫn chạy source code

B1: Vào SQL Server thực thi đoạn query sau để tạo database và table
CREATE DATABASE HRM8_SQL_SE_TESTBUILD;
GO
USE HRM8_SQL_SE_TESTBUILD;
GO

DROP TABLE IF EXISTS Temp_Employees; -- Xóa bảng nếu đã tồn tại

CREATE TABLE Temp_Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CodeEmp AS ('NV_' + CAST(Id AS VARCHAR)) PERSISTED, -- Mã nhân viên tự động
    FullName NVARCHAR(255) NOT NULL, 
    DateOfBirth DATE NOT NULL, 
    Age AS (DATEDIFF(YEAR, DateOfBirth, GETDATE()) - 
           CASE 
               WHEN MONTH(DateOfBirth) > MONTH(GETDATE()) 
                    OR (MONTH(DateOfBirth) = MONTH(GETDATE()) AND DAY(DateOfBirth) > DAY(GETDATE())) 
               THEN 1 
               ELSE 0 
           END) 
);

B2: pull code từ nhánh master về git pull origin master 
