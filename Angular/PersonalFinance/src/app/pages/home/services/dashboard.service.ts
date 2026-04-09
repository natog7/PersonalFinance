import { Injectable, signal } from '@angular/core';
import {
  BalanceSummary,
  CashFlow,
  ProjectionSummary,
  SpendingByCategory,
} from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly _balance = signal<BalanceSummary>({
    totalBalance: 142580.24,
    monthlyGrowthPercent: 12.4,
    monthlyIncome: 18200.0,
    totalExpenses: 4120.3,
  });

  private readonly _projection = signal<ProjectionSummary>({
    projectedBalance: 156660.14,
    emergencyGoalPercent: 85,
    description:
      'Simulação baseada em algoritmos preditivos considerando investimentos atuais e padrões de gastos recorrentes.',
  });

  private readonly _cashFlow = signal<CashFlow>({
    totalIncome: 24500.0,
    totalExpenses: 11200.0,
    netProfit: 13300.0,
  });

  private readonly _spending = signal<SpendingByCategory>({
    totalSpent: 4120,
    month: 'Agosto 2024',
    categories: [
      { label: 'Moradia', percent: 42, colorVar: 'var(--color-primary)' },
      { label: 'Lazer',   percent: 28, colorVar: 'var(--color-secondary)' },
      { label: 'Saúde',   percent: 15, colorVar: 'var(--color-tertiary)' },
      { label: 'Outros',  percent: 15, colorVar: 'var(--color-outline-variant)' },
    ],
  });

  // Public read-only signals
  readonly balance    = this._balance.asReadonly();
  readonly projection = this._projection.asReadonly();
  readonly cashFlow   = this._cashFlow.asReadonly();
  readonly spending   = this._spending.asReadonly();

  toggleBalanceVisibility(hidden: boolean): void {
    // Hook for future encryption/privacy feature
    console.log('Balance hidden:', hidden);
  }
}
