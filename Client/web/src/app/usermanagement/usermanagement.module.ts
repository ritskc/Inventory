import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsermanagementRoutingModule } from './usermanagement-routing.module';
import { ReportsComponent } from './reports/reports.component';
import { SharedModule } from '../common/shared/shared.module';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ng6-toastr-notifications';
import { UserManagementComponent } from './user-management/user-management.component';
import { ReportManagementComponent } from './report-management/report-management.component';

@NgModule({
  declarations: [
    ReportsComponent,
    UserManagementComponent,
    ReportManagementComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    UsermanagementRoutingModule,
    ToastrModule.forRoot()
  ]
})
export class UsermanagementModule { }
