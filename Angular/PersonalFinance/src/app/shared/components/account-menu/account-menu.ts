import { Component, Output, EventEmitter, input, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-account-menu',
  standalone: true,
  imports: [],
  templateUrl: './account-menu.html',
  styleUrl: './account-menu.scss'
})
export class AccountMenu {
  private readonly authService = inject(AuthService);

  isOpen = input<boolean>(false);
  @Output() logout = new EventEmitter<void>();
  @Output() settings = new EventEmitter<void>();

  get nickname(): string {
    return localStorage.getItem('user_nickname') || 'Usuário';
  }

  get email(): string {
    return localStorage.getItem('user_email') || '';
  }

  onLogout(): void {
    this.authService.logout();
    this.logout.emit();
  }
}
