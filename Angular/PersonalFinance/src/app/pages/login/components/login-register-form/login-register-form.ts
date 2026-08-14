import { Component, signal, computed, inject, output, input, effect } from '@angular/core';
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

  mode = input<'register' | 'forgotPassword'>('register');

  backToLogin = output<void>();
  registerCompleted = output<string>();

  cardTitle = computed(() => this.mode() === 'register' ? 'Criar Conta' : 'Esqueceu a Senha?');
  cardSubtitle = computed(() => this.mode() === 'register' ? 'Comece sua jornada financeira' : 'Informe seus dados para redefinir a senha');
  cardIcon = computed(() => this.mode() === 'register' ? 'person_add' : 'lock_reset');
  passwordLabel = computed(() => this.mode() === 'register' ? 'Senha' : 'Nova Senha');
  confirmPasswordLabel = computed(() => this.mode() === 'register' ? 'Confirmar Senha' : 'Confirmar Nova Senha');
  submitButtonText = computed(() => this.mode() === 'register' ? 'Criar Conta' : 'Redefinir Senha');
  submitButtonIcon = computed(() => this.mode() === 'register' ? 'arrow_forward' : 'lock_reset');

  registerForm: FormGroup = this.fb.group({
    nickname: [''],
    email: ['', [Validators.required, Validators.email]],
    verifyCode: [''],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])/)
    ]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  password = passwordVisibility();
  confirmPassword = passwordVisibility();

  constructor() {
    effect(() => {
      const currentMode = this.mode();
      this.registerForm.reset();
      if (currentMode === 'register') {
        this.registerForm.get('nickname')?.setValidators([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(128)
        ]);
        this.registerForm.get('verifyCode')?.clearValidators();
      } else {
        this.registerForm.get('nickname')?.clearValidators();
        this.registerForm.get('verifyCode')?.setValidators([
          Validators.required
        ]);
      }
      this.registerForm.get('nickname')?.updateValueAndValidity();
      this.registerForm.get('verifyCode')?.updateValueAndValidity();
    });
  }

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

  onSubmit(): void {
    if (this.registerForm.valid) {
      const { email, password, nickname, verifyCode } = this.registerForm.value;
      if (this.mode() === 'register') {
        this.authService.register({ email, password, nickname }).subscribe({
          next: () => {
            this.registerCompleted.emit(email);
            this.toastService.success('Registro realizado com sucesso!');
          },
          error: (err: HttpErrorResponse) => {
            this.toastService.httpError(err.status, 'Erro ao registrar.');
          }
        });
      } else {
        this.authService.forgotPassword({ email, verifyCode, password }).subscribe({
          next: () => {
            this.registerCompleted.emit(email);
            this.toastService.success('Senha redefinida com sucesso!');
          },
          error: (err: HttpErrorResponse) => {
            this.toastService.httpError(err.status, 'Erro ao redefinir a senha.');
          }
        });
      }
    } else {
      this.registerForm.markAllAsTouched();
    }
  }

  sendVerificationCode(): void {
    const emailControl = this.registerForm.get('email');
    if (!emailControl?.value || emailControl.hasError('email') || emailControl.hasError('required')) {
      emailControl?.markAsTouched();
      this.toastService.error('Informe um e-mail válido para enviar o código.');
      return;
    }

    this.authService.sendVerificationCode(emailControl.value).subscribe({
      next: () => {
        this.toastService.success('Código de verificação enviado para o e-mail!');
      },
      error: (err: HttpErrorResponse) => {
        this.toastService.httpError(err.status, 'Erro ao enviar código de verificação.');
      }
    });
  }

  goBack(): void {
    this.backToLogin.emit();
  }
}