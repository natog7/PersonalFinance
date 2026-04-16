import { Injectable, signal, inject, computed } from '@angular/core';
import { Category } from '../models/category.model';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://sua-api.com/api/categories';

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
  public totalCategories = computed(() => this._categories().length);

  fetch() {
    this.http.get<Category[]>(this.apiUrl).subscribe({
      next: (data) => this._categories.set(data),
      error: (err) => console.error('Erro ao buscar.', err)
    });
  }

  getAll(): Category[] {
    return this._categories();
  }

  create(category: Omit<Category, 'id'>): void {
    // const newCategory: Category = {
    //   ...category,
    //   id: Math.random().toString(36).substring(2, 9),
    // };
    // this._categories.update((list) => [...list, newCategory]);

    this.http.post<Category>(this.apiUrl, category).subscribe({
      next: (newCategory) => {
        this._categories.update((list) => [...list, newCategory]);
      },
      error: (err) => console.error('Erro ao criar.', err)
    });
  }

  update(updatedCategory: Category): void {
    // this._categories.update((list) =>
    //   list.map((c) => (c.id === updatedCategory.id ? updatedCategory : c))
    // );

    this.http.put<Category>(`${this.apiUrl}/${updatedCategory.id}`, updatedCategory).subscribe({
      next: (resp) => {
        this._categories.update((list) =>
          list.map((c) => (c.id === resp.id ? resp : c))
        );
      },
      error: (err) => console.error('Erro ao atualizar.', err)
    });
  }

  delete(id: string): void {
    const current = this._categories();
    this._categories.set(current.filter((c) => c.id !== id));

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      error: () => {
        this._categories.set(current);
        alert('Erro ao apagar.');
      }
    });
  }
}
