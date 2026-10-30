# Mini ERP Backend

คู่มือติดตั้งและใช้งาน Backend API ด้วย ASP.NET Core และ MariaDB

## สิ่งที่ต้องมี

- .NET 10 SDK
- MariaDB ที่กำลังทำงานอยู่ (ค่าเริ่มต้น `127.0.0.1:3306`)

## ติดตั้งและตั้งค่า

เปิด PowerShell ที่โฟลเดอร์โปรเจกต์ แล้วเข้าโฟลเดอร์ backend:

```powershell
cd backend
```

สร้างฐานข้อมูล `mini_erp` ใน MariaDB ก่อน เช่น:

```sql
CREATE DATABASE mini_erp CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
```

ตั้งค่า connection string และ JWT signing key ด้วย User Secrets (แทน `<db-user>` และ `<db-password>` ด้วยบัญชี MariaDB ของคุณ):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Port=3306;Database=mini_erp;User ID=<db-user>;Password=<db-password>"

$bytes = [System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32)
$jwtKey = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:SigningKey" $jwtKey
```

ติดตั้งเครื่องมือและ dependencies จากนั้นสร้างตารางด้วย migrations:

```powershell
dotnet tool restore
dotnet restore
dotnet tool run dotnet-ef -- database update
```

## รัน API

```powershell
dotnet run --launch-profile http
```

API จะเปิดที่ `http://localhost:5285` ใน Development สามารถเปิดหน้า API docs ได้ที่:

```text
http://localhost:5285/swagger
```

หากเชื่อมต่อฐานข้อมูลไม่ได้ ให้ตรวจว่า MariaDB ทำงานอยู่ และ host, port, ชื่อฐานข้อมูล และบัญชีใน connection string ถูกต้อง หาก API ทำงานใน Docker ให้ใช้ชื่อ service ของ MariaDB แทน `127.0.0.1`

## เริ่มใช้งาน

ไม่มี endpoint สมัครผู้ใช้ หลังรัน migration แล้ว ให้เพิ่มผู้ใช้สำหรับทดสอบในฐานข้อมูลก่อน ตัวอย่าง:

```sql
INSERT INTO users (name, password) VALUES ('dev', 'change-me');
```

