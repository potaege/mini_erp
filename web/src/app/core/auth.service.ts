import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthSession, LoginRequest } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly sessionState = signal<AuthSession | null>(null);

  readonly session = this.sessionState.asReadonly();
  readonly isAuthenticated = computed(() => this.sessionState() !== null);

  login(request: LoginRequest): Observable<AuthSession> {
    return this.http
      .post<AuthSession>('/api/Auth/login', request)
      .pipe(tap((session) => this.sessionState.set(session)));
  }

  logout(): void {
    this.sessionState.set(null);
  }
}
