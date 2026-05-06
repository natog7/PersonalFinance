import { Component, input, ChangeDetectionStrategy, computed, signal, HostListener, ElementRef, inject, forwardRef } from '@angular/core';
import { ReactiveFormsModule, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgClass } from '@angular/common';

export interface SelectOption {
  value: any;
  label: string;
}

@Component({
  selector: 'app-select-field',
  standalone: true,
  imports: [NgClass, ReactiveFormsModule],
  templateUrl: './select-field.html',
  styleUrl: './select-field.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectField),
      multi: true
    }
  ]
})
export class SelectField implements ControlValueAccessor {
  fieldId = input.required<string>();
  label = input<string>();
  placeholder = input<string>();
  icon = input<string>();
  error = input<string>();
  options = input.required<SelectOption[]>();

  isOpen = signal(false);
  searchTerm = signal('');
  selectedValue = signal<any>(null);
  touched = signal(false);
  isDisabled = signal(false);

  private el = inject(ElementRef);

  filteredOptions = computed(() => {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.options();
    return this.options().filter(opt => opt.label.toLowerCase().includes(term));
  });

  selectedLabel = computed(() => {
    const val = this.selectedValue();
    const opt = this.options().find(o => o.value === val);
    return opt ? opt.label : '';
  });

  inputValue = computed(() => {
    if (this.isOpen()) {
      return this.searchTerm();
    }
    return this.selectedLabel();
  });

  onChange: any = () => {};
  onTouched: any = () => {};

  writeValue(val: any): void {
    this.selectedValue.set(val);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }

  toggleOpen() {
    if (this.isDisabled()) return;
    this.isOpen.update(v => !v);
    if (!this.isOpen()) {
      this.searchTerm.set('');
      this.markAsTouched();
    }
  }

  onFocus() {
    if (this.isDisabled()) return;
    this.isOpen.set(true);
  }

  onInputClick(event: Event) {
    if (this.isDisabled()) return;
    event.stopPropagation();
    if (!this.isOpen()) {
      this.isOpen.set(true);
    }
  }

  onSearch(event: Event) {
    if (this.isDisabled()) return;
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
    if (!this.isOpen()) {
      this.isOpen.set(true);
    }
  }

  selectOption(option: SelectOption) {
    if (this.isDisabled()) return;
    this.selectedValue.set(option.value);
    this.onChange(option.value);
    this.isOpen.set(false);
    this.searchTerm.set('');
    this.markAsTouched();
  }

  markAsTouched() {
    if (!this.touched()) {
      this.onTouched();
      this.touched.set(true);
    }
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (!this.el.nativeElement.contains(event.target)) {
      if (this.isOpen()) {
        this.isOpen.set(false);
        this.searchTerm.set('');
        this.markAsTouched();
      }
    }
  }
}
