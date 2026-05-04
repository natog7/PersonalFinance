export interface Transaction {
  id: string;
  title: string;
  amount: number;
  currency: string;
  type: 'Income' | 'Expense';
  categoryId: string;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  date: string;
  status: 'Confirmado' | 'Débito' | 'Agendado';
}
