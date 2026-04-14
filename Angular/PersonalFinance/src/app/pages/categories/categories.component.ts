import { Component, inject, computed, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgClass } from '@angular/common';
import { CategoryService } from './services/category.service';
import { Category } from './models/category.model';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { SidebarStateService } from '../../shared/services/sidebar-state.service';

export interface VisualOption {
  color: string;
  bg: string;
  hex: string;
  icon: string;
}

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [SidebarComponent, HeaderComponent, ReactiveFormsModule, NgClass],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent {
  private readonly categoryService = inject(CategoryService);
  private readonly fb = inject(FormBuilder);
  readonly sidebarState = inject(SidebarStateService);

  readonly allCategories = this.categoryService.categories;
  
  searchQuery = signal('');
  isFormVisible = signal(false);

  filteredCategories = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    if (!query) {
      return this.allCategories();
    }
    return this.allCategories().filter((cat) =>
      cat.name.toLowerCase().includes(query) || cat.description.toLowerCase().includes(query)
    );
  });

  categoryForm: FormGroup;
  editingId: string | null = null;

  // Preset icons for the category form
  readonly iconOptions: string[] = [
    'home', 'restaurant', 'directions_car', 'health_and_safety', 
    'confirmation_number', 'shopping_bag', 'school', 'pets', 
    'favorite', 'flight', 'savings', 'work', 'electric_bolt',
    'water_drop', 'router', 'fitness_center', 'movie', 'redeem'
  ];

  constructor() {
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      icon: ['home', Validators.required],
      iconColor: ['#2563eb', Validators.required],
      isActive: [true],
    });
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }

  onNew(): void {
    this.resetForm();
    this.isFormVisible.set(true);
  }

  onEdit(category: Category): void {
    this.editingId = category.id;
    this.categoryForm.patchValue({
      name: category.name,
      description: category.description,
      icon: category.icon,
      iconColor: category.iconColor,
      isActive: category.isActive,
    });
    this.isFormVisible.set(true);
  }

  onDelete(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta categoria?')) {
      this.categoryService.delete(id);
      if (this.editingId === id) {
        this.resetForm();
      }
    }
  }

  onSave(): void {
    if (this.categoryForm.invalid) return;

    const formValue = this.categoryForm.value;

    // Helper to generate a light background version of the color
    const hexToRgb = (hex: string) => {
      const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
      return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
      } : null;
    };
    
    const rgb = hexToRgb(formValue.iconColor);
    const iconBg = rgb ? `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, 0.15)` : 'rgba(0,0,0,0.05)';

    const categoryData: Omit<Category, 'id'> = {
      name: formValue.name,
      description: formValue.description,
      isActive: formValue.isActive,
      icon: formValue.icon,
      iconColor: formValue.iconColor,
      iconBg: iconBg,
    };

    if (this.editingId) {
      this.categoryService.update({ ...categoryData, id: this.editingId });
    } else {
      this.categoryService.create(categoryData);
    }
    
    this.resetForm();
  }

  resetForm(): void {
    this.editingId = null;
    this.categoryForm.reset({
      name: '',
      description: '',
      icon: 'home',
      iconColor: '#2563eb',
      isActive: true,
    });
    this.isFormVisible.set(false);
  }
}
