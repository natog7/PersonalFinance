import { Component, signal, computed, inject, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { LoginField } from '../login-field/login-field';
import { AuthService } from '../../../../shared/services/auth.service';

function passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
  const password = control.get('password');
  const confirm = control.get('confirmPassword');
  if (password && confirm && password.value !== confirm.value) {
    return { passwordMismatch: true };
  }
  return null;
}

function passwordVisibility() {
  const visible = signal(false);
  return {
    visible,
    type: computed(() => visible() ? 'text' : 'password'),
    icon: computed(() => visible() ? 'visibility_off' : 'visibility'),
    toggle: () => visible.update(v => !v)
  };
}

@Component({
  selector: 'app-login-register-form',
  standalone: true,
  imports: [ReactiveFormsModule, LoginField],
  templateUrl: './login-register-form.html',
  styleUrl: './login-register-form.scss'
})
export class LoginRegisterFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  backToLogin = output<void>();

  registerForm: FormGroup = this.fb.group({
    nickname: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  password = passwordVisibility();
  confirmPassword = passwordVisibility();

  register(): void {
    if (this.registerForm.valid) {
      const { email, password, nickname } = this.registerForm.value;
      this.authService.register({ email, password, nickname }).subscribe({
        next: () => this.goBack(),
        error: (err) => {
          console.error('Register failed', err);
          alert('Erro ao criar conta. Tente novamente.');
        }
      });
    }
  }

  goBack(): void {
    this.backToLogin.emit();
  }
}