import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InvoiceListComponent } from './invoice-list/invoice-list.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Invoice'
  }, children: [{
    path: '',
    component: InvoiceListComponent,
    data: {
      title: 'List'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InvoiceRoutingModule { }
