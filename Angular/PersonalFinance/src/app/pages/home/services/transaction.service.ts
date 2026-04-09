import { Injectable, signal } from '@angular/core';
import { Transaction } from '../models/transaction.model';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  // Using signals for reactive state (Angular modern pattern)
  private readonly _transactions = signal<Transaction[]>([
    {
      id: '1',
      title: 'Apple Store Brasil',
      category: 'Tecnologia',
      amount: 12499.0,
      type: 'expense',
      time: '14:32',
      icon: 'shopping_bag',
      iconBg: '#eff6ff',
      iconColor: '#2563eb',
      account: 'Cartão Final 8821',
    },
    {
      id: '2',
      title: 'Dividendos Petrobras',
      category: 'Investimentos',
      amount: 2450.12,
      type: 'income',
      time: '11:15',
      icon: 'payments',
      iconBg: '#f0fdf4',
      iconColor: '#16a34a',
      account: 'Conta Corrente',
    },
    {
      id: '3',
      title: 'Restaurante Fasano',
      category: 'Alimentação',
      amount: 850.0,
      type: 'expense',
      time: 'Ontem',
      icon: 'restaurant',
      iconBg: '#faf5ff',
      iconColor: '#9333ea',
      account: 'Cartão Final 8821',
    },
    {
      id: '4',
      title: 'Salário Mensal',
      category: 'Renda',
      amount: 18200.0,
      type: 'income',
      time: 'Ontem',
      icon: 'account_balance',
      iconBg: '#f0fdf4',
      iconColor: '#16a34a',
      account: 'Conta Corrente',
    },
    {
      id: '5',
      title: 'Netflix',
      category: 'Lazer',
      amount: 55.9,
      type: 'expense',
      time: '2 dias atrás',
      icon: 'live_tv',
      iconBg: '#fff1f2',
      iconColor: '#e11d48',
      account: 'Cartão Final 8821',
    },
  ]);

  readonly transactions = this._transactions.asReadonly();

  getAll(): Transaction[] {
    return this._transactions();
  }

  getRecent(limit = 3): Transaction[] {
    return this._transactions().slice(0, limit);
  }

  add(transaction: Transaction): void {
    this._transactions.update((list) => [transaction, ...list]);
  }

  remove(id: string): void {
    this._transactions.update((list) => list.filter((t) => t.id !== id));
  }
}
