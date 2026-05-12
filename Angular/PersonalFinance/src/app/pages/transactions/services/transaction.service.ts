import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Transaction } from '../models/transaction.model';
import { ToastService } from '../../../shared/services/toast.service';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly http = inject(HttpClient);
  private toastService = inject(ToastService);
  private readonly apiUrl = 'https://localhost:55784/api/transactions';

  private readonly _transactions = signal<Transaction[]>([]);

  readonly transactions = this._transactions.asReadonly();
  readonly totalTransactions = computed(() => this.transactions().length);

  fetchFiltered(startDate: string, endDate: string | null = null, type: string | null = null, categoryIds: string[] = []) {
    const payload = {
      title: null,
      date: {
        start: startDate,
        end: endDate
      },
      type: type,
      currency: null,
      categoryIds: categoryIds.length > 0 ? categoryIds : [""]
    };

    this.http.post<{ items: any[] }>(`${this.apiUrl}/filter`, payload)
      .subscribe({
        next: (response) => {
          if (response.items) {
            const mappedItems: Transaction[] = response.items.map(item => ({
              id: item.id,
              title: item.title,
              amount: item.amount,
              type: item.type as 'Income' | 'Expense',
              categoryId: item.categoryId,
              categoryName: item.categoryName,
              categoryIcon: item.categoryId === 'cat1' ? 'payments' : item.categoryId === 'cat2' ? 'shopping_cart' : 'home',
              categoryColor: item.categoryId === 'cat1' ? 'bg-secondary-container text-on-secondary-container' : item.categoryId === 'cat2' ? 'bg-tertiary-fixed text-tertiary' : 'bg-surface-container-high text-on-surface-variant',
              date: item.date,
              status: 'Confirmado', // Default status for API items if not returned
              isRecurrent: item.isRecurrent,
              recurrent: item.recurrent
            }));
            this._transactions.set(mappedItems);
          }
        },
        error: (err: HttpErrorResponse) => this.toastService.httpError(err.status, 'Erro ao buscar transações.')
      });
  }


  create(transaction: Omit<Transaction, 'id'>) {
    const payload = {
      title: transaction.title,
      amount: transaction.amount,
      date: transaction.date,
      type: transaction.type,
      categoryId: transaction.categoryId,
      recurrent: transaction.recurrent || null
    };

    this.http.post<{ id: string }>(this.apiUrl, payload)
      .subscribe({
        next: (response) => {
          const newTx: Transaction = {
            ...transaction,
            id: response.id
          };
          this._transactions.update(list => [newTx, ...list]);
          this.toastService.success('Transação criada com sucesso!');
        },
        error: (err: HttpErrorResponse) => this.toastService.httpError(err.status, 'Erro ao criar transação.')
      });
  }

  update(transaction: Transaction) {
    const payload = {
      id: transaction.id,
      title: transaction.title,
      amount: transaction.amount,
      date: transaction.date,
      type: transaction.type,
      categoryId: transaction.categoryId,
      recurrent: transaction.recurrent || null
    };

    this.http.put<Transaction>(`${this.apiUrl}/${transaction.id}`, payload)
      .subscribe({
        next: (response) => {
          this._transactions.update(list => list.map(t => t.id === transaction.id ? { ...transaction, ...response } : t));
          this.toastService.success('Transação atualizada com sucesso!');
        },
        error: (err: HttpErrorResponse) => this.toastService.httpError(err.status, 'Erro ao atualizar transação.')
      });
  }

  delete(id: string) {
    this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`)
      .subscribe({
        next: () => {
          this._transactions.update(list => list.filter(t => t.id !== id));
          this.toastService.success('Transação excluída com sucesso!');
        },
        error: (err: HttpErrorResponse) => this.toastService.httpError(err.status, 'Erro ao excluir transação.')
      });
  }
}
