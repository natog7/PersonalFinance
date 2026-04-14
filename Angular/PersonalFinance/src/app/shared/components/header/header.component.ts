import { Component, inject, signal } from '@angular/core';
import { DashboardService } from '../../../pages/home/services/dashboard.service';
import { SidebarStateService } from '../../services/sidebar-state.service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NgClass],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly dashboardService = inject(DashboardService);
  readonly sidebarState = inject(SidebarStateService);

  balanceHidden = signal(false);
  searchQuery = signal('');

  toggleMenu(): void {
    this.sidebarState.toggle();
  }

  toggleBalance(): void {
    this.balanceHidden.update((v) => !v);
    this.dashboardService.toggleBalanceVisibility(this.balanceHidden());
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }
}
