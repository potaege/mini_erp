import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { getApiErrorMessage } from '../../core/api-error';
import { ProductLog } from '../../core/models';
import { ProductsService } from '../../core/products.service';

interface ProductLogView extends ProductLog {
  productName: string;
  actionLabel: string;
  valueDescription: string;
}

@Component({
  selector: 'app-product-logs-page',
  imports: [DatePipe, RouterLink],
  template: `
    <section class="page">
      <a class="back-link" [routerLink]="backLink()"><span aria-hidden="true">←</span> {{ productId() ? 'กลับไปหน้าสินค้า' : 'กลับไปหน้ารายการสินค้า' }}</a>
      <div class="page-heading">
        <div>
          <p class="eyebrow">ACTIVITY HISTORY</p>
          <h1>{{ productId() ? 'ประวัติสินค้า' : 'ประวัติการเปลี่ยนแปลง' }}</h1>
          <p class="subtitle">{{ productName() ? productName() + ' · ' + (productSku() ?? '') : 'บันทึกการเพิ่ม แก้ไข และเปลี่ยนแปลงสินค้า' }}</p>
        </div>
      </div>
      @if (errorMessage()) {
        <div class="alert" role="alert">{{ errorMessage() }}</div>
      }
      <section class="card history-card">
        @if (isLoading()) {
          <div class="loading">กำลังโหลดประวัติ...</div>
        } @else if (logs().length === 0) {
          <div class="empty-state"><strong>ยังไม่มีประวัติการเปลี่ยนแปลง</strong><p>รายการบันทึกจะแสดงที่นี่เมื่อมีการเปลี่ยนแปลงสินค้า</p></div>
        } @else {
          <div class="table-wrap">
            <table>
              <thead><tr>@if (!productId()) { <th>สินค้า</th> }<th>กิจกรรม</th><th>รายละเอียด</th><th>ผู้ดำเนินการ</th><th>วันและเวลา</th></tr></thead>
              <tbody>
                @for (log of logs(); track log.id) {
                  <tr>
                    @if (!productId()) { <td><a class="product-link" [routerLink]="['/products', log.productId]">{{ log.productName }}</a><span class="sku">{{ skuFor(log) }}</span></td> }
                    <td><span class="log-type" [class.log-create]="log.type === 0" [class.log-delete]="log.type === 6">{{ log.actionLabel }}</span></td>
                    <td class="detail">{{ log.valueDescription }}</td>
                    <td class="user-id">{{ log.userName }} <span>(#{{ log.userId }})</span></td>
                    <td class="date">{{ log.createdAt | date:'dd/MM/yyyy HH:mm:ss' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          <div class="table-footer">แสดง {{ logs().length }} รายการ</div>
        }
      </section>
    </section>
  `,
  styles: `
    .back-link { display: inline-flex; gap: 8px; margin-bottom: 22px; color: #778397; font-size: 12px; font-weight: 600; }
    .back-link:hover, .product-link:hover { color: #3559e8; }
    .history-card { overflow: hidden; }
    .product-link { display: block; font-weight: 700; }
    .sku { display: block; margin-top: 4px; color: #8490a2; font: 11px ui-monospace, SFMono-Regular, Consolas, monospace; }
    .log-type { display: inline-block; border-radius: 5px; padding: 5px 8px; background: #eef1ff; color: #405bd1; font-size: 10px; font-weight: 700; white-space: nowrap; }
    .log-create { background: #e8f6ef; color: #21865b; }
    .log-delete { background: #fff0f1; color: #c43e49; }
    .detail { color: #667085; }
    .user-id, .date { color: #8490a2; font-size: 11px; white-space: nowrap; }
    .table-footer { border-top: 1px solid #eff1f5; padding: 12px 19px; color: #8490a2; font-size: 10px; }
    .empty-state { padding: 58px 20px; color: #344054; text-align: center; }
    .empty-state p { margin: 8px 0 0; color: #8490a2; font-size: 12px; }
  `,
})
export class ProductLogsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly productsService = inject(ProductsService);

  protected readonly productId = signal<number | null>(null);
  protected readonly productName = signal<string | null>(null);
  protected readonly productSku = signal<string | null>(null);
  protected readonly logs = signal<ProductLogView[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal('');

  ngOnInit(): void {
    const idValue = this.route.snapshot.paramMap.get('id');
    if (idValue === null) {
      this.loadAllLogs();
      return;
    }

    const id = Number(idValue);
    if (!Number.isInteger(id) || id < 1) {
      this.errorMessage.set('ไม่พบรหัสสินค้า');
      this.isLoading.set(false);
      return;
    }

    this.productId.set(id);
    this.loadLogs(this.productsService.getLogsByProductId(id));
  }

  private loadAllLogs(): void {
    this.loadLogs(this.productsService.getLogs());
  }

  private loadLogs(request: ReturnType<ProductsService['getLogs']>): void {
    request.subscribe({
      next: (logs) => this.setLogData(logs),
      error: (error: unknown) => this.handleLoadError(error),
      complete: () => this.isLoading.set(false),
    });
  }

  private setLogData(logs: ProductLog[]): void {
    const selectedProductLog = logs.find((log) => log.productId === this.productId());
    if (selectedProductLog) {
      this.productName.set(
        selectedProductLog.productName
          ?? (selectedProductLog.type === 6 ? selectedProductLog.newValue : null),
      );
      this.productSku.set(
        selectedProductLog.productSku
          ?? (selectedProductLog.type === 6 ? selectedProductLog.oldValue : null),
      );
    }
    this.logs.set(logs.map((log) => this.toLogView(log)));
    this.errorMessage.set('');
  }

  private handleLoadError(error: unknown): void {
    this.errorMessage.set(getApiErrorMessage(error, 'โหลดประวัติสินค้า'));
    this.isLoading.set(false);
  }

  protected backLink(): string[] {
    const id = this.productId();
    return id === null ? ['/products'] : ['/products', String(id)];
  }

  protected skuFor(log: ProductLog): string {
    return log.productSku
      ?? (log.type === 0 ? log.newValue : log.type === 6 ? log.oldValue : null)
      ?? `#${log.productId}`;
  }

  private toLogView(log: ProductLog): ProductLogView {
    const labels = ['เพิ่มสินค้า', 'เปลี่ยนชื่อ', 'เปลี่ยนรหัสสินค้า', 'เปลี่ยนราคา', 'เพิ่มสต็อก', 'ลดสต็อก', 'ลบสินค้า'];
    const oldValue = log.oldValue ?? '—';
    const newValue = log.newValue ?? '—';
    const details = log.changeQuantity === null
      ? `${oldValue} → ${newValue}`
      : `${log.changeQuantity > 0 ? '+' : ''}${log.changeQuantity} ชิ้น (${oldValue} → ${newValue})`;

    return {
      ...log,
      productName: log.productName
        ?? (log.type === 6 ? log.newValue : null)
        ?? `สินค้า #${log.productId}`,
      actionLabel: labels[log.type] ?? 'เปลี่ยนแปลงสินค้า',
      valueDescription: details,
    };
  }
}
