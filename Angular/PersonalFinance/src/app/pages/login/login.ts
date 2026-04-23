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
  isOnline = computed(() => this.generalService.isOnline());
  textOnline = computed(() => this.isOnline() ? 'Online' : 'Offline');

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
    this.showRegister.set(true);
  }

  closeRegister(): void {
    this.showRegister.set(false);
  }
}

