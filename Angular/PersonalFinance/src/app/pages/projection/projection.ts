import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { SidebarStateService } from '../../shared/services/sidebar-state.service';

@Component({
  selector: 'app-projection',
  standalone: true,
  imports: [CommonModule, SidebarComponent, HeaderComponent],
  templateUrl: './projection.html',
  styleUrl: './projection.scss'
})
export class ProjectionComponent {
  sidebarState = inject(SidebarStateService);
}
