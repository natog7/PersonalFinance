import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [ReactiveFormsModule, NgClass],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryForm {
  categoryForm = input.required<FormGroup>();
  isOpen = input.required<boolean>();
  editingId = input<string | null>(null);
  iconOptions = input<string[]>([]);

  save = output<void>();
  cancel = output<void>();
  selectIcon = output<string>();

  onSave(): void {
    this.save.emit();
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onIconClick(icon: string): void {
    this.selectIcon.emit(icon);
  }
}