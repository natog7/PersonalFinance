import { Component, Output, EventEmitter, inject } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-shortcuts',
  standalone: true,
  imports: [NgClass],
  templateUrl: './shortcuts.html',
  styleUrl: './shortcuts.scss'
})
export class ShortcutsComponent {
  private readonly dashboardService = inject(DashboardService);
  readonly isBalanceHidden = this.dashboardService.isBalanceHidden;
  
  @Output() createTransaction = new EventEmitter<void>();
  @Output() createCategory = new EventEmitter<void>();
  @Output() createGoal = new EventEmitter<void>();

  toggleBalance(): void {
    const currentState = this.dashboardService.isBalanceHidden();
    this.dashboardService.toggleBalanceVisibility(!currentState);
  }
}
