import { Component, inject, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SidebarStateService } from '../../services/sidebar-state.service';
import { NgClass } from '@angular/common';


export interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgClass],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  readonly sidebarState = inject(SidebarStateService);
  investClick = output<void>();
  logoutClick = output<void>();

  readonly navItems: NavItem[] = [
    { label: 'Início', icon: 'dashboard', route: '/home' },
    { label: 'Projeção', icon: 'trending_up', route: '/projection' },
    { label: 'Transações', icon: 'account_balance_wallet', route: '/transactions' },
    { label: 'Categorias', icon: 'category', route: '/categories' },
  ];

  onInvest(): void {
    this.investClick.emit();
  }

  onLogout(): void {
    this.logoutClick.emit();
  }
}
