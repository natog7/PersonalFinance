import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { BalanceCardComponent } from './components/balance-card/balance-card.component';
import { ProjectionCardComponent } from './components/projection-card/projection-card.component';
import { CashFlowCardComponent } from './components/cash-flow-card/cash-flow-card.component';
import { CategoryChartCardComponent } from './components/category-chart-card/category-chart-card.component';
import { TransactionsCardComponent } from './components/transactions-card/transactions-card.component';
import { ShortcutsComponent } from './components/shortcuts/shortcuts';
import { SidebarStateService } from '../../shared/services/sidebar-state.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NgClass,
    SidebarComponent,
    HeaderComponent,
    BalanceCardComponent,
    ProjectionCardComponent,
    CategoryChartCardComponent,
    TransactionsCardComponent,
    ShortcutsComponent,
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class HomeComponent {
  readonly sidebarState = inject(SidebarStateService);

  onNewTransaction(): void {
    // TODO: open transaction modal
    console.log('Nova Transação');
  }

  onInvest(): void {
    // TODO: navigate to investment flow
    console.log('Investir Agora');
  }

  onLogout(): void {
    // TODO: handle auth logout
    console.log('Sair');
  }
}
