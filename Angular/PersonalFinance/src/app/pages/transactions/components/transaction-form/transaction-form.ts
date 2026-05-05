import { Component, input, output, inject, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CategoryService } from '../../../categories/services/category.service';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './transaction-form.html',
  styleUrl: './transaction-form.scss'
})
export class TransactionForm implements OnInit {
  transactionForm = input.required<FormGroup>();
  isOpen = input<boolean>(false);
  editingId = input<string | null>(null);

  save = output<void>();
  cancel = output<void>();

  private categoryService = inject(CategoryService);
  categories = this.categoryService.categories;

  ngOnInit(): void {
    if (this.categories().length === 0) {
      this.categoryService.fetch();
    }
  }

  onSave(): void {
    if (this.transactionForm().valid) {
      this.save.emit();
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
