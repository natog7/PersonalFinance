import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { TransactionService } from '../../services/transaction.service';
import { Transaction } from '../../models/transaction.model';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'app-transactions-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './transactions-card.component.html',
  styleUrl: './transactions-card.component.scss',
})
export class TransactionsCardComponent {
  private readonly transactionService = inject(TransactionService);
  private readonly dashboardService = inject(DashboardService);
  readonly transactions = this.transactionService.transactions;
  readonly isBalanceHidden = this.dashboardService.isBalanceHidden;

  trackById(_: number, item: Transaction): string {
    return item.id;
  }

  isIncome(tx: Transaction): boolean {
    return tx.type === 'income';
  }
}
