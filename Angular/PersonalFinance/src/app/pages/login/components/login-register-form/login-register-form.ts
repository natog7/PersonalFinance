import { Component, signal, computed, inject, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { InputField } from '../../../../shared/components/input-field/input-field';
import { AuthService } from '../../../../shared/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastService } from '../../../../shared/services/toast.service';

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
  imports: [ReactiveFormsModule, InputField],
  templateUrl: './login-register-form.html',
  styleUrl: './login-register-form.scss'
})
export class LoginRegisterFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);

  backToLogin = output<void>();
  registerCompleted = output<string>();

  registerForm: FormGroup = this.fb.group({
    nickname: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(128)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])/)
    ]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  password = passwordVisibility();
  confirmPassword = passwordVisibility();

  getErrorMessage(controlName: string): string | undefined {
    const control = this.registerForm.get(controlName);
    if (!control || !(control.touched || control.dirty)) return undefined;

    if (control.hasError('required')) return 'Campo obrigatório';
    if (control.hasError('email')) return 'Email inválido';
    if (control.hasError('maxlength')) return 'Máximo de 128 caracteres';
    if (control.hasError('minlength')) {
      if (controlName === 'nickname') return 'Mínimo de 3 caracteres';
      else return 'Mínimo de 8 caracteres';
    }
    if (controlName === 'password' && control.hasError('pattern')) {
      return 'Deve conter 1 maiúscula, 1 número e 1 caractere especial';
    }

    if (controlName === 'confirmPassword' && this.registerForm.hasError('passwordMismatch')) {
      return 'As senhas não coincidem';
    }

    return undefined;
  }

  register(): void {
    if (this.registerForm.valid) {
      const { email, password, nickname } = this.registerForm.value;
      this.authService.register({ email, password, nickname }).subscribe({
        next: () => this.registerCompleted.emit(email),
        error: (err: HttpErrorResponse) => {
          this.toastService.httpError(err.status, 'Erro ao criar conta.');
        }
      });
    } else {
      this.registerForm.markAllAsTouched();
    }
  }

  goBack(): void {
    this.backToLogin.emit();
  }
}