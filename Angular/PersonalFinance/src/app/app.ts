import { Component, inject } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { ThemeService } from './shared/services/theme.service';
import { ToastComponent } from './shared/components/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [IonApp, IonRouterOutlet, ToastComponent],
  template: `
    <ion-app>
      <ion-router-outlet></ion-router-outlet>
      <app-toast></app-toast>
    </ion-app>
  `,
  styles: [':host { display: block; height: 100vh; }'],
})
export class App {
  private readonly themeService = inject(ThemeService);
}
