import { Component, inject, signal } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter, map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { DashboardService } from '../../../pages/home/services/dashboard.service';
import { SidebarStateService } from '../../services/sidebar-state.service';
import { ThemeService } from '../../services/theme.service';
import { AccountMenu } from '../account-menu/account-menu';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [AccountMenu],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly router = inject(Router);
  private readonly dashboardService = inject(DashboardService);
  readonly sidebarState = inject(SidebarStateService);
  readonly themeService = inject(ThemeService);

  pageTitle = toSignal(
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map(() => this.getRouteTitle())
    ),
    { initialValue: 'Painel Principal' }
  );

  balanceHidden = signal(false);
  searchQuery = signal('');
  isAccountMenuOpen = signal(false);

  toggleAccountMenu(): void {
    this.isAccountMenuOpen.update(v => !v);
  }

  navigateToSettings(): void {
    this.isAccountMenuOpen.set(false);
    this.router.navigate(['/settings']);
  }

  toggleMenu(): void {
    this.sidebarState.toggle();
  }

  // toggleBalance(): void {
  //   this.balanceHidden.update((v) => !v);
  //   this.dashboardService.toggleBalanceVisibility(this.balanceHidden());
  // }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }

  private getRouteTitle(): string {
    const url = this.router.url;
    if (url.includes('/home')) return 'Painel Principal';
    if (url.includes('/categories')) return 'Categorias';
    if (url.includes('/transactions')) return 'Transações';
    if (url.includes('/projection')) return 'Projeção Financeira';
    return 'Personal Finance';
  }
}
