import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: number;
  message: string;
  type: ToastType;
  isClosing?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toastIdCounter = 0;
  private timeouts = new Map<number, any>();
  readonly toasts = signal<Toast[]>([]);

  show(message: string, type: ToastType = 'info', duration: number = 4000) {
    const id = this.toastIdCounter++;
    const newToast: Toast = { id, message, type };

    this.toasts.update(current => [...current, newToast]);

    if (duration > 0) {
      const timeoutId = setTimeout(() => this.remove(id), duration);
      this.timeouts.set(id, timeoutId);
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

  httpErrorCustom(statusCode: number, messages: { code: number; message: string }[], duration?: number) {
    for (const message of messages) {
      if (message.code === statusCode) {
        this.show(message.message, 'error', duration);
        return;
      }
    }
    this.httpError(statusCode, '', duration);
  }

  info(message: string, duration?: number) {
    this.show(message, 'info', duration);
  }

  warning(message: string, duration?: number) {
    this.show(message, 'warning', duration);
  }

  remove(id: number) {
    if (this.timeouts.has(id)) {
      clearTimeout(this.timeouts.get(id));
      this.timeouts.delete(id);
    }

    const toast = this.toasts().find(t => t.id === id);
    if (toast && !toast.isClosing) {
      this.toasts.update(current =>
        current.map(t => t.id === id ? { ...t, isClosing: true } : t)
      );
      setTimeout(() => {
        this.toasts.update(current => current.filter(t => t.id !== id));
      }, 300); // 300ms matches the fadeOut animation duration
    }
  }
}
