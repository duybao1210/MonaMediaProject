Hướng dẫn chạy source code

B1: Vào SQL Server thực thi đoạn query sau để tạo database và table

CREATE DATABASE HRM8_SQL_SE_TESTBUILD;
GO
USE HRM8_SQL_SE_TESTBUILD;
GO

DROP TABLE IF EXISTS Temp_Employees; -- Xóa bảng nếu đã tồn tại

CREATE TABLE Temp_Employees ( Id INT IDENTITY(1,1) PRIMARY KEY, 
CodeEmp AS ('NV_' + CAST(Id AS VARCHAR)) PERSISTED,
FullName NVARCHAR(255) NOT NULL, 
DateOfBirth DATE NOT NULL)

B2: pull code từ nhánh master về => git pull origin master 

B3: vào app setting cấu hình lại đường dẫn đến server db muốn dùng
"ConnectionStrings": {
  "DefaultConnection": "Server={tên server};Database=HRM8_SQL_SE_TESTBUILD;User Id={tên login};Password={password};TrustServerCertificate=True;"
}
Chỉnh sửa lại phần setup "JwtSettings": {
  "SecretKey": "m+7F9dGkm8a4P3Q6rYz0V1FmlRT/68eB2I1p5nClHjI=",
  "Issuer": "{đường dẫn chạy source}", vd: "https://localhost:7284"
  "Audience": "{đường dẫn chạy source}/api", vd: "https://localhost:7284/api"
  "ExpiryMinutes": 60
}

B4: Build source và chạy source trên swagger

B5: Thực thi Login trên giao diện swagger để lấy token mặc định username = admin; password = 123456 => sau khi trả về token thì tiến hành add token để authorize
