import { Directive, ElementRef, HostListener, inject, OnInit } from '@angular/core';
import { NgControl } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Directive({
  selector: '[appCurrencyMask]',
  standalone: true
})
export class CurrencyMaskDirective implements OnInit {
  private el = inject(ElementRef<HTMLInputElement>);
  private control = inject(NgControl);
  private authService = inject(AuthService);

  ngOnInit() {
    // Initial format
    setTimeout(() => {
      const value = this.control.value;
      if (value !== null && value !== undefined) {
        this.formatValue(value);
      }
    });
  }

  @HostListener('input', ['$event'])
  onInput(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input) return;
    
    const value = input.value;

    // Remove all non-digits
    let digits = value.replace(/\D/g, '');
    
    // Convert to number (cents to decimal)
    const amount = digits ? parseInt(digits, 10) / 100 : 0;
    
    // Format for display
    const formatted = this.format(amount);
    
    // Update input display
    input.value = formatted;
    
    // Update form control value as number
    this.control.control?.setValue(amount, { 
      emitEvent: false,
      emitModelToViewChange: false,
      emitViewToModelChange: true
    });
  }

  @HostListener('blur')
  onBlur() {
    const value = this.control.value;
    this.formatValue(value);
  }

  private formatValue(value: any) {
    const amount = typeof value === 'number' ? value : 0;
    this.el.nativeElement.value = this.format(amount);
  }

  private format(amount: number): string {
    const currency = this.authService.userCurrency();
    const locale = currency === 'BRL' ? 'pt-BR' : 'en-US';
    
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency: currency,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount);
  }
}
