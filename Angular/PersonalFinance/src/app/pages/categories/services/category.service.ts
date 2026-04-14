import { Injectable, signal } from '@angular/core';
import { Category } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  // Using signals for reactive state, simulating a database/API.
  private readonly _categories = signal<Category[]>([
    {
      id: '1',
      name: 'Moradia',
      description: 'Aluguel, condomínio, luz e internet.',
      icon: 'home',
      iconColor: '#1d4ed8', // blue-700
      iconBg: '#dbeafe', // blue-100
      isActive: true,
    },
    {
      id: '2',
      name: 'Alimentação',
      description: 'Supermercado, restaurantes e delivery.',
      icon: 'restaurant',
      iconColor: '#15803d', // green-700
      iconBg: '#dcfce3', // green-100
      isActive: true,
    },
    {
      id: '3',
      name: 'Transporte',
      description: 'Combustível, IPVA e transporte público.',
      icon: 'directions_car',
      iconColor: '#c2410c', // orange-700
      iconBg: '#ffedd5', // orange-100
      isActive: true,
    },
    {
      id: '4',
      name: 'Lazer',
      description: 'Viagens, cinema e hobbies.',
      icon: 'confirmation_number',
      iconColor: '#7e22ce', // purple-700
      iconBg: '#f3e8ff', // purple-100
      isActive: false,
    },
  ]);

  readonly categories = this._categories.asReadonly();

  getAll(): Category[] {
    return this._categories();
  }

  // API placeholders (Ready for HttpClient integration later)
  create(category: Omit<Category, 'id'>): void {
    const newCategory: Category = {
      ...category,
      id: Math.random().toString(36).substring(2, 9),
    };
    this._categories.update((list) => [...list, newCategory]);
  }

  update(updatedCategory: Category): void {
    this._categories.update((list) =>
      list.map((c) => (c.id === updatedCategory.id ? updatedCategory : c))
    );
  }

  delete(id: string): void {
    this._categories.update((list) => list.filter((c) => c.id !== id));
  }
}
