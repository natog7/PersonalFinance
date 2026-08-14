import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { LoginRegisterFormComponent } from './components/login-register-form/login-register-form';
import { LoginCardComponent } from './components/login-card/login-card';
import { GeneralService } from '../../shared/services/general.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [LoginRegisterFormComponent, LoginCardComponent],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent implements OnInit {
  private readonly generalService = inject(GeneralService);
  showRegister = signal(false);
  formMode = signal<'register' | 'forgotPassword'>('register');
  isOnline = computed(() => this.generalService.isOnline());
  textOnline = computed(() => this.isOnline() ? 'Online' : 'Offline');
  email = signal('');

  constructor() {
    this.checkOnline();
  }

  ngOnInit() {
    this.checkOnline();
  }

  checkOnline(): void {
    this.generalService.checkOnline();
  }

  openRegister(): void {
    this.formMode.set('register');
    this.showRegister.set(true);
  }

  openForgotPassword(): void {
    this.formMode.set('forgotPassword');
    this.showRegister.set(true);
  }

  closeRegister(): void {
    this.showRegister.set(false);
  }

  registerCompleted(email: string): void {
    this.email.set(email);
    this.closeRegister();
  }
}

