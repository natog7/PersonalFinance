import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('access_token');

  // Do not add Authorization header to the login request
  const isLoginRequest = req.url.includes('/api/auth/login');

  let processedReq = req;
  if (token && !isLoginRequest) {
    processedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(processedReq).pipe(
    catchError((error) => {
      // If unauthorized and it's not the login request itself
      if (error.status === 401 && !isLoginRequest) {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('user_nickname');
        localStorage.removeItem('user_email');
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};