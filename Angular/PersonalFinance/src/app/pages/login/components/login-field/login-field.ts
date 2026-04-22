import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { ReactiveFormsModule, ControlContainer, FormGroupDirective } from '@angular/forms';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-login-field',
  standalone: true,
  imports: [NgClass, ReactiveFormsModule],
  templateUrl: './login-field.html',
  styleUrl: './login-field.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  viewProviders: [
    {
      provide: ControlContainer,
      useExisting: FormGroupDirective,
    },
  ],
})
export class LoginField {
  fieldId = input.required<string>();
  label = input<string>();
  labelLink = input<string>();
  placeholder = input<string>();
  type = input.required<string>();
  icon = input<string>();
  buttonIcon = input<string>();

  buttonClick = output<void>();
  labelLinkClick = output<void>();

  onButtonClick(): void {
    this.buttonClick.emit();
  }

  onLabelLinkClick(): void {
    this.labelLinkClick.emit();
  }
}
