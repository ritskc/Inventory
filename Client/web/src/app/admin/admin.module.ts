import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { SharedModule } from '../common/shared/shared.module';
import { ContainerEditComponent } from './container/edit/edit.component';

@NgModule({
  declarations: [AdminDashboardComponent, ContainerEditComponent],
  imports: [
    CommonModule,
    AdminRoutingModule,
    SharedModule
  ]
})
export class AdminModule { }
