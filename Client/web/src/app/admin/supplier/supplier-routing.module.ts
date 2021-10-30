import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SupplierListComponent } from './supplier-list/supplier-list.component';
import { SupplierDetailComponent } from './supplier-detail/supplier-detail.component';
import { PurchaseOrderListComponent } from './purchase-order-list/purchase-order-list.component';
import { PurchaseOrderDetailComponent } from './purchase-order-detail/purchase-order-detail.component';
import { PurchaseReportComponent } from './purchase-report/purchase-report.component';
import { LocationListComponent } from './location-list/location-list.component';
import { LocationDetailComponent } from './location-detail/location-detail.component';

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
  }, {
    path: 'purchase-report',
    component: PurchaseReportComponent,
    data: {
      title: 'Purchase Report'
    }
  }, {
    path: 'locations',
    component: LocationListComponent,
    data: {
      title: 'Locations'
    }
  }, {
    path: 'locations/:action/:id',
    component: LocationDetailComponent,
    data: {
      title: 'Supplier Detail'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupplierRoutingModule { }
