import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { TransactionService } from '../../services/transaction.service';
import { Transaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transactions-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './transactions-card.component.html',
  styleUrl: './transactions-card.component.scss',
})
export class TransactionsCardComponent {
  private readonly transactionService = inject(TransactionService);
  readonly transactions = this.transactionService.transactions;

  trackById(_: number, item: Transaction): string {
    return item.id;
  }

  isIncome(tx: Transaction): boolean {
    return tx.type === 'income';
  }
}
