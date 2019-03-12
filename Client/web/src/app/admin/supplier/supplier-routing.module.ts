import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SupplierListComponent } from './supplier-list/supplier-list.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Suppliers'
  }, 
  children: [{
    path: '',
    component: SupplierListComponent
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupplierRoutingModule { }
