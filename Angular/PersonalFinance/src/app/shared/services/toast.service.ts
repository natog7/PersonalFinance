import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toastIdCounter = 0;
  readonly toasts = signal<Toast[]>([]);

  show(message: string, type: ToastType = 'info', duration: number = 3000) {
    const id = this.toastIdCounter++;
    const newToast: Toast = { id, message, type };

    this.toasts.update(current => [...current, newToast]);

    if (duration > 0) {
      setTimeout(() => this.remove(id), duration);
    }
  }

  success(message: string, duration?: number) {
    this.show(message, 'success', duration);
  }

  error(message: string, duration?: number) {
    this.show(message, 'error', duration);
  }

  httpError(statusCode: number, message: string, duration?: number) {
    switch (statusCode) {
      case 0:
        message = "Erro de conexão. O servidor pode estar offline.";
        break;
      case 400:
        message = "Dados inválidos. Verifique os campos e tente novamente.";
        break;
      case 401:
        message = "Sessão expirada. Faça login novamente.";
        break;
      case 403:
        message = "Você não tem permissão para realizar esta ação.";
        break;
      case 404:
        message = "Não encontrado. Verifique e tente novamente.";
        break;
      case 500:
        message = "Erro interno. Tente novamente mais tarde.";
        break;
      default:
        message = message ?? "Ocorreu um erro. Tente novamente mais tarde.";
        break;
    }
    this.show(message, 'error', duration);
  }

  info(message: string, duration?: number) {
    this.show(message, 'info', duration);
  }

  warning(message: string, duration?: number) {
    this.show(message, 'warning', duration);
  }

  remove(id: number) {
    this.toasts.update(current => current.filter(t => t.id !== id));
  }
}
