import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyDetailComponent } from './company-detail/company-detail.component';
import { CreateShipmentComponent } from './create-shipment/create-shipment.component';
import { InventoryPartsListComponent } from './inventory-parts-list/inventory-parts-list.component';
import { ShipmentListComponent } from './shipment-list/shipment-list.component';
import { CompanyInvoiceComponent } from './company-invoice/company-invoice.component';
import { POSUploadComponent } from './pos/upload/upload.component';
import { MasterShipmentListComponent } from './master-shipment-list/master-shipment-list.component';
import { MasterShipmentDetailComponent } from './master-shipment-detail/master-shipment-detail.component';
import { SaleReportComponent } from './sale-report/sale-report.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Companies'
  },
  children: [
    {
      path: '',
      component: CompanyListComponent
    },
    {
      path: 'shipment-list',
      component: ShipmentListComponent,
      data: {
        title: 'View Shipments'
      }
    },
    {
      path: 'master-shipment-list',
      component: MasterShipmentListComponent,
      data: {
        title: 'Master Shipments'
      }
    },
    {
      path: 'master-shipment-detail/:customerId/:mode/:shipmentId',
      component: MasterShipmentDetailComponent,
      data: {
        title: 'Master Shipment Detail'
      }
    },
    {
      path: 'create-shipment',
      component: CreateShipmentComponent,
      data: {
        title: 'Create Shipment'
      }
    },
    {
      path: 'create-shipment/:id/:action/0',
      component: CreateShipmentComponent,
      data: {
        title: 'Create Shipment'
      }
    },
    {
      path: 'create-shipment/:id/:action/:shipmentId',
      component: CreateShipmentComponent,
      data: {
        title: 'Create Shipment'
      }
    },
    {
      path: 'detail/:action/:id',
      component: CompanyDetailComponent,
      data: {
        title: 'Company Detail'
      }
    },
    {
      path: 'inventory',
      component: InventoryPartsListComponent,
      data: {
        title: 'Inventory'
      }
    },
    {
      path: 'invoice',
      component: CompanyInvoiceComponent,
      data: {
        title: 'Invoice'
      }
    },
    {
      path: 'invoice/:customerId/:shipmentId',
      component: CompanyInvoiceComponent,
      data: {
        title: 'Invoice'
      }
    },
    {
      path: 'pos',
      component: POSUploadComponent,
      data: {
        title: 'POS (Upload & Remove)'
      }
    }, 
    {
      path: 'sale-report',
      component: SaleReportComponent,
      data: {
        title: 'Sale Report'
      }
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompanyRoutingModule { }
