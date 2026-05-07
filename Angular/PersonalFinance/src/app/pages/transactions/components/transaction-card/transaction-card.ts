import { Component, input, output } from '@angular/core';
import { Transaction } from '../../models/transaction.model';
import { NgClass, CurrencyPipe, DatePipe } from '@angular/common';
import { AuthService } from '../../../../shared/services/auth.service';
import { inject } from '@angular/core';

@Component({
  selector: 'app-transaction-card',
  standalone: true,
  imports: [NgClass, CurrencyPipe, DatePipe],
  templateUrl: './transaction-card.html',
  styleUrl: './transaction-card.scss'
})
export class TransactionCard {
  transaction = input.required<Transaction>();
  isExpanded = input<boolean>(false);
  readonly authService = inject(AuthService);

  edit = output<Transaction>();
  delete = output<string>();
  toggleExpand = output<string>();

  onEdit(event: Event): void {
    event.stopPropagation();
    this.edit.emit(this.transaction());
  }

  onDelete(event: Event): void {
    event.stopPropagation();
    this.delete.emit(this.transaction().id);
  }

  onToggle(event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.toggleExpand.emit(this.transaction().id);
  }
}
