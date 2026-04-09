export type TransactionType = 'income' | 'expense';

export interface Transaction {
  id: string;
  title: string;
  category: string;
  amount: number;
  type: TransactionType;
  time: string;
  icon: string;
  iconBg: string;
  iconColor: string;
  account: string;
}
