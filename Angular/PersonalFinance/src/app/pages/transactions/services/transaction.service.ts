import { Injectable, signal, computed } from '@angular/core';
import { Transaction } from '../models/transaction.model';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly _transactions = signal<Transaction[]>([
    {
      id: '1',
      title: 'Dividendos Mensais',
      amount: 1250.40,
      currency: 'BRL',
      type: 'Income',
      categoryId: 'cat1',
      categoryName: 'Investimentos',
      categoryIcon: 'payments',
      categoryColor: 'bg-secondary-container text-on-secondary-container',
      date: '2024-06-14',
      status: 'Confirmado'
    },
    {
      id: '2',
      title: 'Supermercado Pão de Açúcar',
      amount: 412.89,
      currency: 'BRL',
      type: 'Expense',
      categoryId: 'cat2',
      categoryName: 'Alimentação',
      categoryIcon: 'shopping_cart',
      categoryColor: 'bg-tertiary-fixed text-tertiary',
      date: '2024-06-12',
      status: 'Débito'
    },
    {
      id: '3',
      title: 'Aluguel Apartamento',
      amount: 3500.00,
      currency: 'BRL',
      type: 'Expense',
      categoryId: 'cat3',
      categoryName: 'Moradia',
      categoryIcon: 'home',
      categoryColor: 'bg-surface-container-high text-on-surface-variant',
      date: '2024-06-10',
      status: 'Agendado'
    }
  ]);

  readonly transactions = this._transactions.asReadonly();
  readonly totalTransactions = computed(() => this.transactions().length);

  create(transaction: Omit<Transaction, 'id'>) {
    const newTx: Transaction = {
      ...transaction,
      id: Math.random().toString(36).substring(2, 9)
    };
    this._transactions.update(list => [newTx, ...list]);
  }

  update(transaction: Transaction) {
    this._transactions.update(list => list.map(t => t.id === transaction.id ? transaction : t));
  }

  delete(id: string) {
    this._transactions.update(list => list.filter(t => t.id !== id));
  }
}
