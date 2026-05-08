import { Component, input, output, inject, OnInit, computed } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CategoryService } from '../../../categories/services/category.service';
import { SelectField, SelectOption } from '../../../../shared/components/select-field/select-field';
import { CurrencyMaskDirective } from '../../../../shared/directives/currency-mask.directive';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [ReactiveFormsModule, SelectField, CurrencyMaskDirective],
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

  typeOptions: SelectOption[] = [
    { value: 'Income', label: 'Receita' },
    { value: 'Expense', label: 'Despesa' }
  ];

  categoryOptions = computed<SelectOption[]>(() => {
    return this.categories().map(cat => ({
      value: cat.id,
      label: cat.name
    }));
  });

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
