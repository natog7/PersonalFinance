import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
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
  @Input({ required: true }) categoryForm!: FormGroup;
  @Input({ required: true }) isOpen = false;
  @Input() editingId: string | null = null;
  @Input() iconOptions: string[] = [];

  @Output() save = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  @Output() selectIcon = new EventEmitter<string>();

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