import { Component, inject, output } from '@angular/core';
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
  
  createTransaction = output<void>();
  createCategory = output<void>();
  createGoal = output<void>();

  toggleBalance(): void {
    const currentState = this.dashboardService.isBalanceHidden();
    this.dashboardService.toggleBalanceVisibility(!currentState);
  }
}
