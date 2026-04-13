export interface BalanceSummary {
  totalBalance: number;
  monthlyGrowthPercent: number;
  monthlyIncome: number;
  totalExpenses: number;
}

export interface ProjectionSummary {
  projectedBalance: number;
  emergencyGoalPercent: number;
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
