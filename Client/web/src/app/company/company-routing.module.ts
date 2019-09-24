import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyDetailComponent } from './company-detail/company-detail.component';
import { CreateShipmentComponent } from './create-shipment/create-shipment.component';
import { InventoryPartsListComponent } from './inventory-parts-list/inventory-parts-list.component';
import { ShipmentListComponent } from './shipment-list/shipment-list.component';

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
      path: 'create-shipment',
      component: CreateShipmentComponent,
      data: {
        title: 'Create Shipment'
      }
    },
    {
      path: 'create-shipment/:id',
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
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompanyRoutingModule { }
