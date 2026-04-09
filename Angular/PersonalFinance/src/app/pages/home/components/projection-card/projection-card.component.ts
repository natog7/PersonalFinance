import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'app-projection-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './projection-card.component.html',
  styleUrl: './projection-card.component.scss',
})
export class ProjectionCardComponent {
  private readonly dashboardService = inject(DashboardService);
  readonly projection = this.dashboardService.projection;
}
