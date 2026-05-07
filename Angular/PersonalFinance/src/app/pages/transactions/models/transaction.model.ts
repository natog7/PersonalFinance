export interface Recurrence {
  endDate: string | null;
  period: 'Daily' | 'Weekly' | 'Monthly' | 'Yearly';
}

export interface Transaction {
  id: string;
  title: string;
  amount: number;
  type: 'Income' | 'Expense';
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  date: string;
  status: 'Confirmado' | 'Débito' | 'Agendado';
  isRecurrent?: boolean;
  recurrent?: Recurrence | null;
}
