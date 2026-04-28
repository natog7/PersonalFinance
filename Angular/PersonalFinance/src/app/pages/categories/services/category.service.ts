import { Injectable, signal, inject, computed } from '@angular/core';
import { Category } from '../models/category.model';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:55784/api/categories';

  // Using signals for reactive state, simulating a database/API.
  // private readonly _categories = signal<Category[]>([
  //   {
  //     id: '1',
  //     name: 'Moradia',
  //     description: 'Aluguel, condomínio, luz e internet.',
  //     icon: 'home',
  //     color: '#1d4ed8', // blue-700
  //     iconBg: '#dbeafe', // blue-100
  //     isActive: true,
  //   },
  //   {
  //     id: '2',
  //     name: 'Alimentação',
  //     description: 'Supermercado, restaurantes e delivery.',
  //     icon: 'restaurant',
  //     color: '#15803d', // green-700
  //     iconBg: '#dcfce3', // green-100
  //     isActive: true,
  //   },
  //   {
  //     id: '3',
  //     name: 'Transporte',
  //     description: 'Combustível, IPVA e transporte público.',
  //     icon: 'directions_car',
  //     color: '#c2410c', // orange-700
  //     iconBg: '#ffedd5', // orange-100
  //     isActive: true,
  //   },
  //   {
  //     id: '4',
  //     name: 'Lazer',
  //     description: 'Viagens, cinema e hobbies.',
  //     icon: 'confirmation_number',
  //     color: '#7e22ce', // purple-700
  //     iconBg: '#f3e8ff', // purple-100
  //     isActive: false,
  //   },
  // ]);

  // readonly categories = this._categories.asReadonly();
  readonly categories = signal<Category[]>([]);
  public totalCategories = computed(() => this.categories().length);

  fetch() {
    const filterPayload = {
      name: null,
      description: null,
      parentCategoryId: null,
      isActive: null
    };

    this.http.post<{ items: Category[] }>(`${this.apiUrl}/filter`, filterPayload).subscribe({
      next: (response) => {
        const mappedItems = response.items.map(item => ({
          ...item,
          description: item.description ?? '',
          icon: item.icon ?? 'category',
          iconBg: item.color ? `${item.color}33` : '#e2e8f0'
        }));
        this.categories.set(mappedItems);
      },
      error: (err) => console.error('Erro ao buscar.', err)
    });
  }

  getAll(): Category[] {
    return this.categories();
  }

  create(category: Omit<Category, 'id'>): void {
    const tempId = Math.random().toString(36).substring(2, 9);
    const optimisticCategory: Category = { ...category, id: tempId };
    
    this.categories.update((list) => [...list, optimisticCategory]);

    this.http.post<Category>(this.apiUrl, category).subscribe({
      next: (newCategory) => {
        const mappedCategory = {
          ...optimisticCategory,
          ...newCategory,
          description: newCategory.description ?? optimisticCategory.description,
          icon: newCategory.icon ?? optimisticCategory.icon,
          iconBg: newCategory.color ? `${newCategory.color}33` : optimisticCategory.iconBg
        };
        this.categories.update((list) => list.map((c) => (c.id === tempId ? mappedCategory : c)));
      },
      error: (err) => {
        this.categories.update((list) => list.filter((c) => c.id !== tempId));
        console.error('Erro ao criar.', err);
      }
    });
  }

  update(updatedCategory: Category): void {
    // this._categories.update((list) =>
    //   list.map((c) => (c.id === updatedCategory.id ? updatedCategory : c))
    // );

    this.http.put<Category>(`${this.apiUrl}/${updatedCategory.id}`, updatedCategory).subscribe({
      next: (resp) => {
        this.categories.update((list) =>
          list.map((c) => (c.id === resp.id ? resp : c))
        );
      },
      error: (err) => console.error('Erro ao atualizar.', err)
    });
  }

  delete(id: string): void {
    const current = this.categories();
    this.categories.set(current.filter((c) => c.id !== id));

    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      error: () => {
        this.categories.set(current);
        alert('Erro ao apagar.');
      }
    });
  }
}
