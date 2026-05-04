import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BalanceSummary,
  CashFlow,
  ProjectionSummary,
  SpendingByCategory,
} from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  private readonly _balance = signal<BalanceSummary>({
    totalBalance: 142580.24,
    monthlyBalance: 18200.0,
    monthlyGrowthPercent: 12.4,
    monthlyIncome: 12000.0,
    monthlyIncomeGrowthPercent: 10.0,
    monthlyExpenses: 4120.3,
    monthlyExpensesGrowthPercent: -8.12,
  });

  private readonly _projection = signal<ProjectionSummary>({
    projectedBalance: 40685.19,
    projectedGrowthPercent: 123.45,
    projectedIncome: 10890.0,
    projectedIncomeGrowthPercent: -9.25,
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
    totalSpent: 0,
    month: '',
    categories: [],
    // categories: [
    //   { label: 'Lazer', value: 1154, percent: 28, colorVar: 'var(--color-dashboard-1)' },
    //   { label: 'Alimentação', value: 742, percent: 18, colorVar: 'var(--color-dashboard-2)' },
    //   { label: 'Transporte', value: 577, percent: 14, colorVar: 'var(--color-dashboard-3)' },
    //   { label: 'Contas', value: 494, percent: 12, colorVar: 'var(--color-dashboard-4)' },
    //   { label: 'Higiene', value: 412, percent: 10, colorVar: 'var(--color-dashboard-5)' },
    //   { label: 'Saúde', value: 329, percent: 8, colorVar: 'var(--color-dashboard-6)' },
    //   { label: 'Investimentos', value: 124, percent: 3, colorVar: 'var(--color-dashboard-7)' },
    //   { label: 'Outros', value: 288, percent: 7, colorVar: 'var(--color-dashboard-10)' },
    // ],
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

  fetchBalanceByMonth(startDate: string, endDate: string | null = null, categoryIds: string[] = [], isProjection: boolean = false): void {
    const payload = {
      date: {
        start: startDate,
        end: endDate
      },
      categoryIds
    };

    this.http.post<{ items: any[] }>('https://localhost:55784/api/finance/balance-by-month', payload)
      .subscribe({
        next: (response) => {
          if (response.items && response.items.length > 0) {
            const data = response.items[0];
            if (isProjection) {
              this._projection.update(current => ({
                ...current,
                projectedBalance: data.total,
                projectedGrowthPercent: data.totalGrowthPercentage,
                projectedIncome: data.totalIncome,
                projectedIncomeGrowthPercent: data.totalIncomeGrowthPercentage,
                projectedExpenses: data.totalExpense,
                projectedExpensesGrowthPercent: data.totalExpenseGrowthPercentage
              }));
            } else {
              this._balance.update(current => ({
                ...current,
                monthlyBalance: data.total,
                monthlyGrowthPercent: data.totalGrowthPercentage,
                monthlyIncome: data.totalIncome,
                monthlyIncomeGrowthPercent: data.totalIncomeGrowthPercentage,
                monthlyExpenses: data.totalExpense,
                monthlyExpensesGrowthPercent: data.totalExpenseGrowthPercentage
              }));
            }
          }
        },
        error: (err) => console.error('Erro ao buscar balance-by-month', err)
      });
  }

  fetchSpendingByCategory(startDate: string, endDate: string | null = null, type: string = 'Expense', categoryIds: string[] = []): void {
    const payload = {
      date: {
        start: startDate,
        end: endDate
      },
      type,
      categoryIds
    };

    this.http.post<{ items: any[] }>('https://localhost:55784/api/finance/transactions-by-category', payload)
      .subscribe({
        next: (response) => {
          if (response.items) {
            const items = response.items;
            const totalSpent = items.reduce((sum, item) => sum + item.totalAmount, 0);

            // Format month string, e.g. "Maio 2026"
            const startD = new Date(startDate);
            const monthNames = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
            const monthName = `${monthNames[startD.getMonth()]} ${startD.getFullYear()}`;

            const categories = items.map((item, index) => {
              const percent = totalSpent > 0 ? (item.totalAmount / totalSpent) * 100 : 0;
              return {
                label: item.categoryName,
                value: item.totalAmount,
                percent: parseFloat(percent.toFixed(1)),
                colorVar: `var(--color-dashboard-${(index % 10) + 1})`
              };
            });

            this._spending.set({
              totalSpent,
              month: monthName,
              categories
            });
          }
        },
        error: (err) => console.error('Erro ao buscar transactions-by-category', err)
      });
  }
}
