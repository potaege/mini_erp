# mini_erp

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

ตั้งค่า URL หลักของ backend ที่ `app\appsettings.json` ใน `Api:BaseUrl` โดยค่าเริ่มต้นคือ `http://localhost:5000`
หน้า Login ใช้ `POST /api/Auth/login` และส่ง `name` กับ `password`
token จะเก็บไว้ในหน่วยความจำขณะเปิดแอปเท่านั้น ไม่บันทึกรหัสผ่านหรือ token ลงดิสก์
เมื่อ token เหลือเวลาตาม `Authentication:WarningMinutes` แอปจะแสดง popup ให้เลือกอยู่ต่อหรือไม่อยู่ต่อ
หากไม่ตอบจน token หมดอายุ แอปจะกลับไปหน้า Login โดยอัตโนมัติ
การต่ออายุ token จริงต้องมี refresh-token endpoint จาก backend; API ปัจจุบันยังไม่มี endpoint นี้

หลัง Login แอปโหลดรายการสินค้าจาก `GET /api/Products` และค้นหาชื่อหรือรหัสสินค้าผ่าน query `search`
เลือกสินค้าเพื่อปรับจำนวนคงเหลือด้วย `PUT /api/Products/{id}/stock`
การเรียก API ทำงานแบบ asynchronous พร้อมแสดงสถานะโหลดและข้อความเมื่อเชื่อมต่อหรือบันทึกไม่สำเร็จ
