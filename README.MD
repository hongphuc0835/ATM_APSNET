tạo project webapi: dotnet new webapi -n ten_project
(webapp, webapi, mvc)

1. Tạo project webapi
2. cài dặt thư viẹn cho Entity Framework (data model)
 dotnet add package Pomelo.EntityFrameWorkCore.MySql (Pomlo MySql Provider)
 dotnet add package Microsoft.EntityFrameWorkCore.Tools
 (nếu bị lỗi câu lệnh trên thì dùng cái này : dotnet tool install --global dotnet-ef )


 3. đồng bộ hóa với database ( tạo migration)
 dotnet ef migrations add InitialCreate

cập nhật vao database
dotnet ef database update
