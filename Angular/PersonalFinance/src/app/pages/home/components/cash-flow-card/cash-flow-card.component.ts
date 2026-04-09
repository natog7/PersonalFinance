import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'app-cash-flow-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './cash-flow-card.component.html',
  styleUrl: './cash-flow-card.component.scss',
})
export class CashFlowCardComponent {
  private readonly dashboardService = inject(DashboardService);
  readonly cashFlow = this.dashboardService.cashFlow;

  /** Percentage of expenses vs income for the progress bar */
  get expenseBarWidth(): number {
    const cf = this.cashFlow();
    if (cf.totalIncome === 0) return 0;
    return Math.min((cf.totalExpenses / cf.totalIncome) * 100, 100);
  }
}
