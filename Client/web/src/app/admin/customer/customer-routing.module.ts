import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerDetailComponent } from './customer-detail/customer-detail.component';
import { PurchaseOrdersComponent } from './purchase-orders/purchase-orders.component'

const routes: Routes = [{
  path: '',
  data: {
    title: 'Customers'
  }, 
  children: [{
    path: '',
    component: CustomerListComponent
  }, {
    path: 'purchase-order/:id/:action',
    component: PurchaseOrdersComponent,
    data: {
      title: 'Orders'
    }
  },{
    path: 'detail/:action/:id',
    component: CustomerDetailComponent,
    data: {
      title: 'Detail'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerRoutingModule { }
