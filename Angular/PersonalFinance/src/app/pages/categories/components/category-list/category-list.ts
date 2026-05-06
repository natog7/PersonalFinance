import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { Category } from '../../models/category.model';
import { CategoryCard } from '../category-card/category-card';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CategoryCard],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryList {
  categories = input.required<Category[]>();

  edit = output<Category>();
  delete = output<string>();

  onEdit(category: Category): void {
    this.edit.emit(category);
  }

  onDelete(id: string): void {
    this.delete.emit(id);
  }
}