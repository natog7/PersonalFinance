import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

interface AuthResponse {
  userId: string;
  email: string;
  nickname: string;
  token: {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
    tokenType: string;
  };
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = 'https://localhost:55784/api/auth/login';

  isAuthenticated = signal<boolean>(!!localStorage.getItem('access_token'));

  login(credentials: { email: string; password: string }) {
    return this.http.post<AuthResponse>(this.apiUrl, credentials).pipe(
      tap((response) => {
        localStorage.setItem('access_token', response.token.accessToken);
        localStorage.setItem('refresh_token', response.token.refreshToken);
        localStorage.setItem('user_nickname', response.nickname);
        localStorage.setItem('user_email', response.email);
        this.isAuthenticated.set(true);
        this.router.navigate(['/home']);
      })
    );
  }

  logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_nickname');
    localStorage.removeItem('user_email');
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }
}
