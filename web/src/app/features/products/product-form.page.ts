import { Component, inject, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { getApiErrorMessage } from '../../core/api-error';
import { NotificationService } from '../../core/notification.service';
import { ProductsService } from '../../core/products.service';

const integerValidator: ValidatorFn = (
  control: AbstractControl<number>,
): ValidationErrors | null => Number.isInteger(control.value) ? null : { integer: true };
const moneyValidator: ValidatorFn = (
  control: AbstractControl<number>,
): ValidationErrors | null => {
  if (!Number.isFinite(control.value)) {
    return { number: true };
  }

  return Number(control.value.toFixed(2)) === control.value
    ? null
    : { decimalPlaces: true };
};
const nonWhitespaceValidator: ValidatorFn = (
  control: AbstractControl<string>,
): ValidationErrors | null => control.value.trim().length > 0 ? null : { whitespace: true };

@Component({
  selector: 'app-product-form-page',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="page form-page">
      <a class="back-link" routerLink="/products"><span aria-hidden="true">←</span> กลับไปหน้าสินค้า</a>
      <div class="page-heading">
        <div>
          <p class="eyebrow">{{ isEditMode() ? 'EDIT PRODUCT' : 'NEW PRODUCT' }}</p>
          <h1>{{ isEditMode() ? 'แก้ไขสินค้า' : 'เพิ่มสินค้า' }}</h1>
          <p class="subtitle">{{ isEditMode() ? 'แก้ไขชื่อหรือราคาสินค้า' : 'กรอกรายละเอียดสินค้าใหม่ที่ต้องการเพิ่ม' }}</p>
        </div>
      </div>
      @if (errorMessage()) {
        <div class="alert" role="alert">{{ errorMessage() }}</div>
      }
      @if (isLoading()) {
        <div class="card loading">กำลังโหลดข้อมูลสินค้า...</div>
      } @else {
        <form class="card product-form" [formGroup]="form" (ngSubmit)="save()">
          <div class="form-section-title">รายละเอียดสินค้า</div>
          <div class="form-field">
            <label for="sku">รหัสสินค้า (SKU) <span class="required">*</span></label>
            <input id="sku" formControlName="sku" [readonly]="isEditMode()" placeholder="เช่น SKU-001" maxlength="50">
            @if (form.controls.sku.touched && form.controls.sku.invalid) {
              <span class="field-error">
                @if (form.controls.sku.hasError('maxlength')) { รหัสสินค้าต้องไม่เกิน 50 ตัวอักษร }
                @else { กรุณากรอกรหัสสินค้า }
              </span>
            }
            @if (isEditMode()) { <span class="field-hint">ไม่สามารถแก้ไขรหัสสินค้าได้</span> }
          </div>
          <div class="form-field">
            <label for="name">ชื่อสินค้า <span class="required">*</span></label>
            <input id="name" formControlName="name" placeholder="กรอกชื่อสินค้า" maxlength="200">
            @if (form.controls.name.touched && form.controls.name.invalid) {
              <span class="field-error">
                @if (form.controls.name.hasError('maxlength')) { ชื่อสินค้าต้องไม่เกิน 200 ตัวอักษร }
                @else { กรุณากรอกชื่อสินค้า }
              </span>
            }
          </div>
          <div class="form-field">
            <label for="price">ราคาต่อหน่วย <span class="required">*</span></label>
            <div class="input-with-suffix"><input id="price" type="number" formControlName="price" min="0.01" max="9999999999.99" step="0.01" placeholder="0.00"><span>บาท</span></div>
            @if (form.controls.price.touched && form.controls.price.invalid) {
              <span class="field-error">
                @if (form.controls.price.hasError('decimalPlaces')) { ราคาต้องมีทศนิยมไม่เกิน 2 ตำแหน่ง }
                @else { ราคาต้องมากกว่า 0 และไม่เกิน 9,999,999,999.99 บาท }
              </span>
            }
          </div>
          @if (!isEditMode()) {
            <div class="form-field">
              <label for="stockQuantity">จำนวนคงเหลือเริ่มต้น <span class="required">*</span></label>
              <div class="input-with-suffix"><input id="stockQuantity" type="number" formControlName="stockQuantity" min="0" max="2147483647" step="1" placeholder="0"><span>ชิ้น</span></div>
              @if (form.controls.stockQuantity.touched && form.controls.stockQuantity.invalid) {
                <span class="field-error">จำนวนคงเหลือต้องเป็นจำนวนเต็มตั้งแต่ 0 ขึ้นไป</span>
              }
            </div>
          }
          <div class="form-actions">
            <a class="button button-secondary" [routerLink]="cancelLink()">ยกเลิก</a>
            <button class="button button-primary" type="submit" [disabled]="isSaving()">
              {{ isSaving() ? 'กำลังบันทึก...' : (isEditMode() ? 'บันทึกการแก้ไข' : 'เพิ่มสินค้า') }}
            </button>
          </div>
        </form>
      }
    </section>
  `,
  styles: `
    .back-link { display: inline-flex; gap: 8px; margin-bottom: 22px; color: #778397; font-size: 12px; font-weight: 600; }
    .back-link:hover { color: #3559e8; }
    .product-form { max-width: 720px; padding: 23px 25px; }
    .form-section-title { margin-bottom: 20px; color: #344054; font: 700 14px 'Segoe UI', Arial, sans-serif; }
    .product-form > .form-field { margin-bottom: 17px; }
    .required { color: #c43e49; }
    .field-hint { color: #98a2b3; font-size: 11px; }
    .input-with-suffix { position: relative; }
    .input-with-suffix input { padding-right: 50px; }
    .input-with-suffix span { position: absolute; top: 14px; right: 12px; color: #98a2b3; font-size: 11px; }
    .form-actions { display: flex; justify-content: flex-end; gap: 9px; margin-top: 27px; padding-top: 18px; border-top: 1px solid #eff1f5; }
    @media (max-width: 520px) { .product-form { padding: 19px 16px; } .form-actions { flex-direction: column-reverse; } .form-actions .button { width: 100%; } }
  `,
})
export class ProductFormPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsService = inject(ProductsService);
  private readonly notifications = inject(NotificationService);

  protected readonly isEditMode = signal(false);
  protected readonly isLoading = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly productId = signal<number | null>(null);
  protected readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200), nonWhitespaceValidator]],
    sku: ['', [Validators.required, Validators.maxLength(50), nonWhitespaceValidator]],
    price: [0, [
      Validators.required,
      Validators.min(0.01),
      Validators.max(9999999999.99),
      moneyValidator,
    ]],
    stockQuantity: [0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      integerValidator,
    ]],
  });

  ngOnInit(): void {
    const idValue = this.route.snapshot.paramMap.get('id');
    if (idValue === null) {
      return;
    }

    const id = Number(idValue);
    if (!Number.isInteger(id) || id < 1) {
      this.errorMessage.set('ไม่พบรหัสสินค้า');
      return;
    }

    this.productId.set(id);
    this.isEditMode.set(true);
    this.isLoading.set(true);
    this.productsService.getById(id).subscribe({
      next: (product) => {
        this.form.patchValue({
          name: product.name,
          sku: product.sku,
          price: product.price,
          stockQuantity: product.stockQuantity,
        });
        this.errorMessage.set('');
      },
      error: (error: unknown) => {
        this.errorMessage.set(getApiErrorMessage(error, 'โหลดข้อมูลสินค้า'));
        this.isLoading.set(false);
      },
      complete: () => this.isLoading.set(false),
    });
  }

  protected cancelLink(): string[] {
    const id = this.productId();
    return id === null ? ['/products'] : ['/products', String(id)];
  }

  protected save(): void {
    if (this.form.invalid || this.isSaving()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    const { name, sku, price, stockQuantity } = this.form.getRawValue();
    const id = this.productId();
    const request = id === null
      ? this.productsService.create({ name: name.trim(), sku: sku.trim(), price, stockQuantity })
      : this.productsService.update(id, { name: name.trim(), sku, price });

    request.subscribe({
      next: (product) => {
        this.notifications.success(id === null ? 'เพิ่มสินค้าเรียบร้อยแล้ว' : 'บันทึกการแก้ไขเรียบร้อยแล้ว');
        void this.router.navigate(['/products', product.id]);
      },
      error: (error: unknown) => {
        this.errorMessage.set(getApiErrorMessage(error, id === null ? 'เพิ่มสินค้า' : 'บันทึกข้อมูลสินค้า'));
        this.isSaving.set(false);
      },
      complete: () => this.isSaving.set(false),
    });
  }
}
