import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SupplierListComponent } from './supplier-list/supplier-list.component';
import { SupplierDetailComponent } from './supplier-detail/supplier-detail.component';

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
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupplierRoutingModule { }