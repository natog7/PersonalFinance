import { Component, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { CategoryExpense } from '../../models/dashboard.model';

@Component({
  selector: 'app-category-chart-card',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './category-chart-card.component.html',
  styleUrl: './category-chart-card.component.scss',
})
export class CategoryChartCardComponent {
  private readonly dashboardService = inject(DashboardService);
  readonly spending = this.dashboardService.spending;
  readonly isBalanceHidden = this.dashboardService.isBalanceHidden;

  /** Build the CSS conic-gradient string from category data */
  get donutGradient(): string {
    const categories = this.spending().categories;
    let cumulative = 0;
    const stops: string[] = [];

    for (const cat of categories) {
      const start = cumulative;
      const end = cumulative + cat.percent;
      stops.push(`${cat.colorVar} ${start}% ${end}%`);
      cumulative = end;
    }

    return `conic-gradient(${stops.join(', ')})`;
  }

  trackByLabel(_: number, item: CategoryExpense): string {
    return item.label;
  }
}
