import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SupplierListComponent } from './supplier-list/supplier-list.component';
import { SupplierDetailComponent } from './supplier-detail/supplier-detail.component';
import { PurchaseOrderListComponent } from './purchase-order-list/purchase-order-list.component';
import { PurchaseOrderDetailComponent } from './purchase-order-detail/purchase-order-detail.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Suppliers'
  }, 
  children: [{
    path: '',
    component: SupplierListComponent
  }, {
    path: 'detail/:action/:id',
    component: SupplierDetailComponent,
    data: {
      title: 'Supplier Detail'
    }
  }, {
    path: 'purchase-order/:id/:action',
    component: PurchaseOrderListComponent,
    data: {
      title: 'Purchase Orders'
    }
  }, {
    path: 'pos/:supplierId/:posId',
    component: PurchaseOrderDetailComponent,
    data: {
      title: 'Purchase Order'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupplierRoutingModule { }
