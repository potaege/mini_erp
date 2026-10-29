# mini_erp

## Backend setup

Backend ใช้ C# และ ASP.NET Core Web API บน .NET 10 (`net10.0`) เชื่อม MariaDB ด้วย Entity Framework Core โครงสร้างการทำงานคือ `Controller → Service → Repository → AppDbContext → MariaDB` โดย Repository แยกคำสั่งอ่าน/เขียนฐานข้อมูลออกจาก Service ตารางทั้งสามจาก schema `mini_erp` ถูกกำหนดใน `AppDbContext` และ migration ใน `backend/Migrations`

### สิ่งที่ต้องติดตั้ง

- .NET SDK 10
- MariaDB ที่กำลังทำงานอยู่ (ค่าตั้งต้นคือ `127.0.0.1:3306`)

### สร้าง schema และรัน API

สร้างฐานข้อมูลเปล่าชื่อ `mini_erp` ใน MariaDB/phpMyAdmin ก่อน (ไม่ต้องสร้างตารางเอง) จากนั้นรันคำสั่งใน PowerShell:

```powershell
cd backend
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Port=3306;Database=mini_erp;User ID=<user>;Password=<password>"
$bytes = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
$jwtKey = [Convert]::ToBase64String($bytes)
$rng.Dispose()
dotnet user-secrets set "Jwt:SigningKey" $jwtKey
dotnet tool restore
dotnet restore
dotnet tool run dotnet-ef -- database update
dotnet build
dotnet run --launch-profile http
```

`dotnet tool run dotnet-ef -- database update` จะสร้างตาราง `products`, `product_logs`, `users`, indexes และ foreign keys จาก migration ในโค้ด หากฐานข้อมูลมีตารางจากการ import SQL dump อยู่แล้ว อย่ารัน migration กับฐานนั้นซ้ำ

อย่า commit รหัสผ่านลง Git หาก MariaDB ในเครื่องใช้ `root` โดยไม่มีรหัสผ่าน สามารถใช้ connection string ตั้งต้นใน `backend/appsettings.json` ได้

### Login และเรียก Products API

ส่งชื่อและรหัสผ่านของผู้ใช้ที่มีอยู่ในตาราง `users` ไปที่ `POST /api/auth/login`:

```powershell
$login = Invoke-RestMethod `
  -Uri "http://localhost:5285/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body (@{ name = "<ชื่อผู้ใช้>"; password = "<รหัสผ่าน>" } | ConvertTo-Json)

Invoke-RestMethod `
  -Uri "http://localhost:5285/api/products" `
  -Headers @{ Authorization = "Bearer $($login.accessToken)" }
```

ระบบเทียบรหัสผ่านแบบ plaintext ตามที่กำหนด และคืน JWT ที่หมดอายุใน 60 นาที ทุก endpoint ใน `Products API` ต้องแนบ Bearer token ส่วน login ไม่ต้องใช้ token **การเก็บรหัสผ่านแบบ plaintext ไม่ปลอดภัย**; ควรเปลี่ยนเป็น password hash ก่อนใช้งานจริง

เมื่อ API เริ่มทำงานแล้ว:

- Products API: `http://localhost:5285/api/products`
- OpenAPI JSON สำหรับ Development: `http://localhost:5285/openapi/v1.json`

### Product API

ทุก endpoint ต้องใช้ Bearer token จาก login:

- `POST /api/products` สร้างสินค้า และบันทึก `CREATE`; ถ้าสต็อกเริ่มต้นมากกว่า 0 จะบันทึก `STOCK_IN` เพิ่มด้วย
- `PUT /api/products/{id}` แก้ชื่อ SKU หรือราคา พร้อมบันทึกค่าเดิม/ใหม่ ห้ามแก้สต็อกผ่าน endpoint นี้
- `PUT /api/products/{id}/stock` ตั้งจำนวนสต็อกใหม่ ระบบคำนวณส่วนต่างและบันทึก `STOCK_IN` หรือ `STOCK_OUT`; `change_qty` เป็นค่าบวกเมื่อเพิ่มและค่าลบเมื่อลด
- `DELETE /api/products/{id}` ลบแถวสินค้าออกจริง และบันทึก `DELETE` log ไว้ โดย `product_id` ใน log จะไม่มีสินค้าอ้างถึงหลังลบ

Migration `ProductChangeTracking` เพิ่ม `change_qty`, เพิ่ม log type `SKU` และถอด foreign key `fk_logs_product` สำหรับฐานข้อมูลใหม่/ฐานที่ใช้ EF migrations ตั้งแต่ต้น

ถ้าใช้ฐานข้อมูลที่ import dump และแก้ด้วย SQL เองตามที่ระบุไว้แล้ว (`change_qty` มีอยู่และถอด `fk_logs_product` แล้ว) **อย่ารัน `database update`** ให้เพิ่ม `SKU` ใน enum เพื่อให้แก้ SKU แล้วเขียน log ได้:

```sql
ALTER TABLE product_logs
  MODIFY COLUMN type ENUM('CREATE','NAME','SKU','PRICE','STOCK_IN','STOCK_OUT','DELETE')
  CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;
```
