import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NonNullableFormBuilder, Validators } from '@angular/forms';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { SidebarStateService } from '../../shared/services/sidebar-state.service';
import { TransactionList } from './components/transaction-list/transaction-list';
import { TransactionForm } from './components/transaction-form/transaction-form';
import { TransactionService } from './services/transaction.service';
import { Transaction, RecurrencePeriod } from './models/transaction.model';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, SidebarComponent, HeaderComponent, TransactionList, TransactionForm],
  templateUrl: './transactions.html',
  styleUrl: './transactions.scss'
})
export class TransactionsComponent implements OnInit {
  sidebarState = inject(SidebarStateService);
  private transactionService = inject(TransactionService);
  private fb = inject(NonNullableFormBuilder);

  ngOnInit(): void {
    this.transactionService.fetchFiltered('2024-01-01');
  }

  transactions = this.transactionService.transactions;

  // Search state
  searchQuery = signal<string>('');
  filteredTransactions = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const list = this.transactions();
    if (!query) return list;
    return list.filter(t =>
      t.title.toLowerCase().includes(query) ||
      t.categoryName.toLowerCase().includes(query)
    );
  });

  // Form state
  isFormVisible = signal<boolean>(false);
  editingId = signal<string | null>(null);

  transactionForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    type: ['Expense', Validators.required],
    categoryId: ['', Validators.required],
    date: [new Date().toISOString().split('T')[0], Validators.required],
    status: ['Confirmado', Validators.required],
    isRecurrent: [false],
    recurrent: this.fb.group({
      endDate: [''],
      period: [RecurrencePeriod.Monthly]
    })
  });

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }

  onNew(): void {
    this.resetForm();
    this.isFormVisible.set(true);
  }

  onEdit(transaction: Transaction): void {
    this.editingId.set(transaction.id);
    this.transactionForm.patchValue({
      title: transaction.title,
      amount: transaction.amount,
      type: transaction.type,
      categoryId: transaction.categoryId,
      date: transaction.date,
      status: transaction.status,
      isRecurrent: !!transaction.recurrent,
      recurrent: {
        endDate: transaction.recurrent?.endDate || '',
        period: transaction.recurrent?.period || RecurrencePeriod.Monthly
      }
    });
    this.isFormVisible.set(true);
  }

  onDelete(id: string): void {
    if (confirm('Tem certeza que deseja excluir este lançamento?')) {
      this.transactionService.delete(id);
    }
  }

  onSave(): void {
    if (this.transactionForm.invalid) return;

    const formValue = this.transactionForm.getRawValue();
    const id = this.editingId();

    const partialTx = {
      title: formValue.title,
      amount: formValue.amount,
      type: formValue.type as any,
      categoryId: formValue.categoryId,
      categoryName: formValue.categoryId === 'cat1' ? 'Investimentos' : formValue.categoryId === 'cat2' ? 'Alimentação' : 'Moradia',
      categoryIcon: formValue.categoryId === 'cat1' ? 'payments' : formValue.categoryId === 'cat2' ? 'shopping_cart' : 'home',
      categoryColor: formValue.categoryId === 'cat1' ? 'bg-secondary-container text-on-secondary-container' : formValue.categoryId === 'cat2' ? 'bg-tertiary-fixed text-tertiary' : 'bg-surface-container-high text-on-surface-variant',
      date: formValue.date,
      status: formValue.status as any,
      recurrent: formValue.isRecurrent ? {
        endDate: formValue.recurrent.endDate || null,
        period: formValue.recurrent.period as RecurrencePeriod
      } : null
    };

    if (id) {
      this.transactionService.update({ ...partialTx, id });
    } else {
      this.transactionService.create(partialTx);
    }

    this.resetForm();
  }

  resetForm(): void {
    this.transactionForm.reset({
      title: '',
      amount: 0,
      type: 'Expense',
      categoryId: '',
      date: new Date().toISOString().split('T')[0],
      status: 'Confirmado',
      isRecurrent: false,
      recurrent: {
        endDate: '',
        period: RecurrencePeriod.Monthly
      }
    });
    this.editingId.set(null);
    this.isFormVisible.set(false);
  }
}
