export enum RecurrencePeriod {
  Daily = 'Daily',
  Weekly = 'Weekly',
  Monthly = 'Monthly',
  Yearly = 'Yearly'
}

export interface Recurrence {
  endDate: string | null;
  period: RecurrencePeriod;
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
