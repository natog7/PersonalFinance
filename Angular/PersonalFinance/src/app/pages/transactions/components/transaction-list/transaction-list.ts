import { Component, input, output } from '@angular/core';
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

  onEdit(transaction: Transaction): void {
    this.edit.emit(transaction);
  }

  onDelete(id: string): void {
    this.delete.emit(id);
  }
}
