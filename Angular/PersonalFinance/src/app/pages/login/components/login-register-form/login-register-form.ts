import { Component, inject, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';

function passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
  const password = control.get('password');
  const confirm = control.get('confirmPassword');
  if (password && confirm && password.value !== confirm.value) {
    return { passwordMismatch: true };
  }
  return null;
}

@Component({
  selector: 'app-login-register-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login-register-form.html',
  styleUrl: './login-register-form.scss'
})
export class LoginRegisterFormComponent {
  private readonly fb = inject(FormBuilder);

  backToLogin = output<void>();

  registerForm: FormGroup = this.fb.group({
    nickname: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  passwordVisible = false;
  confirmPasswordVisible = false;

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  toggleConfirmPasswordVisibility(): void {
    this.confirmPasswordVisible = !this.confirmPasswordVisible;
  }

  register(): void {
    if (this.registerForm.valid) {
      console.log('Register:', this.registerForm.value);
    }
  }

  goBack(): void {
    this.backToLogin.emit();
  }
}
