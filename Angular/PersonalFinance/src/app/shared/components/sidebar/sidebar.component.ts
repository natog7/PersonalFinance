import { Component, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';


export interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  investClick = output<void>();
  logoutClick = output<void>();

  readonly navItems: NavItem[] = [
    { label: 'Início',       icon: 'dashboard',               route: '/home' },
    { label: 'Projeção',     icon: 'trending_up',             route: '/projection' },
    { label: 'Transações',   icon: 'account_balance_wallet',  route: '/transactions' },
    { label: 'Categorias',   icon: 'category',                route: '/categories' },
  ];

  onInvest(): void {
    this.investClick.emit();
  }

  onLogout(): void {
    this.logoutClick.emit();
  }
}
