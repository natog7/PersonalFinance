import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = 'https://localhost:55784/api/auth/login';

  isAuthenticated = signal<boolean>(!!localStorage.getItem('access_token'));

  login(credentials: { email: string; password: string }) {
    return this.http.post<{ token: string }>(this.apiUrl, credentials).pipe(
      tap((response) => {
        localStorage.setItem('access_token', response.token);
        this.isAuthenticated.set(true);
        this.router.navigate(['/home']);
      })
    );
  }

  logout() {
    localStorage.removeItem('access_token');
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }
}
