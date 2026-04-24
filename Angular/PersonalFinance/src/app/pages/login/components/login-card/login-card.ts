import { Component, signal, computed, inject, output, input, effect } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../../shared/services/auth.service';
import { LoginField } from '../login-field/login-field';

@Component({
  selector: 'app-login-card',
  standalone: true,
  imports: [ReactiveFormsModule, LoginField],
  templateUrl: './login-card.html',
  styleUrl: './login-card.scss'
})
export class LoginCardComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  email = input<string>();

  openRegister = output<void>();

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  private passwordVisible = signal(false);
  passwordType = computed(() => this.passwordVisible() ? 'text' : 'password');
  passwordIcon = computed(() => this.passwordVisible() ? 'visibility_off' : 'visibility');

  constructor() {
    effect(() => {
      const emailValue = this.email();
      if (emailValue) {
        this.loginForm.patchValue({ email: emailValue });
      }
    });
  }

  togglePasswordVisibility(): void {
    this.passwordVisible.update(value => !value);
  }

  forgotPassword(): void {
    console.log('Forgot password');
  }

  onOpenRegister(event: Event): void {
    event.preventDefault();
    this.openRegister.emit();
  }

  login(): void {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        error: (err) => {
          console.error('Login failed', err);
          alert('E-mail ou senha incorretos.');
        }
      });
    }
  }
}
