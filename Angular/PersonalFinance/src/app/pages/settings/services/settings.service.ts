import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ToastService } from '../../../shared/services/toast.service';
import { AuthService } from '../../../shared/services/auth.service';
import { catchError, tap, throwError } from 'rxjs';

export interface UpdateProfilePayload {
  email: string;
  password?: string;
  nickname: string;
  currency: string | null;
  darkTheme: boolean | null;
}

export interface EditProfileResponse {
  email: string;
  nickname: string;
  currency: string;
  darkTheme: boolean;
  id: string;
}

export interface UpdatePasswordPayload {
  currentPassword: string;
  newPassword: string;
}

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private readonly http = inject(HttpClient);
  private readonly toastService = inject(ToastService);
  private readonly authService = inject(AuthService);
  private readonly apiUrl = 'https://localhost:55784/api/auth';

  updateProfile(payload: UpdateProfilePayload) {
    const requestPayload = {
      ...payload,
      password: payload.password || ''
    };
    return this.http.put<EditProfileResponse>(`${this.apiUrl}/edit`, requestPayload).pipe(
      tap((response) => {
        this.authService.updateUserInfo({
          nickname: response.nickname,
          email: response.email,
          currency: response.currency
        });
        this.toastService.success('Perfil atualizado com sucesso!');
      }),
      catchError((err: HttpErrorResponse) => {
        this.toastService.httpError(err.status, 'Erro ao atualizar perfil.');
        return throwError(() => err);
      })
    );
  }

  updatePassword(payload: UpdatePasswordPayload) {
    return this.http.put(`${this.apiUrl}/password`, payload).pipe(
      tap(() => {
        this.toastService.success('Senha atualizada com sucesso!');
      }),
      catchError((err: HttpErrorResponse) => {
        this.toastService.httpError(err.status, 'Erro ao atualizar senha.');
        return throwError(() => err);
      })
    );
  }
}
