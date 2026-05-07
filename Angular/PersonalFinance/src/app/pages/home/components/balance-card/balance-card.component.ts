import { Component, inject, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../../../shared/services/auth.service';

@Component({
  selector: 'app-balance-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './balance-card.component.html',
  styleUrl: './balance-card.component.scss',
})
export class BalanceCardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  readonly balance = this.dashboardService.balance;
  readonly isBalanceHidden = this.dashboardService.isBalanceHidden;
  readonly authService = inject(AuthService);

  ngOnInit(): void {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    const start = `${firstDay.getFullYear()}-${String(firstDay.getMonth() + 1).padStart(2, '0')}-01`;
    const end = `${lastDay.getFullYear()}-${String(lastDay.getMonth() + 1).padStart(2, '0')}-${String(lastDay.getDate()).padStart(2, '0')}`;

    this.dashboardService.fetchBalanceByMonth(start, end);
  }
}
