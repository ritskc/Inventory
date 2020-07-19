import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AuthGuard } from '../user/auth.guard';

const routes: Routes = [{
  path: '',
  canActivate: [AuthGuard],
  data: {
    title: 'Admin'
  },
  children: [{
    path : '',
    redirectTo: 'user-management',
    pathMatch: 'full'
    }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
