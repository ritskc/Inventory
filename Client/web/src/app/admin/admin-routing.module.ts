import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AuthGuard } from '../user/auth.guard';
import { UserManagementComponent } from './user-management/user-management.component';

const routes: Routes = [{
  path: 'admin',
  component: AdminDashboardComponent,
  canActivate: [AuthGuard],
  children: [{
    path: '',
    children: [{
      path: 'user-management',
      component: UserManagementComponent
    }, {
      path: '',
      component: AdminDashboardComponent
    }]
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
