import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ToastService } from '../../../shared/services/toast.service';
import { AuthService } from '../../../shared/services/auth.service';
import { tap, catchError, throwError } from 'rxjs';

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
      })
    );
  }

  updatePassword(payload: UpdatePasswordPayload) {
    this.toastService.success('Senha atualizada com sucesso!');
    return this.http.put(`${this.apiUrl}/password`, payload);
  }
}
