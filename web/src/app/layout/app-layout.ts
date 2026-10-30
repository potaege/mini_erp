import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { NotificationService } from '../core/notification.service';

@Component({
  selector: 'app-layout',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <header class="topbar">
      <a class="brand" routerLink="/products" aria-label="Mini ERP หน้าสินค้า">
        <span class="brand-mark">M</span>
        <span>mini<span class="brand-light">erp</span></span>
      </a>
      <nav class="navigation" aria-label="เมนูหลัก">
        <a routerLink="/products" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">
          สินค้า
        </a>
        <a routerLink="/logs" routerLinkActive="active">ประวัติทั้งหมด</a>
      </nav>
      <button class="user-menu" type="button" (click)="logout()">
        <span class="user-avatar">U</span>
        <span class="user-label">ออกจากระบบ</span>
      </button>
    </header>
    @if (notifications.notification(); as notification) {
      <div class="toast" [class.toast-error]="notification.type === 'error'" role="status">
        {{ notification.message }}
      </div>
    }
    <main>
      <router-outlet />
    </main>
  `,
  styles: `
    .topbar { height: 68px; display: flex; align-items: center; gap: 48px; padding: 0 max(36px, calc((100vw - 1260px) / 2)); border-bottom: 1px solid #e8ecf2; background: #fff; }
    .brand { display: flex; align-items: center; gap: 10px; font: 800 18px 'Segoe UI', Arial, sans-serif; letter-spacing: -.06em; }
    .brand-mark { width: 31px; height: 31px; display: grid; place-items: center; border-radius: 9px; background: #3559e8; color: white; font-size: 15px; }
    .brand-light { color: #778397; font-weight: 500; }
    .navigation { display: flex; align-self: stretch; gap: 26px; }
    .navigation a { position: relative; display: grid; place-items: center; color: #778397; font-size: 13px; font-weight: 600; }
    .navigation a.active { color: #3559e8; }
    .navigation a.active::after { position: absolute; right: 0; bottom: -1px; left: 0; height: 2px; background: #3559e8; content: ''; }
    .user-menu { display: flex; align-items: center; gap: 9px; margin-left: auto; border: 0; background: transparent; color: #667085; font-size: 12px; font-weight: 600; }
    .user-avatar { width: 29px; height: 29px; display: grid; place-items: center; border-radius: 50%; background: #eef1ff; color: #3559e8; font-weight: 700; }
    @media (max-width: 650px) {
      .topbar { height: 60px; gap: 20px; padding: 0 16px; }
      .navigation { gap: 15px; }
      .navigation a { font-size: 12px; }
      .user-label { display: none; }
    }
  `,
})
export class AppLayout {
  protected readonly notifications = inject(NotificationService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  logout(): void {
    this.auth.logout();
    void this.router.navigate(['/login']);
  }
}
