import { Component, input, output } from '@angular/core';
import { Transaction } from '../../models/transaction.model';
import { NgClass, CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-transaction-card',
  standalone: true,
  imports: [NgClass, CurrencyPipe, DatePipe],
  templateUrl: './transaction-card.html',
  styleUrl: './transaction-card.scss'
})
export class TransactionCard {
  transaction = input.required<Transaction>();
  edit = output<Transaction>();
  delete = output<string>();

  onEdit(): void {
    this.edit.emit(this.transaction());
  }

  onDelete(event: Event): void {
    event.stopPropagation();
    this.delete.emit(this.transaction().id);
  }
}
