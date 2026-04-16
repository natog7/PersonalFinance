import { Component, inject, signal, computed } from '@angular/core';
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

  hoveredIndex = signal<number | null>(null);

  /** Calculate SVG segment properties */
  readonly segments = computed(() => {
    const categories = this.spending().categories;
    let cumulativePercent = 0;

    return categories.map((cat, index) => {
      const segment = {
        ...cat,
        index,
        dashArray: `${cat.percent} 100`,
        dashOffset: -cumulativePercent,
      };
      cumulativePercent += cat.percent;
      return segment;
    });
  });

  setHovered(index: number | null): void {
    this.hoveredIndex.set(index);
  }

  trackByLabel(_: number, item: any): string {
    return item.label;
  }
}
