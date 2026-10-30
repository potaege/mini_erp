import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { getApiErrorMessage } from '../../core/api-error';
import { Product } from '../../core/models';
import { ProductsService } from '../../core/products.service';

@Component({
  selector: 'app-products-page',
  imports: [DecimalPipe, RouterLink],
  template: `
    <section class="page">
      <div class="page-heading">
        <div>
          <p class="eyebrow">INVENTORY</p>
          <h1>สินค้า</h1>
          <p class="subtitle">จัดการรายการสินค้าและตรวจสอบจำนวนคงเหลือ</p>
        </div>
        <a class="button button-primary" routerLink="/products/new"><span aria-hidden="true">＋</span> เพิ่มสินค้า</a>
      </div>

      @if (errorMessage()) {
        <div class="alert" role="alert">{{ errorMessage() }}</div>
      }

      <div class="summary-row">
        <div class="summary-card card">
          <span class="summary-icon">▦</span>
          <span class="summary-label">สินค้าทั้งหมด</span>
          <strong>{{ products().length }}</strong>
          <span class="summary-foot">รายการในคลังสินค้า</span>
        </div>
      </div>

      <section class="card catalog-card">
        <div class="catalog-toolbar">
          <div>
            <h2>รายการสินค้า</h2>
            <p>ค้นหาด้วยชื่อหรือรหัสสินค้า</p>
          </div>
          <label class="search-box">
            <span aria-hidden="true">⌕</span>
            <input type="search" placeholder="ค้นหาชื่อ หรือ SKU..." [value]="search()" (input)="updateSearch($event)">
            @if (search()) {
              <button type="button" aria-label="ล้างคำค้น" (click)="search.set('')">×</button>
            }
          </label>
        </div>

        @if (isLoading()) {
          <div class="loading">กำลังโหลดรายการสินค้า...</div>
        } @else if (filteredProducts().length === 0) {
          <div class="empty-state">
            <span class="empty-icon">▦</span>
            <strong>{{ search() ? 'ไม่พบสินค้าที่ค้นหา' : 'ยังไม่มีสินค้าในระบบ' }}</strong>
            <p>{{ search() ? 'ลองค้นหาด้วยชื่อหรือรหัสสินค้าอื่น' : 'เริ่มต้นด้วยการเพิ่มสินค้าใหม่' }}</p>
            @if (!search()) {
              <a class="button button-primary" routerLink="/products/new">เพิ่มสินค้า</a>
            }
          </div>
        } @else {
          <div class="table-wrap">
            <table>
              <thead>
                <tr><th>สินค้า</th><th>รหัสสินค้า</th><th>ราคาต่อหน่วย</th><th>คงเหลือ</th><th>สถานะ</th><th></th></tr>
              </thead>
              <tbody>
                @for (product of filteredProducts(); track product.id) {
                  <tr>
                    <td><a class="product-name" [routerLink]="['/products', product.id]">{{ product.name }}</a></td>
                    <td class="mono">{{ product.sku }}</td>
                    <td class="price">{{ product.price | number:'1.2-2' }}</td>
                    <td>{{ product.stockQuantity | number }}</td>
                    <td><span class="badge" [class.badge-green]="product.isActive" [class.badge-gray]="!product.isActive">{{ product.isActive ? 'ใช้งาน' : 'ปิดใช้งาน' }}</span></td>
                    <td class="actions"><a class="button button-link" [routerLink]="['/products', product.id]">ดูรายละเอียด <span aria-hidden="true">→</span></a></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          <div class="table-footer">แสดง {{ filteredProducts().length }} รายการ</div>
        }
      </section>
    </section>
  `,
  styles: `
    .summary-row { display: grid; grid-template-columns: minmax(0, 1fr); margin-bottom: 23px; }
    .summary-card { position: relative; display: grid; grid-template-columns: 39px 1fr; align-items: center; column-gap: 13px; padding: 17px 19px; }
    .summary-icon { grid-row: span 3; width: 37px; height: 37px; display: grid; place-items: center; border-radius: 10px; background: #eef1ff; color: #3559e8; font-size: 19px; }
    .summary-label { color: #778397; font-size: 11px; }
    .summary-card strong { margin-top: 3px; font: 800 21px 'Segoe UI', Arial, sans-serif; }
    .summary-foot { margin-top: 3px; color: #9aa4b2; font-size: 10px; }
    .catalog-card { overflow: hidden; }
    .catalog-toolbar { display: flex; align-items: center; justify-content: space-between; gap: 18px; padding: 19px 21px; }
    h2 { margin: 0; font: 700 15px 'Segoe UI', Arial, sans-serif; }
    .catalog-toolbar p { margin: 5px 0 0; color: #8490a2; font-size: 11px; }
    .search-box { width: min(320px, 50%); height: 39px; display: flex; align-items: center; gap: 8px; border: 1px solid #e0e5ed; border-radius: 8px; padding: 0 10px; color: #98a2b3; }
    .search-box > span { font-size: 21px; line-height: 1; transform: rotate(-20deg); }
    .search-box input { width: 100%; border: 0; outline: 0; color: #344054; font-size: 12px; }
    .search-box button { border: 0; background: transparent; color: #8490a2; font-size: 18px; }
    .product-name { font-weight: 700; }
    .product-name:hover { color: #3559e8; }
    .table-footer { border-top: 1px solid #eff1f5; padding: 12px 19px; color: #8490a2; font-size: 10px; }
    .empty-state { display: grid; justify-items: center; gap: 9px; }
    .empty-state strong { color: #344054; font-size: 14px; }
    .empty-state p { margin: 0 0 8px; font-size: 12px; }
    .empty-icon { color: #aab5cb; font-size: 29px; }
    @media (max-width: 650px) {
      .summary-card { padding: 13px 11px; grid-template-columns: 31px 1fr; gap: 10px; }
      .summary-icon { width: 30px; height: 30px; }
      .catalog-toolbar { align-items: stretch; flex-direction: column; }
      .search-box { width: 100%; }
    }
  `,
})
export class ProductsPage implements OnInit {
  private readonly productsService = inject(ProductsService);
  protected readonly products = signal<Product[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly filteredProducts = computed(() => {
    const query = this.search().trim().toLocaleLowerCase();
    return query
      ? this.products().filter((product) =>
          product.name.toLocaleLowerCase().includes(query)
          || product.sku.toLocaleLowerCase().includes(query),
        )
      : this.products();
  });
  ngOnInit(): void {
    this.loadProducts();
  }

  protected updateSearch(event: Event): void {
    this.search.set((event.target as HTMLInputElement).value);
  }

  private loadProducts(): void {
    this.isLoading.set(true);
    this.productsService.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
        this.errorMessage.set('');
      },
      error: (error: unknown) => {
        this.errorMessage.set(getApiErrorMessage(error, 'โหลดรายการสินค้า'));
        this.isLoading.set(false);
      },
      complete: () => this.isLoading.set(false),
    });
  }
}
