import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { getApiErrorMessage } from '../../core/api-error';
import { AuthService } from '../../core/auth.service';

const nonWhitespaceValidator: ValidatorFn = (
  control: AbstractControl<string>,
): ValidationErrors | null => control.value.trim().length > 0 ? null : { whitespace: true };

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule],
  template: `
    <main class="login-page">
      <section class="login-card">
        <div class="login-brand">
          <span class="brand-mark">M</span>
          <span>mini<span class="brand-light">erp</span></span>
        </div>
        <p class="eyebrow">WELCOME BACK</p>
        <h1>เข้าสู่ระบบ</h1>
        <p class="intro">ลงชื่อเข้าใช้เพื่อจัดการสินค้าและตรวจสอบประวัติ</p>
        @if (errorMessage()) {
          <div class="alert" role="alert">{{ errorMessage() }}</div>
        }
        <form [formGroup]="form" (ngSubmit)="submit()">
          <div class="form-field">
            <label for="username">ชื่อผู้ใช้</label>
            <input id="username" formControlName="name" autocomplete="username" placeholder="กรอกชื่อผู้ใช้" maxlength="100">
            @if (form.controls.name.touched && form.controls.name.invalid) {
              <span class="field-error">
                @if (form.controls.name.hasError('maxlength')) { ชื่อผู้ใช้ต้องไม่เกิน 100 ตัวอักษร }
                @else { กรุณากรอกชื่อผู้ใช้ }
              </span>
            }
          </div>
          <div class="form-field">
            <label for="password">รหัสผ่าน</label>
            <input id="password" type="password" formControlName="password" autocomplete="current-password" placeholder="กรอกรหัสผ่าน" maxlength="30">
            @if (form.controls.password.touched && form.controls.password.invalid) {
              <span class="field-error">
                @if (form.controls.password.hasError('maxlength')) { รหัสผ่านต้องไม่เกิน 30 ตัวอักษร }
                @else { กรุณากรอกรหัสผ่าน }
              </span>
            }
          </div>
          <button class="button button-primary submit-button" type="submit" [disabled]="isSubmitting()">
            {{ isSubmitting() ? 'กำลังเข้าสู่ระบบ...' : 'เข้าสู่ระบบ' }}
          </button>
        </form>
      </section>
      <footer>MINI ERP <span>·</span> PRODUCT MANAGEMENT</footer>
    </main>
  `,
  styles: `
    .login-page { min-height: 100vh; display: grid; align-content: center; justify-items: center; padding: 30px 18px; background: radial-gradient(ellipse at 50% 5%, #e8edff 0, #f5f7fb 54%); }
    .login-card { width: min(100%, 430px); border: 1px solid #e9edf4; border-radius: 17px; padding: 39px 40px 30px; background: #fff; box-shadow: 0 16px 50px #243a6d12; }
    .login-brand { display: flex; align-items: center; gap: 10px; margin-bottom: 38px; font: 800 19px 'Segoe UI', Arial, sans-serif; letter-spacing: -.06em; }
    .brand-mark { width: 33px; height: 33px; display: grid; place-items: center; border-radius: 9px; background: #3559e8; color: white; font-size: 15px; }
    .brand-light { color: #778397; font-weight: 500; }
    .eyebrow { margin-bottom: 7px; color: #3559e8; font-size: 10px; font-weight: 700; letter-spacing: .15em; }
    h1 { margin: 0; font: 800 27px 'Segoe UI', Arial, sans-serif; letter-spacing: -.04em; }
    .intro { margin: 8px 0 25px; color: #778397; font-size: 13px; }
    form { display: grid; gap: 17px; }
    .submit-button { width: 100%; margin-top: 5px; }
    footer { margin-top: 26px; color: #98a2b3; font-size: 9px; font-weight: 700; letter-spacing: .13em; }
    footer span { padding: 0 7px; color: #c0c6d0; }
    @media (max-width: 480px) { .login-card { padding: 30px 23px 25px; } }
  `,
})
export class LoginPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100), nonWhitespaceValidator]],
    password: ['', [Validators.required, Validators.maxLength(30)]],
  });

  submit(): void {
    if (this.form.invalid || this.isSubmitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage.set('');
    this.isSubmitting.set(true);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        const safeReturnUrl = returnUrl?.startsWith('/') && !returnUrl.startsWith('//')
          ? returnUrl
          : '/products';
        void this.router.navigateByUrl(safeReturnUrl);
      },
      error: (error: unknown) => {
        this.errorMessage.set(getApiErrorMessage(error, 'เข้าสู่ระบบ'));
        this.isSubmitting.set(false);
      },
      complete: () => this.isSubmitting.set(false),
    });
  }
}
