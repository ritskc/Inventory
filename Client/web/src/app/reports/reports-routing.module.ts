import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InventoryComponent } from './inventory/inventory.component';

const routes: Routes = [
  {
    path: '',
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
