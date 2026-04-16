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
    monthlyBalance: 18200.0,
    monthlyGrowthPercent: 12.4,
    monthlyIncome: 12000.0,
    monthlyIncomeGrowthPercent: 0.0,
    monthlyExpenses: 4120.3,
    monthlyExpensesGrowthPercent: -10.0,
  });

  private readonly _projection = signal<ProjectionSummary>({
    projectedBalance: 156660.14,
    projectedGrowthPercent: 12.4,
    projectedIncome: 12000.0,
    projectedIncomeGrowthPercent: 0.0,
    projectedExpenses: 3420.54,
    projectedExpensesGrowthPercent: -10.0,
    description:
      'Simulação preditiva considerando rendimentos e padrões de gastos recorrentes.',
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
      { label: 'Lazer', value: 1154, percent: 28, colorVar: 'var(--color-dashboard-1)' },
      { label: 'Alimentação', value: 742, percent: 18, colorVar: 'var(--color-dashboard-2)' },
      { label: 'Transporte', value: 577, percent: 14, colorVar: 'var(--color-dashboard-3)' },
      { label: 'Contas', value: 494, percent: 12, colorVar: 'var(--color-dashboard-4)' },
      { label: 'Higiene', value: 412, percent: 10, colorVar: 'var(--color-dashboard-5)' },
      { label: 'Saúde', value: 329, percent: 8, colorVar: 'var(--color-dashboard-6)' },
      { label: 'Investimentos', value: 124, percent: 3, colorVar: 'var(--color-dashboard-7)' },
      { label: 'Outros', value: 288, percent: 7, colorVar: 'var(--color-dashboard-10)' },
    ],
  });

  // Public read-only signals
  readonly balance = this._balance.asReadonly();
  readonly projection = this._projection.asReadonly();
  readonly cashFlow = this._cashFlow.asReadonly();
  readonly spending = this._spending.asReadonly();
  readonly isBalanceHidden = signal<boolean>(false);

  toggleBalanceVisibility(hidden: boolean): void {
    this.isBalanceHidden.set(hidden);
  }
}
