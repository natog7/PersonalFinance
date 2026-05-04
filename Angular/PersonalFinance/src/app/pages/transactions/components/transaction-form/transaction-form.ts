import { Component, input, output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './transaction-form.html',
  styleUrl: './transaction-form.scss'
})
export class TransactionForm {
  transactionForm = input.required<FormGroup>();
  isOpen = input<boolean>(false);
  editingId = input<string | null>(null);

  save = output<void>();
  cancel = output<void>();

  onSave(): void {
    if (this.transactionForm().valid) {
      this.save.emit();
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
