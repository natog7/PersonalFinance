import { Component, inject, signal } from '@angular/core';
import { DashboardService } from '../../../pages/home/services/dashboard.service';


@Component({
  selector: 'app-header',
  standalone: true,
  imports: [],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  private readonly dashboardService = inject(DashboardService);

  balanceHidden = signal(false);
  searchQuery = signal('');

  toggleBalance(): void {
    this.balanceHidden.update((v) => !v);
    this.dashboardService.toggleBalanceVisibility(this.balanceHidden());
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }
}
