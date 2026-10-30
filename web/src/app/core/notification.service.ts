import { Injectable, signal } from '@angular/core';

export interface Notification {
  message: string;
  type: 'success' | 'error';
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private timeoutId: ReturnType<typeof setTimeout> | null = null;
  readonly notification = signal<Notification | null>(null);

  success(message: string): void {
    this.show(message, 'success');
  }

  error(message: string): void {
    this.show(message, 'error');
  }

  private show(message: string, type: Notification['type']): void {
    if (this.timeoutId !== null) {
      clearTimeout(this.timeoutId);
    }

    this.notification.set({ message, type });
    this.timeoutId = setTimeout(() => {
      this.notification.set(null);
      this.timeoutId = null;
    }, 4200);
  }
}
