import { Component, inject, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../../../shared/services/auth.service';

@Component({
  selector: 'app-projection-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './projection-card.component.html',
  styleUrl: './projection-card.component.scss',
})
export class ProjectionCardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  readonly projection = this.dashboardService.projection;
  readonly isBalanceHidden = this.dashboardService.isBalanceHidden;
  readonly authService = inject(AuthService);

  ngOnInit(): void {
    const today = new Date();
    // Calculate dates for the NEXT month
    const firstDay = new Date(today.getFullYear(), today.getMonth() + 1, 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 2, 0);

    const start = `${firstDay.getFullYear()}-${String(firstDay.getMonth() + 1).padStart(2, '0')}-01`;
    const end = `${lastDay.getFullYear()}-${String(lastDay.getMonth() + 1).padStart(2, '0')}-${String(lastDay.getDate()).padStart(2, '0')}`;

    // Pass `true` for isProjection parameter
    this.dashboardService.fetchBalanceByMonth(start, end, [], true);
  }
}
