import { HttpErrorResponse } from '@angular/common/http';

type ValidationProblem = {
  errors?: Record<string, string[] | string>;
};

const fieldLabels: Record<string, string> = {
  name: 'ชื่อสินค้า',
  sku: 'รหัสสินค้า (SKU)',
  price: 'ราคา',
  stockquantity: 'จำนวนคงเหลือ',
  password: 'รหัสผ่าน',
};

export function getApiErrorMessage(error: unknown, operation = 'ทำรายการ'): string {
  if (!(error instanceof HttpErrorResponse)) {
    return `เกิดข้อผิดพลาด ไม่สามารถ${operation}ได้ กรุณาลองใหม่อีกครั้ง`;
  }

  if (error.status === 0) {
    return 'ไม่สามารถเชื่อมต่อเซิร์ฟเวอร์ได้ กรุณาตรวจสอบการเชื่อมต่อแล้วลองใหม่อีกครั้ง';
  }

  if (error.status === 400 || error.status === 422) {
    return getValidationMessage(error.error);
  }

  if (error.status === 401) {
    return 'ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง หรือเซสชันหมดอายุ';
  }

  if (error.status === 403) {
    return 'คุณไม่มีสิทธิ์ดำเนินการนี้';
  }

  if (error.status === 404) {
    return 'ไม่พบข้อมูลที่ต้องการ อาจถูกลบหรือเปลี่ยนแปลงไปแล้ว';
  }

  if (error.status === 409) {
    return 'รหัสสินค้า (SKU) นี้มีอยู่ในระบบแล้ว กรุณาใช้รหัสอื่น';
  }

  if (error.status === 429) {
    return 'มีการใช้งานระบบมากเกินไป กรุณารอสักครู่แล้วลองใหม่';
  }

  if (error.status >= 500) {
    return `ระบบขัดข้อง ไม่สามารถ${operation}ได้ในขณะนี้ กรุณาลองใหม่ภายหลัง`;
  }

  return `ไม่สามารถ${operation}ได้ กรุณาตรวจสอบข้อมูลแล้วลองใหม่`;
}

function getValidationMessage(body: unknown): string {
  if (!body || typeof body !== 'object' || !('errors' in body)) {
    return 'ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบข้อมูลที่กรอกแล้วลองใหม่';
  }

  const errors = (body as ValidationProblem).errors;
  if (!errors || typeof errors !== 'object') {
    return 'ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบข้อมูลที่กรอกแล้วลองใหม่';
  }

  const labels = Object.keys(errors)
    .map((field) => fieldLabels[field.toLocaleLowerCase()])
    .filter((label): label is string => label !== undefined);
  const uniqueLabels = [...new Set(labels)];

  return uniqueLabels.length > 0
    ? `กรุณาตรวจสอบข้อมูลต่อไปนี้: ${uniqueLabels.join(', ')}`
    : 'ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบข้อมูลที่กรอกแล้วลองใหม่';
}
