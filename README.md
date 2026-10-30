# Mini ERP

## แอปเดสก์ท็อป (app)

แอปติดตั้งบนคอมพิวเตอร์ พัฒนาด้วย C# และ Windows Forms บน .NET 10 สำหรับ Windows

โปรเจกต์อยู่ที่ `app\MiniErp.App.csproj`

```powershell
dotnet run --project app\MiniErp.App.csproj
```

หน้าจอหลักแยกเป็น `LoginForm` และ `ProductForm` โดย `MiniErpApplicationContext` เป็นตัวสลับหน้าต่างตามสถานะการเข้าสู่ระบบ
`ProductForm` จัดการหน้าต่างและอายุ session ส่วน `ProductCatalogController` จัดการค้นหา/โหลดสินค้าและบันทึกสต็อกผ่าน `ProductsService`

ส่วน UI แยกตามหน้าที่ไว้ใน `app\Controls\Products\`:

- `ProductCatalogControl.cs` ประกอบหน้ารายการสินค้า
- `ProductSearchControl.cs` ช่องค้นหาสินค้า
- `ProductGridControl.cs` ตารางแสดงสินค้า
- `ProductStockEditorControl.cs` ส่วนปรับจำนวนคงเหลือ

ฟอร์ม Login ใช้ `app\Controls\Auth\LoginControl.cs` แยกจาก `LoginForm`

ตั้งค่า URL หลักของ backend ที่ `app\appsettings.json` ใน `Api:BaseUrl` โดยค่าเริ่มต้นคือ `http://localhost:5285`
หน้า Login ใช้ `POST /api/Auth/login` และส่ง `name` กับ `password`
token จะเก็บไว้ในหน่วยความจำขณะเปิดแอปเท่านั้น ไม่บันทึกรหัสผ่านหรือ token ลงดิสก์
เมื่อ token เหลือเวลาตาม `Authentication:WarningMinutes` แอปจะแสดง popup ให้เลือกอยู่ต่อหรือไม่อยู่ต่อ
หากไม่ตอบจน token หมดอายุ แอปจะกลับไปหน้า Login โดยอัตโนมัติ
การต่ออายุ token จริงต้องมี refresh-token endpoint จาก backend; API ปัจจุบันยังไม่มี endpoint นี้

หลัง Login แอปโหลดรายการสินค้าจาก `GET /api/Products` และค้นหาชื่อหรือรหัสสินค้าผ่าน query `search`
เลือกสินค้าเพื่อปรับจำนวนคงเหลือด้วย `PUT /api/Products/{id}/stock`
การเรียก API ทำงานแบบ asynchronous พร้อมแสดงสถานะโหลดและข้อความเมื่อเชื่อมต่อหรือบันทึกไม่สำเร็จ

## Backend API

Backend API พัฒนาด้วย ASP.NET Core และ MariaDB

### สิ่งที่ต้องมี
- .NET 10 SDK
- MariaDB ที่กำลังทำงานอยู่ (ค่าเริ่มต้น `127.0.0.1:3306`)

### ติดตั้งและตั้งค่า

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

### รัน API

```powershell
dotnet run --launch-profile http
```

API จะเปิดที่ `http://localhost:5285` ใน Development สามารถเปิดหน้า API docs ได้ที่:

```text
http://localhost:5285/swagger
```

หากเชื่อมต่อฐานข้อมูลไม่ได้ ให้ตรวจว่า MariaDB ทำงานอยู่ และ host, port, ชื่อฐานข้อมูล และบัญชีใน connection string ถูกต้อง หาก API ทำงานใน Docker ให้ใช้ชื่อ service ของ MariaDB แทน `127.0.0.1`

### เริ่มใช้งาน

ไม่มี endpoint สมัครผู้ใช้ หลังรัน migration แล้ว ให้เพิ่มผู้ใช้สำหรับทดสอบในฐานข้อมูลก่อน ตัวอย่าง:

```sql
INSERT INTO users (name, password) VALUES ('dev', 'change-me');
```

## Web Application (Angular)

Frontend อยู่ที่ `web\` พัฒนาด้วย Angular 21 และ Node.js

### สิ่งที่ต้องมี
- Node.js 20.19.x, 22.12+ หรือ 24+
- npm

### ติดตั้งและรัน

```powershell
cd web
npm install
npm start
```

เปิดเว็บที่ `http://localhost:4200` และใช้คำสั่งต่อไปนี้เพื่อ build หรือทดสอบ:

```powershell
npm run build
npm test
```

เว็บมีหน้า Login, หน้ารายการสินค้า (ค้นหาด้วยชื่อหรือ SKU), เพิ่ม/แก้ไข/ลบสินค้า และหน้าประวัติการเปลี่ยนแปลงทั้งแบบรวมและรายสินค้า ระหว่างพัฒนาจะ proxy `/api` ไปที่ `http://localhost:5285` ดังนั้นให้เปิด backend ไว้พร้อมกัน
