import { Component, Output, EventEmitter, input } from '@angular/core';

@Component({
  selector: 'app-account-menu',
  standalone: true,
  imports: [],
  templateUrl: './account-menu.html',
  styleUrl: './account-menu.scss'
})
export class AccountMenu {
  isOpen = input<boolean>(false);
  @Output() logout = new EventEmitter<void>();
  @Output() settings = new EventEmitter<void>();
}
