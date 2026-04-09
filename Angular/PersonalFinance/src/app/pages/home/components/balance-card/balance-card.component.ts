import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'app-balance-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './balance-card.component.html',
  styleUrl: './balance-card.component.scss',
})
export class BalanceCardComponent {
  private readonly dashboardService = inject(DashboardService);
  readonly balance = this.dashboardService.balance;
}
