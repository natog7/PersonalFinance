import { Component, input, output, signal } from '@angular/core';
import { Transaction } from '../../models/transaction.model';
import { TransactionCard } from '../transaction-card/transaction-card';

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [TransactionCard],
  templateUrl: './transaction-list.html',
  styleUrl: './transaction-list.scss'
})
export class TransactionList {
  transactions = input.required<Transaction[]>();
  edit = output<Transaction>();
  delete = output<string>();

  expandedCardId = signal<string | null>(null);

  onEdit(transaction: Transaction): void {
    this.edit.emit(transaction);
  }

  onDelete(id: string): void {
    this.delete.emit(id);
  }

  onToggleExpand(id: string): void {
    if (this.expandedCardId() === id) {
      this.expandedCardId.set(null);
    } else {
      this.expandedCardId.set(id);
    }
  }
}
