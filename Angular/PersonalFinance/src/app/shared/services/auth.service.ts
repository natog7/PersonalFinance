import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

interface AuthResponse {
  userId: string;
  email: string;
  nickname: string;
  currency: string;
  token: {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
    tokenType: string;
  };
}

interface RegisterResponse {
  userId: string;
  email: string;
  nickname: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = 'https://localhost:55784/api/auth';

  isAuthenticated = signal<boolean>(!!localStorage.getItem('access_token'));
  userCurrency = signal<string>(localStorage.getItem('user_currency') || 'BRL');

  login(credentials: { email: string; password: string }) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((response) => {
        localStorage.setItem('access_token', response.token.accessToken);
        localStorage.setItem('refresh_token', response.token.refreshToken);
        this.updateUserInfo({
          nickname: response.nickname,
          email: response.email,
          currency: response.currency || 'BRL'
        });
        this.isAuthenticated.set(true);
        this.router.navigate(['/home']);
      })
    );
  }

  register(credentials: { email: string; password: string; nickname: string }) {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, credentials);
  }

  forgotPassword(credentials: { email: string; verifyCode: string; password: string }) {
    return this.http.post(`${this.apiUrl}/reset-password`, credentials);
  }

  sendVerificationCode(email: string) {
    return this.http.post(`${this.apiUrl}/send-verification-code`, { email });
  }

  logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_nickname');
    localStorage.removeItem('user_email');
    localStorage.removeItem('user_currency');
    this.isAuthenticated.set(false);
    this.userCurrency.set('BRL');
    this.router.navigate(['/login']);
  }

  updateUserInfo(data: { nickname: string; email: string; currency: string }) {
    localStorage.setItem('user_nickname', data.nickname);
    localStorage.setItem('user_email', data.email);
    localStorage.setItem('user_currency', data.currency || 'BRL');
    this.userCurrency.set(data.currency || 'BRL');
  }
}
