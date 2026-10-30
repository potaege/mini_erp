import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { getApiErrorMessage } from '../../core/api-error';
import { Product } from '../../core/models';
import { NotificationService } from '../../core/notification.service';
import { ProductsService } from '../../core/products.service';

@Component({
  selector: 'app-product-details-page',
  imports: [DatePipe, DecimalPipe, RouterLink],
  template: `
    <section class="page">
      <a class="back-link" routerLink="/products"><span aria-hidden="true">←</span> กลับไปหน้าสินค้า</a>
      @if (isLoading()) {
        <div class="card loading">กำลังโหลดข้อมูลสินค้า...</div>
      } @else if (errorMessage()) {
        <div class="alert" role="alert">{{ errorMessage() }}</div>
      } @else if (product(); as item) {
        <div class="page-heading detail-heading">
          <div>
            <p class="eyebrow">PRODUCT DETAIL</p>
            <h1>{{ item.name }}</h1>
            <p class="subtitle">รหัสสินค้า <span class="mono">{{ item.sku }}</span></p>
          </div>
          <div class="heading-actions">
            <a class="button button-secondary" [routerLink]="['/products', item.id, 'logs']">◷ ดูประวัติ</a>
            <a class="button button-primary" [routerLink]="['/products', item.id, 'edit']">✎ แก้ไขสินค้า</a>
          </div>
        </div>
        <div class="detail-grid">
          <section class="card info-card">
            <p class="card-label">ข้อมูลสินค้า</p>
            <div class="info-row"><span>ชื่อสินค้า</span><strong>{{ item.name }}</strong></div>
            <div class="info-row"><span>รหัสสินค้า (SKU)</span><strong class="mono">{{ item.sku }}</strong></div>
            <div class="info-row"><span>ราคาต่อหน่วย</span><strong>{{ item.price | number:'1.2-2' }}</strong></div>
            <div class="info-row"><span>สถานะสินค้า</span><span class="badge" [class.badge-green]="item.isActive" [class.badge-gray]="!item.isActive">{{ item.isActive ? 'ใช้งาน' : 'ปิดใช้งาน' }}</span></div>
            <div class="info-row"><span>เพิ่มเมื่อ</span><strong>{{ item.createdAt | date:'dd/MM/yyyy HH:mm' }}</strong></div>
            <div class="info-row"><span>แก้ไขล่าสุด</span><strong>{{ item.updatedAt | date:'dd/MM/yyyy HH:mm' }}</strong></div>
          </section>
          <section class="card stock-card">
            <div class="stock-icon">▦</div>
            <p>จำนวนคงเหลือ</p>
            <strong>{{ item.stockQuantity | number }}</strong>
            <span>หน่วย</span>
            <button class="button button-danger" type="button" [disabled]="isDeleting()" (click)="deleteProduct(item)">
              {{ isDeleting() ? 'กำลังลบ...' : 'ลบสินค้า' }}
            </button>
          </section>
        </div>
      }
    </section>
  `,
  styles: `
    .back-link { display: inline-flex; gap: 8px; margin-bottom: 22px; color: #778397; font-size: 12px; font-weight: 600; }
    .back-link:hover { color: #3559e8; }
    .detail-heading { align-items: center; }
    .heading-actions { display: flex; gap: 9px; }
    .detail-grid { display: grid; grid-template-columns: minmax(0, 1fr) 270px; gap: 17px; }
    .info-card { padding: 22px; }
    .card-label { margin-bottom: 8px; color: #344054; font: 700 14px 'Segoe UI', Arial, sans-serif; }
    .info-row { min-height: 49px; display: flex; align-items: center; justify-content: space-between; gap: 12px; border-bottom: 1px solid #eff1f5; font-size: 12px; }
    .info-row:last-child { border-bottom: 0; }
    .info-row > span:first-child { color: #8490a2; }
    .info-row strong { text-align: right; }
    .stock-card { display: flex; flex-direction: column; align-items: center; padding: 25px 20px; }
    .stock-icon { width: 39px; height: 39px; display: grid; place-items: center; border-radius: 11px; background: #eef1ff; color: #3559e8; font-size: 20px; }
    .stock-card p { margin: 16px 0 5px; color: #778397; font-size: 12px; }
    .stock-card > strong { font: 800 36px 'Segoe UI', Arial, sans-serif; letter-spacing: -.05em; }
    .stock-card > span { margin-top: 2px; color: #9aa4b2; font-size: 11px; }
    .stock-card .button { width: 100%; margin-top: auto; }
    @media (max-width: 720px) { .detail-grid { grid-template-columns: 1fr; } .stock-card .button { margin-top: 19px; } }
    @media (max-width: 520px) { .detail-heading { align-items: flex-start; flex-direction: column; } .heading-actions { width: 100%; } .heading-actions .button { flex: 1; } }
  `,
})
export class ProductDetailsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsService = inject(ProductsService);
  private readonly notifications = inject(NotificationService);

  protected readonly product = signal<Product | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isDeleting = signal(false);
  protected readonly errorMessage = signal('');

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isInteger(id) || id < 1) {
      this.errorMessage.set('ไม่พบรหัสสินค้า');
      this.isLoading.set(false);
      return;
    }

    this.productsService.getById(id).subscribe({
      next: (product) => {
        this.product.set(product);
        this.errorMessage.set('');
      },
      error: (error: unknown) => {
        this.errorMessage.set(getApiErrorMessage(error, 'โหลดข้อมูลสินค้า'));
        this.isLoading.set(false);
      },
      complete: () => this.isLoading.set(false),
    });
  }

  protected deleteProduct(product: Product): void {
    if (!window.confirm(`ยืนยันการลบสินค้า "${product.name}" (${product.sku}) หรือไม่?`)) {
      return;
    }

    this.isDeleting.set(true);
    this.productsService.delete(product.id).subscribe({
      next: () => {
        this.notifications.success('ลบสินค้าเรียบร้อยแล้ว');
        void this.router.navigate(['/products']);
      },
      error: (error: unknown) => {
        this.notifications.error(getApiErrorMessage(error, 'ลบสินค้า'));
        this.isDeleting.set(false);
      },
    });
  }
}
