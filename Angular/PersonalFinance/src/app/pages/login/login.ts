import { Component, signal } from '@angular/core';
import { LoginRegisterFormComponent } from './components/login-register-form/login-register-form';
import { LoginCardComponent } from './components/login-card/login-card';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [LoginRegisterFormComponent, LoginCardComponent],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  showRegister = signal(false);

  openRegister(): void {
    this.showRegister.set(true);
  }

  closeRegister(): void {
    this.showRegister.set(false);
  }
}

