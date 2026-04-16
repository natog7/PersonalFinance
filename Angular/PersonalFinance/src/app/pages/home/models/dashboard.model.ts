export interface BalanceSummary {
  totalBalance: number;
  monthlyBalance: number;
  monthlyGrowthPercent: number;
  monthlyIncome: number;
  monthlyIncomeGrowthPercent: number;
  monthlyExpenses: number;
  monthlyExpensesGrowthPercent: number;
}

export interface ProjectionSummary {
  projectedBalance: number;
  projectedGrowthPercent: number;
  projectedIncome: number;
  projectedIncomeGrowthPercent: number;
  projectedExpenses: number;
  projectedExpensesGrowthPercent: number;
  description: string;
}

export interface CashFlow {
  totalIncome: number;
  totalExpenses: number;
  netProfit: number;
}

export interface CategoryExpense {
  label: string;
  percent: number;
  value: number;
  colorVar: string;
}

export interface SpendingByCategory {
  totalSpent: number;
  month: string;
  categories: CategoryExpense[];
}
