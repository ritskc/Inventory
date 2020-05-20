import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReportsComponent } from './reports/reports.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { ReportManagementComponent } from './report-management/report-management.component';

const routes: Routes = [{
  path: '',
  children: [
    {
      path: 'reports',
      component: ReportsComponent
    }, {
      path: 'users',
      component: UserManagementComponent
    }, {
      path: 'reports-management',
      component: ReportManagementComponent
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsermanagementRoutingModule { }
