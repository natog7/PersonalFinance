import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { NgClass } from '@angular/common';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-category-card',
  standalone: true,
  imports: [NgClass],
  templateUrl: './category-card.html',
  styleUrl: './category-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryCard {
  category = input.required<Category>();

  edit = output<Category>();
  delete = output<string>();

  onEdit(): void {
    this.edit.emit(this.category());
  }

  onDelete(): void {
    this.delete.emit(this.category().id);
  }
}