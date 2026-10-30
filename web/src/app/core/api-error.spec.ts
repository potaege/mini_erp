import { HttpErrorResponse } from '@angular/common/http';
import { describe, expect, it } from 'vitest';
import { getApiErrorMessage } from './api-error';

describe('getApiErrorMessage', () => {
  it('explains when the backend cannot be reached', () => {
    const error = new HttpErrorResponse({ status: 0 });

    expect(getApiErrorMessage(error, 'เพิ่มสินค้า')).toContain('ไม่สามารถเชื่อมต่อเซิร์ฟเวอร์ได้');
    expect(getApiErrorMessage(error)).toContain('ลองใหม่อีกครั้ง');
  });

  it('turns backend validation fields into readable Thai labels', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { errors: { Name: ['Required'], Price: ['Out of range'] } },
    });

    expect(getApiErrorMessage(error)).toBe('กรุณาตรวจสอบข้อมูลต่อไปนี้: ชื่อสินค้า, ราคา');
  });

  it('does not expose server exception details for unexpected backend failures', () => {
    const error = new HttpErrorResponse({
      status: 500,
      error: { message: 'Database connection string is secret' },
    });

    const message = getApiErrorMessage(error, 'บันทึกข้อมูลสินค้า');
    expect(message).toContain('ไม่สามารถบันทึกข้อมูลสินค้าได้');
    expect(message).not.toContain('secret');
  });
});
