import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { SidebarStateService } from '../../shared/services/sidebar-state.service';
import { AuthService } from '../../shared/services/auth.service';
import { SettingsService } from './services/settings.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SidebarComponent, HeaderComponent],
  templateUrl: './settings.html',
  styleUrl: './settings.scss'
})
export class SettingsComponent implements OnInit {
  readonly sidebarState = inject(SidebarStateService);
  private readonly authService = inject(AuthService);
  private readonly settingsService = inject(SettingsService);
  private readonly toastService = inject(ToastService);
  private readonly fb = inject(NonNullableFormBuilder);

  isSavingProfile = signal(false);
  isSavingPassword = signal(false);
  showCurrentPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);

  readonly currencyOptions = [
    { value: 'BRL', label: 'Real Brasileiro (R$)' },
    { value: 'USD', label: 'Dólar Americano ($)' },
    { value: 'EUR', label: 'Euro (€)' },
    { value: 'GBP', label: 'Libra Esterlina (£)' },
    { value: 'JPY', label: 'Iene Japonês (¥)' },
    { value: 'CNY', label: 'Yuan Chinês (¥)' },
    { value: 'AUD', label: 'Dólar Australiano ($)' },
    { value: 'CAD', label: 'Dólar Canadense ($)' },
    { value: 'CHF', label: 'Franco Suíço (Fr)' },
    { value: 'SEK', label: 'Coroa Sueca (kr)' },
    { value: 'NOK', label: 'Coroa Norueguesa (kr)' },
    { value: 'DKK', label: 'Coroa Dinamarquesa (kr)' },
    { value: 'NZD', label: 'Dólar Neozelandês ($)' },
    { value: 'ZAR', label: 'Rand Sul-Africano (R)' },
    { value: 'INR', label: 'Rúpia Indiana (₹)' },
    { value: 'MXN', label: 'Peso Mexicano ($)' },
  ];

  profileForm = this.fb.group({
    nickname: [localStorage.getItem('user_nickname') || '', [Validators.required, Validators.maxLength(50)]],
    email: [localStorage.getItem('user_email') || '', [Validators.required, Validators.email]],
    currency: [this.authService.userCurrency(), Validators.required],
    darkTheme: [false],
  });

  passwordForm = this.fb.group({
    oldPassword: ['', [Validators.required, Validators.minLength(6)]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]],
  });

  ngOnInit(): void { }

  onSaveProfile(): void {
    if (this.profileForm.invalid) return;

    this.isSavingProfile.set(true);
    const formValue = this.profileForm.getRawValue();

    this.settingsService.updateProfile({
      email: formValue.email,
      nickname: formValue.nickname,
      currency: formValue.currency,
      darkTheme: formValue.darkTheme,
    }).subscribe({
      next: () => {
        this.toastService.show('Perfil atualizado com sucesso!', 'success');
        this.isSavingProfile.set(false);
      },
      error: () => {
        this.isSavingProfile.set(false);
      }
    });
  }

  onSavePassword(): void {
    const { oldPassword, newPassword, confirmPassword } = this.passwordForm.getRawValue();
    if (this.passwordForm.invalid) return;
    if (newPassword !== confirmPassword) {
      this.toastService.show('As senhas não coincidem.', 'error');
      return;
    }

    this.isSavingPassword.set(true);

    this.settingsService.updatePassword({ oldPassword, newPassword }).subscribe({
      next: () => {
        this.toastService.show('Senha atualizada com sucesso!', 'success');
        this.passwordForm.reset();
        this.isSavingPassword.set(false);
      },
      error: () => {
        this.isSavingPassword.set(false);
      }
    });
  }

  toggleCurrentPassword(): void { this.showCurrentPassword.update(v => !v); }
  toggleNewPassword(): void { this.showNewPassword.update(v => !v); }
  toggleConfirmPassword(): void { this.showConfirmPassword.update(v => !v); }
}
