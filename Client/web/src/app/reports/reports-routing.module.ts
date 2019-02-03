import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InventoryComponent } from './inventory/inventory.component';
import { AuthGuard } from '../user/auth.guard';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    data: {
      title: 'Reports'
    },
    children: [{
      path: '',
      redirectTo: 'inventory',
      pathMatch: 'full'
    }, {
      path: 'inventory',
      component: InventoryComponent,
      data: {
        title: 'Inventory Management'
      }
    }]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
